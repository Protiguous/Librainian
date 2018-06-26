// Copyright � Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "MilliElectronVolts.cs" belongs to Protiguous@Protiguous.com
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
// File "MilliElectronVolts.cs" was last formatted by Protiguous on 2018/06/26 at 1:25 AM.

namespace Librainian.Measurement.Physics {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using JetBrains.Annotations;
	using Librainian.Extensions;

	/// <summary>Units of mass and energy in Thousandths of <see cref="ElectronVolts" />.</summary>
	/// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
	/// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
	/// <seealso cref="http://wikipedia.org/wiki/Milli-" />
	/// <seealso cref="http://wikipedia.org/wiki/Electronvolt" />
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public struct MilliElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {

		public const Decimal InOneElectronVolt = 1E3m;

		public const Decimal InOneGigaElectronVolt = 1E12m;

		public const Decimal InOneKiloElectronVolt = 1E6m;

		public const Decimal InOneMegaElectronVolt = 1E9m;

		public const Decimal InOneMilliElectronVolt = 1E0m;

		public const Decimal InOneTeraElectronVolt = 1E15m;

		/// <summary>About 79228162514264337593543950335.</summary>
		public static readonly MilliElectronVolts MaxValue = new MilliElectronVolts( Decimal.MaxValue );

		/// <summary>About -79228162514264337593543950335.</summary>
		public static readonly MilliElectronVolts MinValue = new MilliElectronVolts( Decimal.MinValue );

		public static readonly MilliElectronVolts Zero = new MilliElectronVolts( 0 );

		public readonly Decimal Value;

		public MilliElectronVolts( Decimal units ) : this() => this.Value = units;

		public static implicit operator ElectronVolts( MilliElectronVolts milliElectronVolts ) => milliElectronVolts.ToElectronVolts();

		public static Boolean operator <( MilliElectronVolts left, MilliElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

		public static Boolean operator >( MilliElectronVolts left, MilliElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

		public Int32 CompareTo( ElectronVolts other ) => this.ToElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.Value );

		public Int32 CompareTo( KiloElectronVolts other ) => this.ToKiloElectronVolts().Value.CompareTo( other.Value );

		[NotNull]
		public String Simpler() {
			var list = new HashSet<String> {
				this.ToTeraElectronVolts().ToString(),
				this.ToGigaElectronVolts().ToString(),
				this.ToMegaElectronVolts().ToString(),
				this.ToElectronVolts().ToString(),
				this.ToMilliElectronVolts().ToString()
			};

			return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
		}

		public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * InOneElectronVolt );

		public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

		public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

		public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

		public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

		public override String ToString() => $"{this.Value} meV";

		public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );
	}
}