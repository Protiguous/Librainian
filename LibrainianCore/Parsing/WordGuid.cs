﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "WordGuid.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "WordGuid.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Parsing {

    /*
        [Obsolete]
        public class WordGuid {
            private readonly Action< String > Info = obj => obj.WriteLine();

            private readonly WordToGuidAndGuidToWord WGGW;

            public WordGuid( Action< String > info ) {
                this.Info = null;
                if ( null != info ) {
                    this.Info += info;
                }
                this.WGGW = new WordToGuidAndGuidToWord( "WordsGuids", "xml" );
            }

            public Boolean Load() {
                this.Info( "Loading word guid database." );
                return this.WGGW.Load();
            }

            public Boolean Save() {
                this.Info( "Saving word guid database." );
                return this.WGGW.Save();
            }

            public int Count => this.WGGW.Count;

            public void Erase() {
                try {
                    this.WGGW.Clear();
                    this.Info( "All words in the word guid database have been erased." );
                }
                catch ( Exception exception ) {
                    exception.More( );
                }
            }

            /// <summary>Adds and returns the guid for the word.</summary>
            /// <param name="word"></param>
            /// <returns></returns>
            public Guid Get( String word ) {
                try {
                    if ( String.IsNullOrEmpty( word ) ) {
                        return Guid.Empty;
                    }

                    if ( !this.WGGW.Contains( word ) ) {
                        this.WGGW[ word ] = Guid.NewGuid();
                        String.Format( "Word {0} has guid {1}.", word, this.WGGW[ word ] ).WriteLine();
                    }

                    return this.WGGW[ word ];
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
                return Guid.Empty;
            }

            /// <summary>Returns the word for this guid or String.Empty;</summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            public String Get( Guid guid ) {
                try {
                    if ( Guid.Empty == guid ) {
                        return String.Empty;
                    }
                    return !this.WGGW.Contains( guid ) ? String.Empty : this.WGGW[ guid ];
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
                return String.Empty;
            }
        }
    */

}