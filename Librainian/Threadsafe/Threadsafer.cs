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
// File "Threadsafer.cs" last formatted on 2022-02-07 at 5:15 AM by Protiguous.

namespace Librainian.Threadsafe;

using System;
using System.Threading;
using Utilities;

/// <summary>
///     I don't think this is doing what I envision.. I want a FUNC()tion per thread. Each thread to return its own value.
/// </summary>
/// <typeparam name="TT"></typeparam>
/// <typeparam name="TF"></typeparam>
[NeedsTesting]
public static class Threadsafer<TT, TF> where TT : notnull {

    private static ThreadLocal<Func<TT, TF>> PerThreadCache { get; } = new();

#pragma warning disable CA1000 // Do not declare static members on generic types

    public static Func<TT, TF>? Get() => PerThreadCache.Value;

#pragma warning restore CA1000 // Do not declare static members on generic types

}