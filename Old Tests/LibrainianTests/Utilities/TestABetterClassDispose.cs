﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TestABetterClassDispose.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "LibrainianTests", "TestABetterClassDispose.cs" was last formatted by Protiguous on 2019/11/15 at 1:38 PM.

namespace LibrainianTests.Utilities {

    using System;
    using System.Diagnostics;
    using Librainian.Utilities;
    using Xunit;

    public class TestABetterClassDispose {

        [Fact]
        public void TestMethod1() {
            using var test1 = new Test( 1 );
            using var test2 = new Test( 2 );
            using var test3 = new Test( 3 );
            using var test4 = test3;
            using var test = test1;
        }

        internal class Test : ABetterClassDispose {

            private readonly Byte value;

            public Test( Byte val ) => this.value = val;

            /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
            public override void DisposeManaged() => Debug.WriteLine( this.value );

        }
    }
}