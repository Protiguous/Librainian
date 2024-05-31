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
// File "TimeSpanExtensions.cs" last formatted on 2022-02-17 at 11:22 AM by Protiguous.

namespace Librainian.Measurement.Time;

using System;

public static class TimeSpanExtensions {

    /// <summary>
    ///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="scalar">  </param>
    public static TimeSpan Divide( this TimeSpan timeSpan, Double scalar ) => TimeSpan.FromTicks( ( Int64 ) ( timeSpan.Ticks / scalar ) );

    /// <summary>
    ///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="scalar">  </param>
    public static TimeSpan Divide( this TimeSpan timeSpan, Int64 scalar ) => TimeSpan.FromTicks( timeSpan.Ticks / scalar );

    /// <summary>
    ///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="scalar">  </param>
    public static TimeSpan Divide( this TimeSpan timeSpan, Decimal scalar ) => TimeSpan.FromTicks( ( Int64 ) ( timeSpan.Ticks / scalar ) );

    /// <summary>
    ///     Example: Console.WriteLine( 3.Days().FromNow() );
    /// </summary>
    /// <param name="timeSpan"></param>
    public static DateTimeOffset FromNow( this TimeSpan timeSpan ) => DateTimeOffset.UtcNow.Add( timeSpan );

    /// <summary>
    ///     Multiplies a timespan by an integer value
    /// </summary>
    /// <param name="multiplicand"></param>
    /// <param name="multiplier"></param>
    public static TimeSpan Multiply( this TimeSpan multiplicand, Int64 multiplier ) => TimeSpan.FromTicks( multiplicand.Ticks * multiplier );

    /// <summary>
    ///     Multiplies a timespan by a double value
    /// </summary>
    /// <param name="multiplicand"></param>
    /// <param name="multiplier"></param>
    public static TimeSpan Multiply( this TimeSpan multiplicand, Double multiplier ) => TimeSpan.FromTicks( ( Int64 ) ( multiplicand.Ticks * multiplier ) );

    /// <summary>
    ///     Multiplies a timespan by a decimal value
    /// </summary>
    /// <param name="multiplicand"></param>
    /// <param name="multiplier"></param>
    public static TimeSpan Multiply( this TimeSpan multiplicand, Decimal multiplier ) => TimeSpan.FromTicks( ( Int64 ) ( multiplicand.Ticks * multiplier ) );

    /// <summary>
    ///     Multiplies a timespan by an integer value
    /// </summary>
    /// <param name="multiplicand"></param>
    /// <param name="multiplier"></param>
    public static TimeSpan Multiply( this TimeSpan multiplicand, Int32 multiplier ) => TimeSpan.FromTicks( multiplicand.Ticks * multiplier );

}