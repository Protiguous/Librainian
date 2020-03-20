// Copyright � 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Yoctoseconds.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Yoctoseconds.cs" was last formatted by Protiguous on 2020/03/16 at 3:08 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    /// <summary></summary>
    /// <see cref="http://wikipedia.org/wiki/Yoctosecond" />
    [JsonObject]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Yoctoseconds : IComparable<Yoctoseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneZeptosecond = 1000;

        /// <summary><see cref="Five" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Five = new Yoctoseconds( value: 5 );

        /// <summary><see cref="One" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds One = new Yoctoseconds( value: 1 );

        /// <summary><see cref="Seven" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Seven = new Yoctoseconds( value: 7 );

        /// <summary><see cref="Ten" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Ten = new Yoctoseconds( value: 10 );

        /// <summary><see cref="Thirteen" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Thirteen = new Yoctoseconds( value: 13 );

        /// <summary><see cref="Thirty" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Thirty = new Yoctoseconds( value: 30 );

        /// <summary><see cref="Three" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Three = new Yoctoseconds( value: 3 );

        /// <summary><see cref="Two" /><see cref="Yoctoseconds" />.</summary>
        public static Yoctoseconds Two = new Yoctoseconds( value: 2 );

        /// <summary></summary>
        public static Yoctoseconds Zero = new Yoctoseconds( value: 0 );

        public static Rational InOneSecond { get; } = new BigInteger( value: 10E24 );

        [JsonProperty]
        public Rational Value { get; }

        public Yoctoseconds( Decimal value ) => this.Value = ( Rational ) value;

        public Yoctoseconds( Rational value ) => this.Value = value;

        public Yoctoseconds( Int64 value ) => this.Value = value;

        public Yoctoseconds( BigInteger value ) => this.Value = value;

        public static Yoctoseconds Combine( Yoctoseconds left, Yoctoseconds right ) => Combine( left: left, yoctoseconds: right.Value );

        public static Yoctoseconds Combine( Yoctoseconds left, Rational yoctoseconds ) => new Yoctoseconds( value: left.Value + yoctoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Yoctoseconds left, Yoctoseconds right ) => left.Value == right.Value;

        /// <summary>Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="PlanckTimes" />.</summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator PlanckTimes( Yoctoseconds yoctoseconds ) => ToPlanckTimes( yoctoseconds: yoctoseconds );

        public static implicit operator SpanOfTime( Yoctoseconds yoctoseconds ) => new SpanOfTime( planckTimes: yoctoseconds );

        /// <summary>Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="Zeptoseconds" />.</summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator Zeptoseconds( Yoctoseconds yoctoseconds ) => yoctoseconds.ToZeptoseconds();

        public static Yoctoseconds operator -( Yoctoseconds yoctoseconds ) => new Yoctoseconds( value: yoctoseconds.Value * -1 );

        public static Yoctoseconds operator -( Yoctoseconds left, Yoctoseconds right ) => Combine( left: left, right: -right );

        public static Yoctoseconds operator -( Yoctoseconds left, Decimal seconds ) => Combine( left: left, yoctoseconds: ( Rational ) ( -seconds ) );

        public static Boolean operator !=( Yoctoseconds left, Yoctoseconds right ) => !Equals( left: left, right: right );

        public static Yoctoseconds operator +( Yoctoseconds left, Yoctoseconds right ) => Combine( left: left, right: right );

        public static Yoctoseconds operator +( Yoctoseconds left, Decimal yoctoseconds ) => Combine( left: left, yoctoseconds: ( Rational ) yoctoseconds );

        public static Boolean operator <( Yoctoseconds left, Yoctoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Yoctoseconds left, Yoctoseconds right ) => Equals( left: left, right: right );

        public static Boolean operator >( Yoctoseconds left, Yoctoseconds right ) => left.Value > right.Value;

        public static PlanckTimes ToPlanckTimes( Yoctoseconds yoctoseconds ) => new PlanckTimes( value: yoctoseconds.Value * ( Rational ) PlanckTimes.InOneYoctosecond );

        public Int32 CompareTo( Yoctoseconds other ) => this.Value.CompareTo( other: other.Value );

        public Boolean Equals( Yoctoseconds other ) => Equals( left: this, right: other );

        public override Boolean Equals( [CanBeNull] Object? obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Yoctoseconds yoctoseconds && this.Equals( other: yoctoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: this.Value * ( Rational ) PlanckTimes.InOneYoctosecond );

        public Seconds ToSeconds() => new Seconds( value: this.Value * InOneSecond );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "ys" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( singular: "ys" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( value: this.Value / InOneZeptosecond );

    }

}