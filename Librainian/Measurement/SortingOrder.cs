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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous/".
// 
// File "SortingOrder.cs" last formatted on 2022-06-30 at 4:57 AM by Protiguous.

namespace Librainian.Measurement;

/// <summary>
///     English values for use in <see cref="IComparable{T}.CompareTo" /> methods.
/// </summary>
public static class CompareOrder {

    /// <summary>
    ///     This is <see cref="After" /> in a CompareTo operation.
    /// </summary>
    public const Int32 After = 1;

    /// <summary>
    ///     This is <see cref="Before" /> in a CompareTo operation.
    /// </summary>
    public const Int32 Before = -1;

    /// <summary>
    ///     Default to <see cref="NullsFirst" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsDefault = NullsFirst;

    /// <summary>
    ///     Have nulls sort <see cref="Before" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsFirst = Before;

    /// <summary>
    ///     Have nulls sort <see cref="After" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsLast = After;

    /// <summary>
    ///     This is the <see cref="Same" /> in a CompareTo operation.
    /// </summary>
    public const Int32 Same = 0;

}

/// <summary>
///     English values for use in <see cref="IComparable{T}.CompareTo" /> methods.
/// </summary>
public static class CompareToOrder {

    /// <summary>
    ///     This is <see cref="After" /> in a CompareTo operation.
    /// </summary>
    public const Int32 After = 1;

    /// <summary>
    ///     This is <see cref="Before" /> in a CompareTo operation.
    /// </summary>
    public const Int32 Before = -1;

    /// <summary>
    ///     Default to <see cref="NullsFirst" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsDefault = NullsFirst;

    /// <summary>
    ///     Have nulls sort <see cref="Before" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsFirst = Before;

    /// <summary>
    ///     Have nulls sort <see cref="After" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsLast = After;

    /// <summary>
    ///     This is the <see cref="Same" /> in a CompareTo operation.
    /// </summary>
    public const Int32 Same = 0;

}

/// <summary>
///     English values for use in <see cref="IComparable{T}.CompareTo" /> methods.
/// </summary>
public static class SortingOrder {

    /// <summary>
    ///     This is <see cref="After" /> in a CompareTo operation.
    /// </summary>
    public const Int32 After = 1;

    /// <summary>
    ///     This is <see cref="Before" /> in a CompareTo operation.
    /// </summary>
    public const Int32 Before = -1;

    /// <summary>
    ///     Default to <see cref="NullsFirst" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsDefault = NullsFirst;

    /// <summary>
    ///     Have nulls sort <see cref="Before" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsFirst = Before;

    /// <summary>
    ///     Have nulls sort <see cref="After" /> in a CompareTo operation.
    /// </summary>
    public const Int32 NullsLast = After;

    /// <summary>
    ///     This is the <see cref="Same" /> in a CompareTo operation.
    /// </summary>
    public const Int32 Same = 0;

}