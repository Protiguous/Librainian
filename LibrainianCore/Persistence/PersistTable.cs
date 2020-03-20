﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PersistTable.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "PersistTable.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Measurement.Time;
    using Microsoft.Database.Isam.Config;
    using Microsoft.Isam.Esent.Collections.Generic;
    using Microsoft.Isam.Esent.Interop.Windows81;
    using Newtonsoft.Json;
    using OperatingSystem.Compression;
    using OperatingSystem.FileSystem;
    using Parsing;
    using Utilities;

    /// <summary>
    ///     <para>Allows the <see cref="PersistentDictionary{TKey,TValue}" /> class to persist almost any object by using Newtonsoft.Json.</para>
    /// </summary>
    /// <see cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public sealed class PersistTable<TKey, TValue> : ABetterClassDispose, IDictionary<TKey, TValue> where TKey : IComparable<TKey> {

        public Int32 Count => this.Dictionary.Count;

        public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

        public ICollection<TKey> Keys => this.Dictionary.Keys;

        /// <summary>This deserializes the list of values.. I have a feeling this cannot be very fast.</summary>
        public ICollection<TValue> Values => ( ICollection<TValue> ) this.Dictionary.Values.Select( selector: value => value.FromCompressedBase64().FromJSON<TValue>() );

        /// <summary></summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public TValue this[ [CanBeNull] TKey key ] {
            [CanBeNull]
            get {
                if ( key is null ) {
                    return default;
                }

                if ( !this.Dictionary.TryGetValue( key: key, value: out var storedValue ) ) {
                    return default;
                }

                return storedValue.FromCompressedBase64().FromJSON<TValue>();
            }

            set {
                if ( key is null ) {
                    return;
                }

                if ( value is null ) {
                    this.Dictionary.Remove( key: key );

                    return;
                }

                this.Dictionary[ key: key ] = value.ToJSON().ToCompressedBase64();
            }
        }

        public void Add( TKey key, [CanBeNull] TValue value ) => this[ key: key ] = value;

        public void Add( KeyValuePair<TKey, TValue> item ) => this[ key: item.Key ] = item.Value;

        public void Clear() => this.Dictionary.Clear();

        public Boolean Contains( KeyValuePair<TKey, TValue> item ) {
            var value = item.Value.ToJSON()?.ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( key: item.Key, value: value );

            return this.Dictionary.Contains( item: asItem );
        }

        public Boolean ContainsKey( TKey key ) => this.Dictionary.ContainsKey( key: key );

        public void CopyTo( KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.Items().GetEnumerator();

        public Boolean Remove( TKey key ) => this.Dictionary.ContainsKey( key: key ) && this.Dictionary.Remove( key: key );

        public Boolean Remove( KeyValuePair<TKey, TValue> item ) {

            var value = item.Value.ToJSON()?.ToCompressedBase64();
            var asItem = new KeyValuePair<TKey, String>( key: item.Key, value: value );

            return this.Dictionary.Remove( item: asItem );

        }

        /// <summary>Gets the value associated with the specified key.</summary>
        /// <returns>true if the object that implements <see cref="IDictionary" /> contains an element with the specified key; otherwise, false.</returns>
        /// <param name="key">  The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the
        /// <paramref name="value" /> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public Boolean TryGetValue( [NotNull] TKey key, out TValue value ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            value = default;

            if ( !this.Dictionary.TryGetValue( key: key, value: out var storedValue ) ) {
                return default;
            }

            value = storedValue.FromCompressedBase64().FromJSON<TValue>();

            return true;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [JsonProperty]
        [NotNull]
        private PersistentDictionary<TKey, String> Dictionary { get; }

        /// <summary>No path given?</summary>
        [NotNull]
        public Folder Folder { get; }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private PersistTable() => throw new NotImplementedException();

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Environment.SpecialFolder specialFolder, [NotNull] String tableName ) : this( folder: new Folder( specialFolder: specialFolder,
            applicationName: null, subFolder: tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( Environment.SpecialFolder specialFolder, [CanBeNull] String subFolder, [NotNull] String tableName ) : this(
            folder: new Folder( specialFolder: specialFolder, applicationName: subFolder, subFolder: tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] Folder folder, [NotNull] String tableName ) : this( fullpath: Path.Combine( path1: folder.FullPath, path2: tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] Folder folder, [NotNull] String subFolder, [NotNull] String tableName ) : this( fullpath: Path.Combine( path1: folder.FullPath,
            path2: subFolder, path3: tableName ) ) { }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [CanBeNull] Folder folder, Boolean testForReadWriteAccess = false ) {
            try {
                this.Folder = folder ?? throw new ArgumentNullException( paramName: nameof( folder ) );

                if ( !this.Folder.Create() ) {
                    throw new DirectoryNotFoundException( message: $"Unable to find or create the folder `{this.Folder.FullPath}`." );
                }

                var customConfig = new DatabaseConfig {
                    CreatePathIfNotExist = true, EnableShrinkDatabase = ShrinkDatabaseGrbit.On, DefragmentSequentialBTrees = true
                };

                this.Dictionary = new PersistentDictionary<TKey, String>( directory: this.Folder.FullPath, customConfig: customConfig );

                if ( testForReadWriteAccess && !this.TestForReadWriteAccess().Result ) {
                    throw new IOException( message: $"Read/write permissions denied in folder {this.Folder.FullPath}." );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public PersistTable( [NotNull] String fullpath ) : this( folder: new Folder( fullPath: fullpath ) ) { }

        /// <summary>Return true if we can read/write in the <see cref="Folder" /> .</summary>
        /// <returns></returns>
        private async Task<Boolean> TestForReadWriteAccess() {
            try {
                using var document = this.Folder.TryGetTempDocument();

                var text = Randem.NextString( length: 64, lowers: true, uppers: true, numbers: true, symbols: true );
                document.AppendText( text: text );

                await document.TryDeleting( delayBetweenRetries: Seconds.Ten, token: CancellationToken.None ).ConfigureAwait( continueOnCapturedContext: false );

                return true;
            }
            catch { }

            return default;
        }

        /// <summary>Dispose any disposable managed fields or properties.</summary>
        public override void DisposeManaged() {
            Trace.Write( message: $"Disposing of {nameof( this.Dictionary )}..." );

            using ( this.Dictionary ) { }

            Trace.WriteLine( message: "done." );
        }

        public void Flush() => this.Dictionary.Flush();

        public void Initialize() {

            if ( this.Dictionary.Database.ToString().IsNullOrWhiteSpace() ) {
                throw new DirectoryNotFoundException( message: $"Unable to find or create the folder `{this.Folder.FullPath}`." );
            }
        }

        /// <summary>All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="TValue" /> deserialized.</summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<KeyValuePair<TKey, TValue>> Items() =>
            this.Dictionary.Select( selector: pair => new KeyValuePair<TKey, TValue>( key: pair.Key, value: pair.Value.FromCompressedBase64().FromJSON<TValue>() ) );

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        [NotNull]
        public override String ToString() => $"{this.Count} items";

        public void TryAdd( [NotNull] TKey key, [CanBeNull] TValue value ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            if ( !this.Dictionary.ContainsKey( key: key ) ) {
                this[ key: key ] = value;
            }
        }

        public Boolean TryRemove( [NotNull] TKey key ) {
            if ( key is null ) {
                throw new ArgumentNullException( paramName: nameof( key ) );
            }

            return this.Dictionary.ContainsKey( key: key ) && this.Dictionary.Remove( key: key );
        }

    }

}