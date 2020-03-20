// Copyright � 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AtomicInt.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "AtomicInt.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>An integer, thread-safe by <see cref="Interlocked" />.</summary>
    [JsonObject]
    public sealed class AtomicInt {

        /// <summary>ONLY always somtimes used in the getter and setter.</summary>
        [JsonProperty]
        private Int64 _value;

        public Int32 Value {
            get => ( Int32 ) Interlocked.Read( location: ref this._value );

            set => Interlocked.Exchange( location1: ref this._value, value: value );
        }

        public AtomicInt( Int32 value = 0 ) => this.Value = value;

        public static implicit operator Int32( [NotNull] AtomicInt special ) => special.Value;

        [NotNull]
        public static AtomicInt operator -( [NotNull] AtomicInt a1, [NotNull] AtomicInt a2 ) => new AtomicInt( value: a1.Value - a2.Value );

        [NotNull]
        public static AtomicInt operator *( [NotNull] AtomicInt a1, [NotNull] AtomicInt a2 ) => new AtomicInt( value: a1.Value * a2.Value );

        [NotNull]
        public static AtomicInt operator +( [NotNull] AtomicInt a1, [NotNull] AtomicInt a2 ) => new AtomicInt( value: a1.Value + a2.Value );

        [NotNull]
        public static AtomicInt operator ++( [NotNull] AtomicInt a1 ) {
            a1.Value++;

            return a1;
        }

        [NotNull]
        public static AtomicInt Parse( [NotNull] String value ) => new AtomicInt( value: Int32.Parse( s: value ) );

        /// <summary>Resets the value to zero if less than zero at this moment in time;</summary>
        public void CheckReset() {
            if ( this.Value < 0 ) {
                this.Value = 0;
            }
        }

        [NotNull]
        public override String ToString() => $"{this.Value}";

        //public long Increment( long byAmount ) {
        //    return Interlocked.Add( ref this._value, byAmount );
        //}

        //public long Decrement( long byAmount ) {
        //    return Interlocked.Add( ref this._value, -byAmount );
        //}

    }

}