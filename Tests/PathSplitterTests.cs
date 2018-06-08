// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PathSplitterTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/LibrainianTests/PathSplitterTests.cs" was last formatted by Protiguous on 2018/05/24 at 7:37 PM.

namespace LibrainianTests {

    using System;
    using FluentAssertions;
    using Librainian.ComputerSystems.FileSystem;
    using NUnit.Framework;

    [TestFixture]
    public static class PathSplitterTests {

        [Test]
        public static void TestTheSplittingOrder() {
            const String example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";
            const String newExample = @"C:\recovered\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

            var bob = new PathSplitter( example );

            var reconstructed = bob.Recombined();

            reconstructed.FullPathWithFileName.Should().Be( example );

            bob.InsertRoot( @"C:\recovered" );
            reconstructed = bob.Recombined();

            reconstructed.FullPathWithFileName.Should().Be( newExample );

            Console.WriteLine( reconstructed.FullPathWithFileName );
        }
    }
}