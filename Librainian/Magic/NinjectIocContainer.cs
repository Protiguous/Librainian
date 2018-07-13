// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "NinjectIocContainer.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "NinjectIocContainer.cs" was last formatted by Protiguous on 2018/07/13 at 1:16 AM.

namespace Librainian.Magic {

	using System;
	using System.Diagnostics;
	using Collections;
	using Extensions;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Ninject;
	using Ninject.Activation.Caching;
	using Ninject.Modules;

	public sealed class NinjectIocContainer : ABetterClassDispose, IIocContainer {

		public IKernel Kernel { get; }

		/// <summary>
		///     Returns a new instance of the given type or throws NullReferenceException.
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		[DebuggerStepThrough]
		public TType Get<TType>() {
			var tryGet = this.Kernel.TryGet<TType>();

			if ( Equals( default, tryGet ) ) {
				tryGet = this.Kernel.TryGet<TType>(); //HACK why would it work at the second time?

				if ( Equals( default, tryGet ) ) { throw new NullReferenceException( "Unable to TryGet() class " + typeof( TType ).FullName ); }
			}

			return tryGet;
		}

		public void Inject( Object item ) => this.Kernel.Inject( item );

		/// <summary>
		///     Warning!
		/// </summary>
		public void ResetKernel() {
			this.Kernel.Should().NotBeNull();
			this.Kernel.GetModules().ForEach( module => this.Kernel.Unload( module.Name ) );
			this.Kernel.Components.Get<ICache>().Clear();
			this.Kernel.Should().NotBeNull();

			//Log.Before( "Ninject is loading assemblies..." );
			this.Kernel.Load( AppDomain.CurrentDomain.GetAssemblies() );

			//Log.After( $"loaded {this.Kernel.GetModules().Count()} assemblies." );
			$"{this.Kernel.GetModules().ToStrings()}".WriteLine();
		}

		/// <summary>
		///     Re
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <returns></returns>
		[DebuggerStepThrough]
		public TType TryGet<TType>() {
			var tryGet = this.Kernel.TryGet<TType>();

			if ( Equals( default, tryGet ) ) {
				tryGet = this.Kernel.TryGet<TType>(); //HACK wtf??
			}

			return tryGet;
		}

		// ReSharper disable once NotNullMemberIsNotInitialized
		public NinjectIocContainer( [NotNull] params INinjectModule[] modules ) {
			if ( modules is null ) { throw new ArgumentNullException( nameof( modules ) ); }

			this.Kernel.Should().BeNull();
			"Loading IoC kernel...".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
			this.Kernel = new StandardKernel( modules );
			this.Kernel.Should().NotBeNull();

			if ( null == this.Kernel ) { throw new InvalidOperationException( "Unable to load kernel!" ); }

			"done.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
		}

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() => this.Kernel.Dispose();

		//public object Get( Type type ) {
		//    return this.Kernel.Get( type );
		//}

		//public T Get<T>() {
		//    var bob = this.Kernel.TryGet<T>();
		//    return bob;
		//}

		//public T Get<T>( String name, String value ) {
		//    var result = this.Kernel.TryGet<T>( metadata => metadata.Has( name ) && metadata.Get<String>( name ).Like( value ) );

		//    if ( Equals( result, default( T ) ) ) {
		//        throw new InvalidOperationException( null );
		//    }
		//    return result;
		//}
	}
}