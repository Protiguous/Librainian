// Copyright © Protiguous. All Rights Reserved.
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
// File "Common.cs" last formatted on 2022-02-14 at 6:47 AM by Protiguous.


namespace Librainian.Utilities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Exceptions;
using Maths;
using Measurement;
using Newtonsoft.Json;
using Parsing;

public static class Common {

	/// <summary>
	/// <para>Gets or sets the default <see cref="Encoding"/> used in <see cref="Librainian"/>.</para>
	/// <para>Defaults to <see cref="Encoding.Unicode"/>.</para>
	/// </summary>
	public static Encoding DefaultEncoding { get; set; } = Encoding.Unicode;

	/// <summary>Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two inclusive values.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="startInclusive"></param>
	/// <param name="endInclusive"></param>
	/// <example>5.Between(1, 10)</example>
	/// <example>5.Between(10, 1)</example>
	/// <example>5.Between(10, 6) == false</example>
	/// <example>5.Between(5, 5))</example>
	[NeedsTesting]
	public static Boolean Between<T>( this T target, T startInclusive, T endInclusive ) where T : IComparable<T> {
		EnsureMinimumMaximum( ref startInclusive, ref endInclusive );

		return ( target.CompareTo( startInclusive ) >= SortingOrder.Same ) && ( target.CompareTo( endInclusive ) <= SortingOrder.Same );
    }



	/// <summary>
	///     Returns a new <typeparamref name="T" /> that is the value of <paramref name="self" />, constrained between
	///     <paramref
	///         name="minimum" />
	///     and <paramref name="maximum" />.
	/// </summary>
	/// <param name="self">The extended T.</param>
	/// <param name="minimum">The minimum value of the <typeparamref name="T" /> that can be returned.</param>
	/// <param name="maximum">The maximum value of the <typeparamref name="T" /> that can be returned.</param>
	/// <returns>The equivalent to: <c>this &lt; minimum ? minimum : this &gt; maximum ? maximum : this</c>.</returns>
	[NeedsTesting]
	public static T Clamp<T>( this T self, T minimum, T maximum ) where T : IComparable<T> {
        EnsureMinimumMaximum( ref minimum, ref maximum );

		if ( self.CompareTo( minimum ) < SortingOrder.Same ) {
			return minimum;
		}

		return self.CompareTo( maximum ) > 0 ? maximum : self;
	}

	public static IEnumerable<T?> Concat<T>( this IEnumerable<T> first, T? second ) {
		foreach ( var item in first ) {
			yield return item;
		}

		yield return second;
	}

	public static T[] Concat<T>( this T[] array1, T[] array2 ) {
		var result = new T[array1.LongLength + array2.LongLength];
		array1.CopyTo( result, 0 );
		array2.CopyTo( result, array1.LongLength );

		return result;
	}

	public static IEnumerable<T> Concat<T>( this IEnumerable<T> left, IEnumerable<T> right ) {
		foreach ( var a in left ) {
			yield return a;
		}

		foreach ( var b in right ) {
			yield return b;
		}
	}

	/// <summary>Swap <paramref name="minimum" /> with <paramref name="maximum" /> if <paramref name="minimum"/> is greater.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="minimum"></param>
	/// <param name="maximum"></param>
	[DebuggerStepThrough]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void EnsureMinimumMaximum<T>( ref T minimum, ref T maximum ) where T : IComparable<T> {

		if ( minimum.CompareTo( maximum ) >= CompareToOrder.After ) {
			Swap( ref minimum, ref maximum );
		}
	}

    /// <summary>Swap <paramref name="minimum" /> with <paramref name="maximum" />.</summary>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    [DebuggerStepThrough]
	public static void EnsureMinimumMaximum( this ref Int32 minimum, ref Int32 maximum, out Int32 range ) {
		if ( minimum.CompareTo( maximum ) >= CompareToOrder.After ) {
			Swap( ref minimum, ref maximum );
		}

        range = maximum - minimum;
    }

    /// <summary>Swap <paramref name="minimum" /> with <paramref name="maximum" />.</summary>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    [DebuggerStepThrough]
	public static void EnsureMinimumMaximum( this ref Int64 minimum, ref Int64 maximum, out Int64 range ) {
		if ( minimum.CompareTo( maximum ) >= CompareToOrder.After ) {
			Swap( ref minimum, ref maximum );
		}

        range = maximum - minimum;
    }

	public static String GetApplicationName( String defaultOtherwise ) =>
			Application.ProductName.Trimmed() ?? defaultOtherwise.Trimmed() ?? throw new NullException( nameof( GetApplicationName ) );

	public static Boolean IsDevelopmentEnviroment<T>( this T _ ) {
		var devEnvironmentVariable = Environment.GetEnvironmentVariable( "NETCORE_ENVIRONMENT" );

		var isDevelopment = String.IsNullOrEmpty( devEnvironmentVariable ) || devEnvironmentVariable.Like( "development" );
		return isDevelopment;
	}

	[NeedsTesting]
	public static UInt64 LengthReal( this String? s ) => s is null ? 0 : ( UInt64 )new StringInfo( s ).LengthInTextElements;

	/// <summary>
	///     Gets a <b>horribly</b> ROUGH guesstimate of the memory consumed by an object by using
	///     <see
	///         cref="JsonConvert" />
	///     .
	/// </summary>
	/// <param name="bob"></param>
	[NeedsTesting]
	public static UInt64 MemoryUsed<T>( [DisallowNull] this T bob ) => JsonConvert.SerializeObject( bob, Formatting.None ).LengthReal();

	/// <summary>Just a no-op for setting a breakpoint on.</summary>
	/// <param name="_"></param>
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	[SuppressMessage( "Critical Code Smell", "S1186:Methods should not be empty", Justification = "<Pending>" )]
	public static void Nop<T>( this T? _ ) { }

	/// <summary>Just a no-op for setting a breakpoint on.</summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	[Conditional( "DEBUG" )]
	[SuppressMessage( "Critical Code Smell", "S1186:Methods should not be empty", Justification = "<Pending>" )]
	public static void Nop() { }

	/// <summary>
	///     <para>Works like the SQL "nullif" function.</para>
	///     <para>
	///         If <paramref name="left" /> is equal to <paramref name="right" /> then return null for classes or the default
	///         value for
	///         value types.
	///     </para>
	///     <para>Otherwise return <paramref name="left" />.</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[DebuggerStepThrough]
	public static T? NullIf<T>( this T? left, T? right, IComparer<T>? comparer = null ) where T : class {

        comparer ??= Comparer<T>.Default;

        return comparer.Compare( left, right ) == CompareOrder.Same ? default( T? ) : left;
    }

    /// <summary>
	/// Swap the <paramref name="left"/> with the <paramref name="right"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Swap<T>( ref T left!!, ref T right!! ) => (left, right) = (right, left);



	public static void Swap<T>( this Span<T> left!!, Span<T> right!!, Int32 leftIndex, Int32 rightIndex ) {
        if ( Invalid(left,right,leftIndex,rightIndex) ) {
            return;
        }

        ( left[ leftIndex ], right[ rightIndex ] ) = ( right[ rightIndex ], left[ leftIndex ] );

        static Boolean Invalid( Span<T> left, Span<T> right, Int32 leftIndex, Int32 rightIndex ) {
            if ( leftIndex.NotAny() ) {
                throw new ArgumentOutOfRangeException( nameof( leftIndex ) );
            }
            if ( rightIndex.NotAny() ) {
                throw new ArgumentOutOfRangeException( nameof( rightIndex ) );
            }

            if ( leftIndex.Between(leftIndex.ToStringWithBase) ) {
                
            }
        }
    }

    public static void Swap<T>( this Memory<T> left, Memory<T> right, Int32 leftIndex, Int32 rightIndex ) {
        ( left[ leftIndex ], right[rightIndex] ) = ( right[ rightIndex ], left[ leftIndex ] );
    }

    /// <summary>Given (T left, T right), Return (T right, T left).</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static (T? right, T? left) Swap<T>( this T? left, T? right ) => (right, left);

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static (T? right, T? left) Swap<T>( (T? left, T? right) tuple ) => (tuple.right, tuple.left);

	/// <summary>
	///     Swap the two indexes in the array.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="index1"></param>
	/// <param name="index2"></param>
	/// <exception cref="ArgumentEmptyException"></exception>
	/// <exception cref="OutOfRangeException"></exception>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void Swap<T>( this T[] array!!, Int32 index1, Int32 index2 ) {
		if ( Invalid(index1,index2, array.Length ) ) {
            return;
        }

        Swap(ref array[index1], ref array[index2] );
		//(array[index1], array[index2]) = (array[index2], array[index1]);

        static Boolean Invalid( Int32 index1, Int32 index2, Int32 length ) {
            if ( index1 == index2 ) {
                return true;
            }

            if ( index1.NotAny() ) {
                throw new OutOfRangeException( $"{nameof( index1 )} cannot be less than 0." );
            }

            if ( index2.NotAny() ) {
                throw new OutOfRangeException( $"{nameof( index2 )} cannot be less than 0." );
            }

            if ( index1 >= length ) {
                throw new OutOfRangeException( $"{nameof( index1 )} cannot be greater than {length - 1}." );
            }

            if ( index2 >= length ) {
                throw new OutOfRangeException( $"{nameof( index2 )} cannot be greater than {length - 1}." );
            }

            return false;
        }
    }

	/// <summary>Swap <paramref name="left" /> with <paramref name="right" />.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="left"></param>
	/// <param name="right"></param>
	[DebuggerStepThrough]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static void SwapNullable<T>( ref T? left, ref T? right ) => (left, right) = (right, left);

	/// <summary>
	///     Convert any number of strings into a Key (keys to use for caching) Using the reasoning that a string lookup will
	///     match sooner by having the most selective "key" first.
	/// </summary>
	/// <param name="keys"></param>
	public static String ToKey( params String[] keys ) =>
		keys.Any( static s => s.Contains( Symbols.TripleTilde ) ) ? keys.ToStrings( Symbols.SkullAndCrossbones ) : keys.ToStrings( Symbols.TripleTilde );

	/// <summary>Create only 1 instance of <see cref="T" /> per thread. (Only unique when using this method!)</summary>
	/// <typeparam name="T"></typeparam>
	[NeedsTesting]
	public static class Cache<T> where T : notnull, new() {
		private static ThreadLocal<T> LocalCache { get; } = new( static () => new T(), false );

		public static T? Instance { get; } = LocalCache.Value;
	}

	/// <summary>Only create 1 instance of <see cref="T" /> reguardless of threads. (only unique when using this method!)</summary>
	/// <typeparam name="T"></typeparam>
	[NeedsTesting]
	public static class CacheGlobal<T> where T : notnull, new() {
		public static T Instance { get; } = new();
	}
}