﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MasterDocumentTable.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "MasterDocumentTable.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Persistence;

    /// <summary>A persisted cache of all found <see cref="Document" />.</summary>
    public static class MasterDocumentTable {

        public static CancellationTokenSource CTS { get; } = new CancellationTokenSource();

        /// <summary>DocumentInfos[StringPath]=DocumentInfo</summary>
        public static PersistTable<String, DocumentInfo> DocumentInfos { get; } =
            new PersistTable<String, DocumentInfo>( specialFolder: Environment.SpecialFolder.CommonApplicationData, tableName: nameof( DocumentInfos ) );

        /// <summary>Documents[StringPath]=Document</summary>
        public static PersistTable<String, Document> Documents { get; } = new PersistTable<String, Document>( specialFolder: Environment.SpecialFolder.CommonApplicationData,
            subFolder: Path.GetFileNameWithoutExtension( path: Process.GetCurrentProcess().ProcessName ), tableName: nameof( Documents ) );

        public static StringKVPTable Settings { get; } = new StringKVPTable( specialFolder: Environment.SpecialFolder.LocalApplicationData,
            subFolder: Path.GetFileNameWithoutExtension( path: Process.GetCurrentProcess().ProcessName ), tableName: nameof( MasterDocumentTable ) );

        /// <summary>Start searching in the givin <paramref name="folder" />, and update our "MFT" with found documents.</summary>
        /// <param name="folder"></param>
        /// <param name="folders"></param>
        /// <param name="ReportDocument"></param>
        /// <returns></returns>
        [NotNull]
        public static async Task SearchAsync( [CanBeNull] IFolder folder, [CanBeNull] IProgress<IFolder> folders = null,
            [CanBeNull] IProgress<Document> ReportDocument = null ) {

            if ( CTS.IsCancellationRequested ) {
                return;
            }

            //Update the MFT
            foreach ( var document in folder.GetDocuments( searchPattern: "*.*" ) ) {
                if ( CTS.IsCancellationRequested ) {
                    return;
                }

                ReportDocument?.Report( value: document );
                Documents[ key: document.FullPath ] = document;
                DocumentInfos[ key: document.FullPath ] = new DocumentInfo( document: document );
                await DocumentInfos[ key: document.FullPath ].ScanAsync( token: CTS.Token ).ConfigureAwait( continueOnCapturedContext: false );
            }

            if ( CTS.IsCancellationRequested ) {
                return;
            }

            //And then scan down into any subfolders.
            foreach ( var subFolder in folder.BetterGetFolders( searchPattern: "*.*" ).TakeWhile( predicate: _ => !CTS.IsCancellationRequested ) ) {
                folders?.Report( value: subFolder );
                await SearchAsync( folder: subFolder, folders: folders, ReportDocument: ReportDocument ).ConfigureAwait( continueOnCapturedContext: false );
            }
        }

    }

}