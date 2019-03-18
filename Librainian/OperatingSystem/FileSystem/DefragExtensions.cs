﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DefragExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "DefragExtensions.cs" was last formatted by Protiguous on 2018/07/10 at 8:53 PM.

namespace Librainian.OperatingSystem.FileSystem
{

    using System;
    using System.Diagnostics;
    using System.IO;
    using ComputerSystem.Devices;
    using JetBrains.Annotations;
    using OperatingSystem;

    public class DefragExtensions
    {

        /// <summary>
        ///     The function starts the Defrag.Exe and waits for it to finish. It ensures the process is
        ///     run with lower priority and the spawned process DfrgNtfs is given 'Idle' priority
        /// </summary>
        /// <param name="disk">Drive to defrag - format is "c:" for example</param>
        [NotNull]
        private static String Defrag(Disk disk)
        {
            var path = Path.Combine(Windows.WindowsSystem32Folder.Value.FullName, "defrag.exe");

            var info = new ProcessStartInfo
            {
                FileName = path,
                Arguments = String.Format("{{{0}}} /O /V /M " + Environment.ProcessorCount, disk),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            var defrag = Process.Start(info);

            if (defrag == null) { return String.Empty; }

            defrag.PriorityClass = ProcessPriorityClass.Idle;
            defrag.WaitForExit();

            return defrag.StandardOutput.ReadToEnd();
        }
    }
}