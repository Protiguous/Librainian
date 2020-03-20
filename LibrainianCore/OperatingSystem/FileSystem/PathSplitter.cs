﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PathSplitter.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "PathSplitter.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Collections.Extensions;
    using JetBrains.Annotations;
    using Parsing;

    //using LanguageExt.Prelude;

    public class PathSplitter {

        [NotNull]
        [ItemNotNull]
        private List<String> Parts { get; } = new List<String>();

        [NotNull]
        public String FileName { get; }

        /// <summary>Null when equal to (is) the root folder.</summary>
        [CanBeNull]
        public String OriginalPath { get; }

        public PathSplitter( [NotNull] Folder folder ) : this( document: new Document( fullPath: folder.FullPath ), newExtension: default ) { }

        public PathSplitter( [NotNull] IDocument document, String newExtension = default ) {
            if ( document == null ) {
                throw new ArgumentNullException( paramName: nameof( document ) );
            }

            newExtension = newExtension.Trimmed() ?? document.Extension();

            if ( !newExtension.StartsWith( value: "." ) ) {
                newExtension = $".{newExtension}";
            }

            this.FileName = $"{document.JustName()}{newExtension}";

            this.OriginalPath = Path.GetDirectoryName( path: document.FullPath );

            this.Parts.Clear();

            if ( !String.IsNullOrEmpty( value: this.OriginalPath ) ) {
                this.Parts.AddRange( collection: Split( path: this.OriginalPath ) );
            }

            this.Parts.TrimExcess();
        }

        [NotNull]
        private static IEnumerable<String> Split( [NotNull] String path ) {
            if ( String.IsNullOrWhiteSpace( value: path ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( path ) );
            }

            return path.Split( separator: new[] {
                Path.DirectorySeparatorChar
            }, options: StringSplitOptions.RemoveEmptyEntries );
        }

        public Boolean AddSubFolder( [NotNull] String subfolder ) {
            subfolder = Folder.CleanPath( fullpath: subfolder.Trim() );

            if ( String.IsNullOrWhiteSpace( value: subfolder ) ) {
                return default;
            }

            this.Parts.Add( item: subfolder );

            return true;
        }

        //[DebuggerStepThrough]
        public Boolean InsertRoot( [NotNull] Folder path ) {
            if ( path is null ) {
                throw new ArgumentNullException( paramName: nameof( path ) );
            }

            this.Parts.Insert( index: 1, item: path.FullPath );

            if ( path.FullPath[ index: 1 ] == ':' ) {
                this.Parts.RemoveAt( index: 0 ); //inserting a drive:\folder? remove the original drive:\folder part
            }

            return true;
        }

        /// <summary>Returns the reconstructed path and filename.</summary>
        /// <returns></returns>
        [NotNull]
        [DebuggerStepThrough]
        public Document Recombined() {
            var folder = new Folder( fullPath: this.Parts.ToStrings( c: Path.DirectorySeparatorChar ) );

            return new Document( folder: folder, filename: this.FileName );
        }

        /// <summary>Replace the original path, with <paramref name="replacement" /> path, not changing the filename.</summary>
        /// <param name="replacement"></param>
        /// <returns></returns>

        //[DebuggerStepThrough]
        public Boolean ReplacePath( [NotNull] IFolder replacement ) {
            if ( replacement == null ) {
                throw new ArgumentNullException( paramName: nameof( replacement ) );
            }

            this.Parts.Clear();
            this.Parts.AddRange( collection: Split( path: replacement.FullPath ) );
            this.Parts.TrimExcess();

            return true;
        }

        [DebuggerStepThrough]
        public Boolean SubstituteDrive( Char d ) {
            var s = this.Parts[ index: 0 ] ?? String.Empty;

            if ( s.Length != 2 || !s.EndsWith( value: ":", comparisonType: StringComparison.Ordinal ) ) {
                return default;
            }

            this.Parts[ index: 0 ] = $"{d}:";

            return true;
        }

    }

}