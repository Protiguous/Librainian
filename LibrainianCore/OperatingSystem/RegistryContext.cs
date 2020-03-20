﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "RegistryContext.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "RegistryContext.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.OperatingSystem {

    using System;
    using JetBrains.Annotations;
    using Logging;
    using Magic;
    using Microsoft.Win32;

    public static class RegistryContext {

        /// <summary>
        ///     <para>Example <paramref name="menuName" />: "Copy Folder"</para>
        ///     <para>Example <paramref name="menuPath" />: "Folder\shell\Copy"</para>
        ///     <para>Example <paramref name="command" />: "command"</para>
        ///     <para>Example <paramref name="action" />: $"{Application.ExecutablePath} \"%1\""</para>
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="menuPath"></param>
        /// <param name="command"></param>
        /// <param name="action"></param>
        public static Boolean AddRegistryContext( [NotNull] String menuName, [NotNull] String menuPath, [CanBeNull] String command = null, [CanBeNull] String action = null ) {
            if ( String.IsNullOrWhiteSpace( value: menuPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( menuPath ) );
            }

            if ( String.IsNullOrWhiteSpace( value: menuName ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( menuName ) );
            }

            try {
                using var registryKey = Registry.ClassesRoot.CreateSubKey( subkey: menuPath );

                if ( registryKey != default ) {
                    registryKey.SetValue( name: String.Empty, value: menuName, valueKind: RegistryValueKind.String );

                    if ( String.IsNullOrEmpty( value: command ) ) {
                        command = "command";
                    }

                    action.ThrowIfNull();

                    if ( String.IsNullOrEmpty( value: action ) ) {

                        //action = $"{Application.ExecutablePath} \"%1\" \"%2\" \"%3\" \"%4\"";
                    }

                    using ( var commandKey = registryKey.CreateSubKey( subkey: command ) ) {

                        if ( commandKey != default ) {
                            commandKey.SetValue( name: String.Empty, value: action, valueKind: RegistryValueKind.String );
                        }
                        else {
                            return default;
                        }
                    }

                    return true;
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        public static void RemoveRegistryContext( [NotNull] String menuPath, [NotNull] String registryCommand ) {
            if ( String.IsNullOrWhiteSpace( value: menuPath ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( menuPath ) );
            }

            if ( String.IsNullOrWhiteSpace( value: registryCommand ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( registryCommand ) );
            }

            try {
                Registry.ClassesRoot.DeleteSubKey( subkey: registryCommand );
                Registry.ClassesRoot.DeleteSubKey( subkey: menuPath );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

    }

}