﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FolderBag.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "FolderBag.cs" was last formatted by Protiguous on 2018/07/10 at 8:54 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para>A bag of folders, stored somewhat efficiently ?memory-wise> than a list.</para>
	/// </summary>
	[JsonObject]
	public partial class FolderBag : IEnumerable<Folder> {

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///     An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Folder> GetEnumerator() {
			foreach ( var ending in this.Endings ) {
				var node = ending;
				var path = String.Empty;

				while ( node.Parent != null ) {
					path = $"{Path.DirectorySeparatorChar}{node.Data}{path}";
					node = node.Parent;
				}

				this.Roots.Should().Contain( node );
				path = String.Concat( node.Data, path );

				yield return new Folder( path );
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		[JsonProperty]
		public List<Node> Endings { get; } = new List<Node>();

		[JsonProperty]
		public List<Node> Roots { get; } = new List<Node>();

		public Boolean Add( [CanBeNull] String folderpath ) {
			if ( null == folderpath ) { return false; }

			this.FoundAnotherFolder( new Folder( folderpath ) );

			return true;
		}

		public UInt64 AddRange( [CanBeNull] IEnumerable<String> folderpaths ) {
			var counter = 0UL;

			if ( null == folderpaths ) { return counter; }

			foreach ( var folderpath in folderpaths ) {
				this.FoundAnotherFolder( new Folder( folderpath ) );
				counter++;
			}

			return counter;
		}

		public void FoundAnotherFolder( [NotNull] Folder folder ) {
			if ( folder is null ) { throw new ArgumentNullException( nameof( folder ) ); }

			var pathParts = folder.Info.SplitPath().ToList();

			if ( !pathParts.Any() ) { return; }

			var currentNode = new Node( pathParts[ 0 ] );

			var existingNode = this.Roots.Find( node => Node.Equals( node, currentNode ) ); // look for an existing root node

			if ( !Node.Equals( existingNode, default ) ) {

				// use existing node
				currentNode = existingNode;
			}
			else {

				// didn't find one, add it
				this.Roots.Add( currentNode );
			}

			// ReSharper disable once LoopCanBePartlyConvertedToQuery
			foreach ( var pathPart in pathParts.Skip( 1 ) ) {
				var nextNode = new Node( pathPart, currentNode );
				existingNode = currentNode.SubFolders.Find( node => Node.Equals( node, nextNode ) );

				if ( !Node.Equals( existingNode, default ) ) {
					nextNode = existingNode; // already there? don't need to add it.
				}
				else {
					currentNode.SubFolders.Add( nextNode ); // didn't find one, add it
				}

				currentNode = nextNode;
			}

			currentNode.IsEmpty.Should().BeTrue();

			if ( !currentNode.Data.EndsWith( ":" ) ) { currentNode.Parent.Should().NotBeNull(); }

			this.Endings.Add( currentNode );
		}
	}
}