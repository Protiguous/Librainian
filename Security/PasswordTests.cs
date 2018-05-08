﻿// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PasswordTests.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.Globalization;
    using Maths;
    using NUnit.Framework;
    using Parsing;

    [TestFixture]
    public static class PasswordTests {

        [Test]
        public static void TestAFew() {
            foreach ( var variable in 1.To( end: 25 ) ) {
                Console.WriteLine( value: PronounceablePasswordCreator.Generate( requiredLength: 3.Next( maxValue: 15 ) ).ToPascalCase( culture: CultureInfo.CurrentUICulture ) );
            }
        }
    }
}