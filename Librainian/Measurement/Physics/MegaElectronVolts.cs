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
// File "MegaElectronVolts.cs" last formatted on 2022-04-01 at 6:51 AM by Protiguous.

namespace Librainian.Measurement.Physics;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ExtendedNumerics;
using Extensions;

/// <summary>Units of mass and energy in ElectronVolts.</summary>
/// <param name="Value"></param>
/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
/// <see cref="http://wikipedia.org/wiki/Mega-" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public readonly record struct MegaElectronVolts( BigDecimal Value ) : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>,
	IComparable<GigaElectronVolts> {
	[SuppressMessage( "Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "<Pending>" )]
	internal class InOne {
		public static readonly BigDecimal InOneElectronVolt = ( BigDecimal )1E-6m;

		public static readonly BigDecimal InOneGigaElectronVolt = ( BigDecimal )1E3m;

		public static readonly BigDecimal InOneKiloElectronVolt = ( BigDecimal )1E-3m;

		public static readonly BigDecimal InOneMegaElectronVolt = ( BigDecimal )1E0m;

		public static readonly BigDecimal InOneMilliElectronVolt = ( BigDecimal )1E-9m;

		public static readonly BigDecimal InOneTeraElectronVolt = ( BigDecimal )1E6m;
	}

	public MegaElectronVolts( Decimal units ) : this( ( BigDecimal )units ) { }

	public MegaElectronVolts( Double units ) : this( ( BigDecimal )units ) { }

	public MegaElectronVolts( ElectronVolts electronVolts ) : this( electronVolts.ToMegaElectronVolts().Value ) { }

	public MegaElectronVolts( GigaElectronVolts gigaElectronVolts ) : this( gigaElectronVolts.ToMegaElectronVolts().Value ) { }

	public MegaElectronVolts( KiloElectronVolts kiloElectronVolts ) : this( kiloElectronVolts.ToMegaElectronVolts().Value ) { }

	public MegaElectronVolts( TeraElectronVolts teraElectronVolts ) : this( teraElectronVolts.ToMegaElectronVolts().Value ) { }

	public static MegaElectronVolts One => new( Decimal.One );

	public static MegaElectronVolts Zero => new( Decimal.Zero );

	public Int32 CompareTo( ElectronVolts other ) => this.ToElectronVolts().CompareTo( other );

	public Int32 CompareTo( GigaElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToElectronVolts().CompareTo( other );

	public Int32 CompareTo( MegaElectronVolts other ) => this.ToElectronVolts().CompareTo( other.ToElectronVolts() );

	public Int32 CompareTo( MilliElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToElectronVolts().CompareTo( other );

	public Int32 CompareTo( ElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToElectronVolts().CompareTo( other );

	public Int32 CompareTo( MegaElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToElectronVolts().CompareTo( other );

	public static MegaElectronVolts operator +( MegaElectronVolts left, MegaElectronVolts right ) => new( left.Value + right.Value );

	public static GigaElectronVolts operator +( MegaElectronVolts megaElectronVolts, GigaElectronVolts gigaElectronVolts ) =>
		megaElectronVolts.ToGigaElectronVolts() + gigaElectronVolts;

	public static Boolean operator <( MegaElectronVolts left, MegaElectronVolts right ) => left.Value < right.Value;

	public static Boolean operator >( MegaElectronVolts left, MegaElectronVolts right ) => left.Value > right.Value;

	public ElectronVolts ToElectronVolts() => new( this.Value * InOne.InOneElectronVolt );

	public GigaElectronVolts ToGigaElectronVolts() => new( this.Value * InOne.InOneGigaElectronVolt );

	public KiloElectronVolts ToKiloElectronVolts() => new( this.Value * InOne.InOneKiloElectronVolt );

	public MegaElectronVolts ToMegaElectronVolts() => new( this.Value * InOne.InOneMegaElectronVolt );

	public MilliElectronVolts ToMilliElectronVolts() => new( this.Value * InOne.InOneMilliElectronVolt );

	public override String ToString() => $"{this.Value} MeV";

	public TeraElectronVolts ToTeraElectronVolts() => new( this.Value * InOne.InOneTeraElectronVolt );

	public static Boolean operator <=( MegaElectronVolts left, MegaElectronVolts right ) => left.Value <= right.Value;

	public static Boolean operator >=( MegaElectronVolts left, MegaElectronVolts right ) => left.Value >= right.Value;
}