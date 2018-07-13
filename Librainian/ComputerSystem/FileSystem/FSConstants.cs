﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FSConstants.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "FSConstants.cs" was last formatted by Protiguous on 2018/07/10 at 8:54 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;

	/// <summary>
	///     constants lifted from winioctl.h from platform sdk
	/// </summary>
	internal class FSConstants {

		private const UInt32 FileAnyAccess = 0;

		private const UInt32 FileDeviceFileSystem = 0x00000009;

		private const UInt32 FileSpecialAccess = FileAnyAccess;

		private const UInt32 MethodBuffered = 0;

		private const UInt32 MethodNeither = 3;

		public static UInt32 FsctlGetRetrievalPointers = CTL_CODE( FileDeviceFileSystem, 28, MethodNeither, FileAnyAccess );

		public static UInt32 FsctlGetVolumeBitmap = CTL_CODE( FileDeviceFileSystem, 27, MethodNeither, FileAnyAccess );

		public static UInt32 FsctlMoveFile = CTL_CODE( FileDeviceFileSystem, 29, MethodBuffered, FileSpecialAccess );

		private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) => ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;
	}
}