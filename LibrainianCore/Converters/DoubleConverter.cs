﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DoubleConverter.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "DoubleConverter.cs" was last formatted by Protiguous on 2020/03/16 at 10:08 PM.

namespace Librainian.Converters {

    using System;
    using JetBrains.Annotations;
    using Maths;

    /// <summary>A class to allow the conversion of doubles to String representations of their exact System.Decimal values. The implementation aims for readability over efficiency.</summary>
    /// <see cref="http://yoda.arachsys.com/csharp/DoubleConverter.cs" />
    public static class DoubleConverter {

        /// <summary>Converts the given Double to a String representation of its exact System.Decimal value.</summary>
        /// <param name="d">The Double to convert.</param>
        /// <returns>A String representation of the Double's exact System.Decimal value.</returns>
        [NotNull]
        public static String ToExactString( this Double d ) {
            if ( Double.IsPositiveInfinity( d: d ) ) {
                return "+Infinity";
            }

            if ( Double.IsNegativeInfinity( d: d ) ) {
                return "-Infinity";
            }

            if ( Double.IsNaN( d: d ) ) {
                return "NaN";
            }

            // Translate the Double into sign, exponent and mantissa.
            var bits = BitConverter.DoubleToInt64Bits( value: d );

            // Note that the shift is sign-extended, hence the test against -1 not 1
            var negative = bits < 0;
            var exponent = ( Int32 ) ( ( bits >> 52 ) & 0x7ffL );
            var mantissa = bits & 0xfffffffffffffL;

            // Subnormal numbers; exponent is effectively one higher, but there's no extra
            // normalisation bit in the mantissa
            if ( exponent == 0 ) {
                exponent++;
            }

            // Normal numbers; leave exponent as it is but add extra bit to the front of the mantissa
            else {
                mantissa |= 1L << 52;
            }

            // Bias the exponent. It's actually biased by 1023, but we're treating the mantissa as
            // m.0 rather than 0.m, so we need to subtract another 52 from it.
            exponent -= 1075;

            if ( mantissa == 0 ) {
                return "0";
            }

            /* Normalize */
            while ( ( mantissa & 1 ) == 0 ) {
                /*  i.e., Mantissa is even */
                mantissa >>= 1;
                exponent++;
            }

            // Construct a new System.Decimal expansion with the mantissa
            var ad = new ArbitraryDecimal( x: mantissa );

            // If the exponent is less than 0, we need to repeatedly divide by 2 - which is the
            // equivalent of multiplying by 5 and dividing by 10.
            if ( exponent < 0 ) {
                for ( var i = 0; i < -exponent; i++ ) {
                    ad.MultiplyBy( amount: 5 );
                }

                ad.Shift( amount: -exponent );
            }

            // Otherwise, we need to repeatedly multiply by 2
            else {
                for ( var i = 0; i < exponent; i++ ) {
                    ad.MultiplyBy( amount: 2 );
                }
            }

            // Finally, return the String with an appropriate sign
            if ( negative ) {
                return "-" + ad;
            }

            return ad.ToString();
        }

    }

}