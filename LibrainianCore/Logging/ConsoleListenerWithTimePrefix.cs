// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConsoleListenerWithTimePrefix.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "ConsoleListenerWithTimePrefix.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Logging {

    using System;
    using System.Diagnostics;

    public class ConsoleListenerWithTimePrefix : ConsoleTraceListener {

        /// <summary>Gets a value indicating whether the trace listener is thread safe.</summary>
        /// <returns>true if the trace listener is thread safe; otherwise, false. The default is false.</returns>
        public override Boolean IsThreadSafe => true;

        public ConsoleListenerWithTimePrefix() : base( useErrorStream: true ) { }

        //TODO  http://msdn.microsoft.com/en-us/Library/system.diagnostics.consoletracelistener(v=vs.110).aspx
        /// <summary>Emits an error message and a detailed error message to the listener you create when you implement the <see cref="TraceListener" /> class.</summary>
        /// <param name="message">      A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public override void Fail( String message, String detailMessage ) {
            base.Fail( message: message, detailMessage: detailMessage );
            this.Flush();
        }

        /// <summary>Writes a message to this instance's <see cref="TextWriterTraceListener.Writer" />.</summary>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DebuggerStepThrough]
        public override void Write( String message ) {
            Console.Write( value: message );
            this.Flush();
        }

        /// <summary>
        /// Writes a message to this instance's <see cref="TextWriterTraceListener.Writer" /> followed by a line terminator. The default line terminator is a carriage return followed
        /// by a line feed (\r\n).
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        [DebuggerStepThrough]
        public override void WriteLine( String message ) {
            Console.WriteLine( value: message );
            this.Flush();
        }

        //private static String HeaderTimeThread() => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} ({Thread.CurrentThread.ManagedThreadId})] ";
    }
}