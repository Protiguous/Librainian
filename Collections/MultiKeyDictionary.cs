// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "MultiKeyDictionary.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original
// license has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/MultiKeyDictionary.cs" was last cleaned by Protiguous on 2018/05/15 at 1:28 AM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Multi-Key Dictionary Class
    /// </summary>
    /// <typeparam name="TK">Primary Key Type</typeparam>
    /// <typeparam name="TL">Sub Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    public class MultiKeyDictionary<TK, TL, TV> : ConcurrentDictionary<TK, TV> {

        internal readonly ConcurrentDictionary<TK, TL> PrimaryToSubkeyMapping = new ConcurrentDictionary<TK, TL>();

        internal readonly ConcurrentDictionary<TL, TK> SubDictionary = new ConcurrentDictionary<TL, TK>();

        public TV this[TL subKey] {
            get {
                if ( this.TryGetValue( subKey: subKey, val: out var item ) ) { return item; }

                throw new KeyNotFoundException( message: $"sub key not found: {subKey}" );
            }
        }

        public new TV this[TK primaryKey] {
            get {
                if ( this.TryGetValue( primaryKey: primaryKey, val: out var item ) ) { return item; }

                throw new KeyNotFoundException( message: $"primary key not found: {primaryKey}" );
            }
        }

        public void Add( TK primaryKey, TV val ) => this.TryAdd( primaryKey, value: val );

        public void Add( TK primaryKey, TL subKey, TV val ) {
            this.TryAdd( primaryKey, value: val );

            this.Associate( subKey: subKey, primaryKey: primaryKey );
        }

        public void Associate( TL subKey, TK primaryKey ) {
            if ( !base.ContainsKey( primaryKey ) ) { throw new KeyNotFoundException( message: $"The primary dictionary does not contain the key '{primaryKey}'" ); }

            if ( this.SubDictionary.ContainsKey( subKey ) ) {
                this.SubDictionary[subKey] = primaryKey;
                this.PrimaryToSubkeyMapping[primaryKey] = subKey;
            }
            else {
                this.SubDictionary.TryAdd( subKey, value: primaryKey );
                this.PrimaryToSubkeyMapping.TryAdd( primaryKey, value: subKey );
            }
        }

        public TK[] ClonePrimaryKeys() => this.Keys.ToArray();

        public TL[] CloneSubKeys() => this.SubDictionary.Keys.ToArray();

        public TV[] CloneValues() => this.Values.ToArray();

        public Boolean ContainsKey( TL subKey ) => this.TryGetValue( subKey: subKey, val: out var val );

        public new Boolean ContainsKey( TK primaryKey ) => this.TryGetValue( primaryKey: primaryKey, val: out var val );

        public void Remove( TK primaryKey ) {
            this.SubDictionary.TryRemove( this.PrimaryToSubkeyMapping[primaryKey], value: out var kvalue );

            this.PrimaryToSubkeyMapping.TryRemove( primaryKey, value: out var lvalue );

            this.TryRemove( primaryKey, value: out var value );
        }

        public void Remove( TL subKey ) {
            this.TryRemove( this.SubDictionary[subKey], value: out var value );
            this.PrimaryToSubkeyMapping.TryRemove( this.SubDictionary[subKey], value: out var lvalue );
            this.SubDictionary.TryRemove( subKey, value: out var kvalue );
        }

        public Boolean TryGetValue( TL subKey, out TV val ) {
            val = default;

            return this.SubDictionary.TryGetValue( subKey, value: out var ep ) && this.TryGetValue( primaryKey: ep, val: out val );
        }

        public new Boolean TryGetValue( TK primaryKey, out TV val ) => base.TryGetValue( primaryKey, value: out val );
    }
}