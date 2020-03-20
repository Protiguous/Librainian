﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "FolderBag.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "FolderBag.cs" was last formatted by Protiguous on 2020/03/16 at 3:09 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    // ReSharper disable RedundantUsingDirective
    using Path = Pri.LongPath.Path;

    // ReSharper restore RedundantUsingDirective

    /// <summary>
    ///     <para>A bag of folders, stored somewhat efficiently ?memory-wise> than a list.</para>
    /// </summary>
    [JsonObject]
    public partial class FolderBag : IEnumerable<Folder> {

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<Folder> GetEnumerator() {
            foreach ( var ending in this.Endings ) {
                var node = ending;
                var path = String.Empty;

                while ( node.Parent != null ) {
                    path = $"{Path.DirectorySeparatorChar}{node.Data}{path}";
                    node = node.Parent;
                }

                //this.Roots.Should().Contain( node );
                path = String.Concat( str0: node.Data, str1: path );

                yield return new Folder( fullPath: path );
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [JsonProperty]
        public List<Node> Endings { get; } = new List<Node>();

        [JsonProperty]
        public List<Node> Roots { get; } = new List<Node>();

        public Boolean Add( [CanBeNull] String folderpath ) {
            if ( null == folderpath ) {
                return default;
            }

            this.FoundAnotherFolder( folder: new Folder( fullPath: folderpath ) );

            return true;
        }

        public UInt64 AddRange( [CanBeNull] IEnumerable<String> folderpaths ) {
            var counter = 0UL;

            if ( null == folderpaths ) {
                return counter;
            }

            foreach ( var folderpath in folderpaths ) {
                this.FoundAnotherFolder( folder: new Folder( fullPath: folderpath ) );
                counter++;
            }

            return counter;
        }

        public void FoundAnotherFolder( [NotNull] IFolder folder ) {
            if ( folder is null ) {
                throw new ArgumentNullException( paramName: nameof( folder ) );
            }

            var pathParts = folder.Info.SplitPath().ToList();

            if ( !pathParts.Any() ) {
                return;
            }

            var currentNode = new Node( data: pathParts[ index: 0 ] );

            var existingNode = this.Roots.Find( match: node => Node.Equals( left: node, right: currentNode ) ); // look for an existing root node

            if ( !Node.Equals( left: existingNode, right: default ) ) {

                // use existing node
                currentNode = existingNode;
            }
            else {

                // didn't find one, add it
                this.Roots.Add( item: currentNode );
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach ( var pathPart in pathParts.Skip( count: 1 ) ) {
                var nextNode = new Node( data: pathPart, parent: currentNode );
                existingNode = currentNode.SubFolders.Find( match: node => Node.Equals( left: node, right: nextNode ) );

                if ( !Node.Equals( left: existingNode, right: default ) ) {
                    nextNode = existingNode; // already there? don't need to add it.
                }
                else {
                    currentNode.SubFolders.Add( item: nextNode ); // didn't find one, add it
                }

                currentNode = nextNode;
            }

            //currentNode.IsEmpty.Should().BeTrue();

            //if ( !currentNode.Data.EndsWith( ":" ) ) { currentNode.Parent.Should().NotBeNull(); }

            this.Endings.Add( item: currentNode );
        }

    }

}