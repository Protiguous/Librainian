// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "MultiKeyDictionary.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "MultiKeyDictionary.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>Multi-Key Dictionary Class</summary>
    /// <typeparam name="TK">Primary Key Type</typeparam>
    /// <typeparam name="TL">Sub Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    public class MultiKeyDictionary<TK, TL, TV> : ConcurrentDictionary<TK, TV> {

        internal ConcurrentDictionary<TK, TL> PrimaryToSubkeyMapping { get; } = new ConcurrentDictionary<TK, TL>();

        internal ConcurrentDictionary<TL, TK> SubDictionary { get; } = new ConcurrentDictionary<TL, TK>();

        [CanBeNull]
        public TV this[ [NotNull] TL subKey ] {
            get {
                if ( this.TryGetValue( subKey: subKey, val: out var item ) ) {
                    return item;
                }

                throw new KeyNotFoundException( message: $"sub key not found: {subKey}" ); //TODO I hate throwing exceptions in indexers..
            }
        }

        [CanBeNull]
        public new TV this[ [NotNull] TK primaryKey ] {
            get {
                if ( this.TryGetValue( primaryKey: primaryKey, val: out var item ) ) {
                    return item;
                }

                throw new KeyNotFoundException( message: $"primary key not found: {primaryKey}" );
            }
        }

        public void Add( [NotNull] TK primaryKey, [CanBeNull] TV val ) => this.TryAdd( key: primaryKey, value: val );

        public void Add( [NotNull] TK primaryKey, [NotNull] TL subKey, [CanBeNull] TV val ) {
            this.TryAdd( key: primaryKey, value: val );

            this.Associate( subKey: subKey, primaryKey: primaryKey );
        }

        public void Associate( [NotNull] TL subKey, [NotNull] TK primaryKey ) {
            if ( !base.ContainsKey( key: primaryKey ) ) {
                throw new KeyNotFoundException( message: $"The primary dictionary does not contain the key '{primaryKey}'" );
            }

            if ( this.SubDictionary.ContainsKey( key: subKey ) ) {
                this.SubDictionary[ key: subKey ] = primaryKey;
                this.PrimaryToSubkeyMapping[ key: primaryKey ] = subKey;
            }
            else {
                this.SubDictionary.TryAdd( key: subKey, value: primaryKey );
                this.PrimaryToSubkeyMapping.TryAdd( key: primaryKey, value: subKey );
            }
        }

        [NotNull]
        public TK[] ClonePrimaryKeys() => this.Keys.ToArray();

        [NotNull]
        public TL[] CloneSubKeys() => this.SubDictionary.Keys.ToArray();

        [NotNull]
        public TV[] CloneValues() => this.Values.ToArray();

        public Boolean ContainsKey( [NotNull] TL subKey ) => this.TryGetValue( subKey: subKey, val: out _ );

        public new Boolean ContainsKey( [NotNull] TK primaryKey ) => this.TryGetValue( primaryKey: primaryKey, val: out _ );

        public void Remove( [NotNull] TK primaryKey ) {
            this.SubDictionary.TryRemove( key: this.PrimaryToSubkeyMapping[ key: primaryKey ], value: out _ );

            this.PrimaryToSubkeyMapping.TryRemove( key: primaryKey, value: out var lvalue );

            this.TryRemove( key: primaryKey, value: out var value );
        }

        public void Remove( [NotNull] TL subKey ) {
            this.TryRemove( key: this.SubDictionary[ key: subKey ], value: out var value );
            this.PrimaryToSubkeyMapping.TryRemove( key: this.SubDictionary[ key: subKey ], value: out var lvalue );
            this.SubDictionary.TryRemove( key: subKey, value: out var kvalue );
        }

        public Boolean TryGetValue( [NotNull] TL subKey, [CanBeNull] out TV val ) {
            val = default;

            return this.SubDictionary.TryGetValue( key: subKey, value: out var ep ) && this.TryGetValue( primaryKey: ep, val: out val );
        }

        public new Boolean TryGetValue( [NotNull] TK primaryKey, [CanBeNull] out TV val ) => base.TryGetValue( key: primaryKey, value: out val );
    }
}