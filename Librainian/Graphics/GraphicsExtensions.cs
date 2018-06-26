// Copyright � Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "GraphicsExtensions.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "GraphicsExtensions.cs" was last formatted by Protiguous on 2018/06/26 at 1:08 AM.

namespace Librainian.Graphics {

	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystems.FileSystem;
	using Extensions;
	using Imaging;
	using JetBrains.Annotations;
	using Moving;
	using OperatingSystem;

	public static class GraphicsExtensions {

		private static Boolean _gotGamma;

		public static RAMP DefaultRamp;

		static GraphicsExtensions() => DefaultRamp = GetGamma();

		[NotNull]
		public static Stream EfvToStream() {
			var ms = new MemoryStream();

			//TODO
			return ms;
		}

		public static RAMP GetGamma() {
			var ramp = new RAMP();

			if ( NativeMethods.GetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref ramp ) ) {
				_gotGamma = true;

				return ramp;
			}

			throw new InvalidOperationException( "Unable to obtain Gamma setting." );
		}

		/// <summary>
		///     Resets the gamma to when it was first called.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public static void ResetGamma() {
			if ( _gotGamma ) {
				NativeMethods.SetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref DefaultRamp );
			}
			else {
				throw new InvalidOperationException( "Unable to obtain Gamma setting on this device." );
			}
		}

		public static void SetGamma( Int32 gamma ) {
			if ( gamma.Between( 1, 256 ) ) {
				var ramp = new RAMP {
					Red = new UInt16[ 256 ],
					Green = new UInt16[ 256 ],
					Blue = new UInt16[ 256 ]
				};

				for ( var i = 1; i < 256; i++ ) {
					var iArrayValue = i * ( gamma + 128 );

					if ( iArrayValue > 65535 ) {
						iArrayValue = 65535;
					}

					ramp.Red[ i ] = ramp.Blue[ i ] = ramp.Green[ i ] = ( UInt16 ) iArrayValue;
				}

				NativeMethods.SetDeviceGammaRamp( NativeMethods.GetDC( IntPtr.Zero ), ref ramp );
			}
		}

		public static async Task<Erg> TryConvertToERG( this Document document, CancellationToken token ) =>
			await Task.Run( () => {
				var erg = new Erg();

				//TODO recalc the checksums
				//load file, checking checksums along the way.. (skip frames/lines with bad checksums?)
				//erg.TryAdd( document, Span.Zero,
				return erg;
			}, token );

		public static async Task<Boolean> TrySave( this Erg erg, Document document, CancellationToken token ) =>
			await Task.Run( () => {

				//TODO recalc the checksums
				//write out to file
				// ReSharper disable once ConvertToLambdaExpression
				return false;
			}, token );

		public static async Task<Boolean> TrySave( this Efv efv, Document document, CancellationToken token ) =>
			await Task.Run( () => {

				//TODO recalc the checksums
				//write out to file
				// ReSharper disable once ConvertToLambdaExpression
				var bob = new BinaryFormatter();

				//bob.Serialize(
				return false;
			}, token );

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
		public struct RAMP {

			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public UInt16[] Red;

			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public UInt16[] Green;

			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public UInt16[] Blue;
		}
	}
}