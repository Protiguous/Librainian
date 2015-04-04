// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/PersistableSettings.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM

//namespace Librainian.Persistence {

//    using System;
//    using System.Diagnostics;
    
//    using System.IO;
//    using System.Windows.Forms;
//    using Controls;
//    using Extensions;
//    using IO;
//    using JetBrains.Annotations;
//    using Parsing;
//    using Properties;
//    using Threading;

////    public class PersistableSettings {

////        /// <summary>
////        /// Returns the <see cref="MainStoragePath"/> as a <see cref="DirectoryInfo"/>.
////        /// </summary>
////        [Obsolete]
////        public DirectoryInfo MainStoragePath {
////            get {
////                DirectoryInfo value;
////                return Types.Name( () => this.MainStoragePath ).TryGet( out value ) ? value : null;
////            }

////            set { value.TrySave( Types.Name( () => this.MainStoragePath ) ); }
////        }

////        /// <summary>
////        /// ask user for folder/network path where to store
////        /// </summary>
////        [UsedImplicitly]
////        public void AskUserForStorageFolder() {
////            var folderBrowserDialog = new FolderBrowserDialog {
////                ShowNewFolderButton = true,
////                Description = Resources._Please_direct_me_to_a_folder_,
////                RootFolder = Environment.SpecialFolder.MyComputer
////            };

////            var owner = WindowWrapper.CreateWindowWrapper( Process.GetCurrentProcess().MainWindowHandle );

////            var dialog = folderBrowserDialog.ShowDialog( owner );

////            if ( dialog != DialogResult.OK || folderBrowserDialog.SelectedPath.IsNullOrWhiteSpace() ) {
////                return;
////            }
////            this.MainStoragePath = new DirectoryInfo( folderBrowserDialog.SelectedPath );
////        }

////        public void Initialize() {
////            Log.Enter();
////            this.ValidateStorageFolder();
////            Log.Exit();
////        }

////        /// <summary>
////        /// check if we have a storage folder. if we don't, popup a dialog to ask. Settings.
////        /// </summary>
////        /// <returns></returns>
////        public void ValidateStorageFolder() {
////            try {

//////TODO recheck all this logic some other day
////Again:
////                if ( null == this.MainStoragePath ) {
////                    this.AskUserForStorageFolder();
////                    if ( null == this.MainStoragePath ) {
////                        goto Again;
////                    }
////                }

////                this.MainStoragePath.Refresh();
////                if ( !this.MainStoragePath.Exists ) {
////                    this.AskUserForStorageFolder();
////                }

////                if ( null == this.MainStoragePath ) {
////                    return;
////                }

////                if ( null == this.MainStoragePath.Ensure( requestReadAccess: true, requestWriteAccess: true ) ) {
////                    goto Again;
////                }

////                if ( !this.MainStoragePath.Exists ) {
////                    var dialogResult = MessageBox.Show( String.Format( "Unable to access storage folder [{0}]. Retry?", this.MainStoragePath.FullName ), "Folder Not Found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error );
////                    switch ( dialogResult ) {
////                        case DialogResult.Retry:
////                            goto Again;
////                        case DialogResult.Cancel:
////                            return;
////                    }
////                }

////                try {
////                    this.TestForReadWriteAccess();
////                }
////                catch ( Exception) {
////                    var dialogResult = MessageBox.Show( String.Format( "Unable to write to storage folder [{0}]. Retry?", this.MainStoragePath ), "No Access", MessageBoxButtons.RetryCancel );
////                    switch ( dialogResult ) {
////                        case DialogResult.Retry:
////                            goto Again;
////                        case DialogResult.Cancel:
////                            return;
////                    }
////                }
////            }
////            finally {
////                String.Format( "Using storage folder `{0}`.", this.MainStoragePath ).WriteLine();
////            }
////        }

////        //[Obsolete]
////        private void TestForReadWriteAccess() {
////            var randomFileName = Path.GetRandomFileName();
////            try {
////                var temp = Path.Combine( this.MainStoragePath.FullName, String.Format( "{0}", randomFileName ) );

////                //TODO
////                //NtfsAlternateStream.WriteAllText( temp, text: Randem.NextString( 144, true, true, true, true ) );
////                //NtfsAlternateStream.Delete( temp );
////            }
////            finally {
////                File.Delete( randomFileName );
////            }
////        }
////    }
//}
