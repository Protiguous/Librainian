﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries,
// repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper licenses and/or copyrights.
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ThreadingExtensions.cs" last formatted on 2022-02-16 at 2:25 PM by Protiguous.


namespace Librainian.Threading;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Logging;
using Maths;
using OperatingSystem;

public static class ThreadingExtensions {

	/// <summary>
	///     <para>Thread.BeginThreadAffinity();</para>
	///     <para>Thread.BeginCriticalRegion();</para>
	///     <para>...</para>
	///     <see cref="End" />
	/// </summary>
	/// <param name="lowPriority"></param>
	public static void Begin( Boolean lowPriority = true ) {
		Thread.BeginThreadAffinity();
		Thread.BeginCriticalRegion();

		if ( lowPriority && ( Thread.CurrentThread.Priority != ThreadPriority.Lowest ) ) {
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
		}
	}

	/// <summary>About X bytes by polling the object's fields.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	public static UInt64 CalcSizeInBytes<T>( this T? obj ) {
		if ( obj is null ) {
			return 0;
		}

		var sizeInBytes = 0UL;

		var fields = obj.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

		foreach ( var value in from field in fields select field.GetValue( obj ) ) {
			switch ( value ) {
				case IEnumerable<T> enumerable: {
						sizeInBytes = enumerable.Aggregate( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() );

						continue;
					}
				case IEnumerable enumerable: {
						sizeInBytes = enumerable.Cast<Object?>().Aggregate( sizeInBytes, ( current, o ) => current + o.CalcSizeInBytes() );

						continue;
					}
				default: {
						sizeInBytes += value.GetSizeOfAnyPrimitives();
						break;
					}
			}
		}

		return sizeInBytes;
	}

	/// <summary>
	///     <para>
	///         <see cref="Begin" />
	///     </para>
	///     <para>...</para>
	///     <para>Thread.EndThreadAffinity();</para>
	///     <para>Thread.EndCriticalRegion();</para>
	/// </summary>
	public static void End() {
		Thread.EndThreadAffinity();
		Thread.EndCriticalRegion();
	}

	public static TaskAwaiter<Int32> GetAwaiter( this Process process ) {
		var tcs = new TaskCompletionSource<Int32>();
		process.EnableRaisingEvents = true;
		process.Exited += ( _, _ ) => tcs.TrySetResult( process.ExitCode );
		if ( process.HasExited ) {
			tcs.TrySetResult( process.ExitCode );
		}

		return tcs.Task.GetAwaiter();
	}

	public static TaskAwaiter<Boolean> GetAwaiter( this Action action ) {
		var tcs = new TaskCompletionSource<Boolean>();
		try {
			action();

			tcs.TrySetResult( true );
		}
		catch ( Exception exception ) {
			tcs.TrySetResult( false );
			exception.Log();
		}

		return tcs.Task.GetAwaiter();
	}

	/// <summary>Asynchronously wait until cancellation is requested.</summary>
	/// <param name="cancellationToken"></param>
	public static TaskAwaiter GetAwaiter( this CancellationToken cancellationToken ) {
		var tcs = new TaskCompletionSource<Boolean>();
		Task t = tcs.Task;
		if ( cancellationToken.IsCancellationRequested ) {
			tcs.SetResult( true );
		}
		else {
			cancellationToken.Register( s => ( s as TaskCompletionSource<Boolean> )?.SetResult( true ), tcs );
		}

		return t.GetAwaiter();
	}

	/// <summary>Asynchronously wait for all <paramref name="tasks" />.</summary>
	/// <param name="tasks"></param>
	public static TaskAwaiter GetAwaiter( this Task[] tasks ) => Task.WhenAll( tasks ).GetAwaiter();

	/// <summary>Asynchronously wait for all <paramref name="tasks" />.</summary>
	/// <param name="tasks"></param>
	public static TaskAwaiter GetAwaiter( this IEnumerable<Task> tasks ) => Task.WhenAll( tasks ).GetAwaiter();

	/// <summary>Asynchronously wait a <see cref="TimeSpan" />.</summary>
	/// <param name="timeSpan"></param>
	public static TaskAwaiter GetAwaiter( this TimeSpan timeSpan ) => Task.Delay( timeSpan ).GetAwaiter();

	[DllImport( DLL.Kernel32 )]
	public static extern void GetCurrentThreadStackLimits( out UInt32 lowLimit, out UInt32 highLimit );

	public static Int32 GetMaximumActiveWorkerThreads() {
		ThreadPool.GetMaxThreads( out var _, out var maxPortThreads );

		return maxPortThreads;
	}

	public static UInt64 GetSizeOfAnyPrimitives<T>( this T? obj ) {
		unsafe {
			return ( UInt64 )( obj switch {
				Boolean => sizeof( Boolean ),
				Byte => sizeof( Byte ),
				SByte => sizeof( SByte ),
				Char => sizeof( Char ),
				Int16 => sizeof( Int16 ),
				UInt16 => sizeof( UInt16 ),
				Int32 => sizeof( Int32 ),
				UInt32 => sizeof( UInt32 ),
				Int64 => sizeof( Int64 ),
				UInt64 => sizeof( UInt64 ),
				Single => sizeof( Single ),
				Double => sizeof( Double ),
				Decimal => sizeof( Decimal ),
				String s => sizeof( Char ) * s.Length,
				IntPtr => sizeof( nint ), //BUG or correct?
				UIntPtr => sizeof( nint ), //BUG or correct?
				{ } => sizeof( void* ), //BUG or correct?
				var _ => sizeof( void* ) //BUG or correct?
			} );
		}
	}

	/// <summary>returns Marshal.SizeOf( typeof( T ) );</summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerStepThrough]
	public static Int32 MarshalSizeOf<T>() where T : struct => Marshal.SizeOf( typeof( T ) );

	/// <summary>boxed returns Marshal.SizeOf( obj )</summary>
	/// <param name="obj"></param>
	public static Int32 MarshalSizeOf( this Object obj ) => Marshal.SizeOf( obj );

	/// <summary>generic returns Marshal.SizeOf( obj )</summary>
	/// <param name="obj"></param>
	public static Int32 MarshalSizeOf<T>( [DisallowNull] this T obj ) => Marshal.SizeOf( obj );

	/// <summary>
	///     Repeat the <paramref name="action" /><paramref name="times" />.
	///     <para>Swallows all exceptions.</para>
	/// </summary>
	/// <param name="times"></param>
	/// <param name="action"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void Repeat( this Int32 times, Action action ) {
		if ( action is null ) {
			throw new ArgumentNullException( nameof( action ) );
		}

		if ( !times.Any() ) {
			return;
		}

		for ( var i = 0; i < times; i++ ) {
			try {
				action();
			}
			catch { }
		}
	}

	/// <summary>
	///     <para>Swallows <see cref="Exception" />.</para>
	/// </summary>
	/// <param name="action"></param>
	/// <param name="times"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public static void Repeat( this Action action, Int32 times ) {
		if ( action is null ) {
			throw new ArgumentNullException( nameof( action ) );
		}

		if ( !times.Any() ) {
			return;
		}

		for ( var i = 0; i < times; i++ ) {
			try {
				action();
			}
			catch { }
		}
	}

	/// <summary>
	///     <para>Swallows <see cref="Exception" />.</para>
	/// </summary>
	/// <param name="action"></param>
	/// <param name="counter"></param>
	/// <param name="useTryCatch"></param>
	public static void RepeatAction( this Action action, Int32 counter, Boolean useTryCatch ) =>
		Parallel.For( 1, counter, _ => {
			if ( useTryCatch ) {
				try {
					action();
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			}
			else {
				action();
			}
		} );

	/// <summary>
	///     Run each <see cref="Action" />, optionally in parallel (defaults to true), optionally printing feedback through an
	///     <see cref="Action" />.
	/// </summary>
	/// <param name="actions"></param>
	/// <param name="output"></param>
	/// <param name="description"></param>
	/// <param name="inParallel"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static Boolean Run( this IEnumerable<Action> actions, Action<String>? output = null, String? description = null, Boolean inParallel = true ) {
		if ( actions is null ) {
			throw new ArgumentEmptyException( nameof( actions ) );
		}

		if ( ( output != null ) && !String.IsNullOrWhiteSpace( description ) ) {
			output( description );
		}

		if ( inParallel ) {
			var result = Parallel.ForEach( actions.AsParallel().AsUnordered(), action => action() );

			return result.IsCompleted;
		}

		foreach ( var action in actions ) {
			action();
		}

		return true;
	}

	/// <summary>Run each <see cref="Func{Boolean}" /> in parallel, optionally printing feedback through an action.</summary>
	/// <param name="functions"></param>
	/// <param name="output"></param>
	/// <param name="description"></param>
	/// <param name="inParallel"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	public static Boolean Run( this IEnumerable<Func<Boolean>> functions, Action<String>? output = null, String? description = null, Boolean inParallel = true ) {
		if ( functions is null ) {
			throw new ArgumentEmptyException( nameof( functions ) );
		}

		if ( ( output != null ) && !String.IsNullOrWhiteSpace( description ) ) {
			output( description );
		}

		if ( inParallel ) {
			var result = Parallel.ForEach( functions.AsParallel(), function => {
				try {
					function();
				}
				catch ( Exception exception ) {
					exception.Log();
				}
			} );

			return result.IsCompleted;
		}

		foreach ( var function in functions ) {
			try {
				function();
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

		return true;
	}
}