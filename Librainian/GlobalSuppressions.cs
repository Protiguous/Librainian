﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "GlobalSuppressions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "GlobalSuppressions.cs" was last formatted by Protiguous on 2020/03/16 at 9:39 PM.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage( category: "Globalization", checkId: "CA1303:Do not pass literals as localized parameters" )]
[assembly: SuppressMessage( category: "Globalization", checkId: "CA1305:Specify IFormatProvider" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1031:Do not catch general exception types" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1065:Do not raise exceptions in unexpected locations" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1036:Override methods on comparable types" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1040:Avoid empty interfaces" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1032:Implement standard exception constructors" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1062:Validate arguments of public methods" )]
[assembly: SuppressMessage( category: "Naming", checkId: "CA1710:Identifiers should have correct suffix" )]
[assembly: SuppressMessage( category: "Performance", checkId: "CA1819:Properties should not return arrays", Justification = "What a dumb rule for dumb programmers." )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1000:Do not declare static members on generic types", Justification = "Stupid rule." )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1008:Enums should have zero value" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1034:Nested types should not be visible", Justification = "Who comes up with some of these stupid 'rules'?" )]
[assembly: SuppressMessage( category: "Performance", checkId: "CA1814:Prefer jagged arrays over multidimensional" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1028:Enum Storage should be Int32", Justification = "Stupid rule. Not everything needs to be an Int32!" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1051:Do not declare visible instance fields", Justification = "Stupid rule for structs." )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1060:Move pinvokes to native methods class", Justification = "Another stupid rule." )]
[assembly:
    SuppressMessage( category: "Design", checkId: "CA1063:Implement IDisposable Correctly", Justification = "I am so fucking tired of stupid design rules. Analyze better!" )]
[assembly: SuppressMessage( category: "Interoperability", checkId: "CA1401:P/Invokes should not be visible", Justification = "Why?" )]
[assembly: SuppressMessage( category: "Performance", checkId: "HAA0301:Closure Allocation Source", Justification = "This is not a useful diagnostic." )]
[assembly: SuppressMessage( category: "Performance", checkId: "HAA0302:Display class allocation to capture closure", Justification = "?!?!" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1054:Uri parameters should not be strings", Justification = "Maybe. When I have time!" )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1066:Type {0} should implement IEquatable<T> because it overrides Equals", Justification = "No." )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1055:Uri return values should not be strings", Justification = "Maybe" )]
[assembly: SuppressMessage( category: "Performance", checkId: "HAA0401:Possible allocation of reference type enumerator", Justification = "Yada yada yada" )]
[assembly: SuppressMessage( category: "Redundancy", checkId: "RCS1036:Remove redundant empty line.", Justification = "What a stupid rule." )]
[assembly:
    SuppressMessage( category: "Performance", checkId: "RCS1080:Use 'Count/Length' property instead of 'Any' method.",
        Justification = "This is the opposite of a Performance boost." )]
[assembly: SuppressMessage( category: "Design", checkId: "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "Code smell." )]
[assembly: SuppressMessage( category: "Performance", checkId: "CA1815:Override equals and operator equals on value types", Justification = "Um, no." )]