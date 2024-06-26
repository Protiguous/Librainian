﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "PathSplitter.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.


namespace Librainian.FileSystem;

using Exceptions;
using Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

//using LanguageExt.Prelude;

public class PathSplitter {
	private List<String> Parts { get; } = new();

	private static IEnumerable<String> Split( String path ) {
		if ( String.IsNullOrWhiteSpace( path ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( path ) );
		}

		return path.Split( Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries );
	}

	public PathSplitter( IFolder folder ) : this( new DocumentFile( folder.FullPath ) ) {
	}

	public PathSplitter( IDocumentFile documentFile, String newExtension = default ) {
		if ( documentFile == null ) {
			throw new ArgumentEmptyException( nameof( documentFile ) );
		}

		newExtension = newExtension.Trimmed() ?? documentFile.Extension();

		if ( !newExtension.StartsWith( "." ) ) {
			newExtension = $".{newExtension}";
		}

		this.FileName = $"{documentFile.JustName()}{newExtension}";

		this.OriginalPath = Path.GetDirectoryName( documentFile.FullPath );

		this.Parts.Clear();

		if ( !String.IsNullOrEmpty( this.OriginalPath ) ) {
			this.Parts.AddRange( Split( this.OriginalPath ) );
		}

		this.Parts.TrimExcess();
	}

	public String FileName { get; }

	/// <summary>Null when equal to (is) the root folder.</summary>
	public String? OriginalPath { get; }

	public Boolean AddSubFolder( String subfolder ) {
		subfolder = Folder.CleanPath( subfolder.Trim() );

		if ( String.IsNullOrWhiteSpace( subfolder ) ) {
			return false;
		}

		this.Parts.Add( subfolder );

		return true;
	}

	//[DebuggerStepThrough]
	public Boolean InsertRoot( Folder path ) {
		if ( path is null ) {
			throw new ArgumentEmptyException( nameof( path ) );
		}

		this.Parts.Insert( 1, path.FullPath );

		if ( path.FullPath[1] == ':' ) {
			this.Parts.RemoveAt( 0 ); //inserting a drive:\folder? remove the original drive:\folder part
		}

		return true;
	}

	/// <summary>Returns the reconstructed path and filename.</summary>
	[DebuggerStepThrough]
	public DocumentFile Recombined() {
		var folder = new Folder( this.Parts.ToStrings( Path.DirectorySeparatorChar ) );

		return new DocumentFile( folder, this.FileName );
	}

	/// <summary>Replace the original path, with <paramref name="replacement" /> path, not changing the filename.</summary>
	/// <param name="replacement"></param>

	//[DebuggerStepThrough]
	public Boolean ReplacePath( IFolder replacement ) {
		this.Parts.Clear();
		this.Parts.AddRange( Split( replacement.FullPath ) );
		this.Parts.TrimExcess();

		return true;
	}

	[DebuggerStepThrough]
	public Boolean SubstituteDrive( Char d ) {
		var s = this.Parts[0];

		if ( ( s.Length != 2 ) || !s.EndsWith( ":", StringComparison.Ordinal ) ) {
			return false;
		}

		this.Parts[0] = $"{d}:";

		return true;
	}
}