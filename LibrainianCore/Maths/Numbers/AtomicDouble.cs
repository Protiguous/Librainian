// Copyright � 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AtomicDouble.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "AtomicDouble.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.ComponentModel;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>A Double number thread-safe by <see cref="Interlocked" />.</summary>
    [JsonObject]
    [Description( description: "A Double number thread-safe by Interlocked." )]
    public struct AtomicDouble {

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Double _value;

        public Double Value {
            get => Interlocked.Exchange( location1: ref this._value, value: this._value );

            set => Interlocked.Exchange( location1: ref this._value, value: value );
        }

        public AtomicDouble( Double value ) : this() => this.Value = value;

        public static implicit operator Double( AtomicDouble special ) => special.Value;

        public static AtomicDouble operator -( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( value: a1.Value - a2.Value );

        public static AtomicDouble operator *( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( value: a1.Value * a2.Value );

        public static AtomicDouble operator +( AtomicDouble a1, AtomicDouble a2 ) => new AtomicDouble( value: a1.Value + a2.Value );

        public static AtomicDouble operator ++( AtomicDouble a1 ) {
            a1.Value++;

            return a1;
        }

        public static AtomicDouble Parse( [NotNull] String value ) {
            if ( value is null ) {
                throw new ArgumentNullException( paramName: nameof( value ) );
            }

            return new AtomicDouble( value: Double.Parse( s: value ) );
        }

        /// <summary>Resets the value to zero if less than zero;</summary>
        public void CheckReset() {
            if ( this.Value < 0 ) {
                this.Value = 0;
            }
        }

        [NotNull]
        public override String ToString() => $"{this.Value:R}";

    }

}