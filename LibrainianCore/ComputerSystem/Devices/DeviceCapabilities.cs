// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "DeviceCapabilities.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "DeviceCapabilities.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.ComputerSystem.Devices {

    using System;

    /// <summary>Contains constants for determining devices capabilities. This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.</summary>
    [Flags]
    public enum DeviceCapabilities {

        Unknown = 0x00000000,

        // matches cfmgr32.h CM_DEVCAP_* definitions

        LockSupported = 0x00000001,

        EjectSupported = 0x00000002,

        Removable = 0x00000004,

        DockDevice = 0x00000008,

        UniqueId = 0x00000010,

        SilentInstall = 0x00000020,

        RawDeviceOk = 0x00000040,

        SurpriseRemovalOk = 0x00000080,

        HardwareDisabled = 0x00000100,

        NonDynamic = 0x00000200
    }
}