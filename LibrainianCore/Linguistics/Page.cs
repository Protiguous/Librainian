// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Page.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Page.cs" was last formatted by Protiguous on 2020/03/16 at 3:05 PM.

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>A <see cref="Page" /> is a sequence of <see cref="Paragraph" /> .</para>
    /// </summary>
    /// <see cref="Book"></see>
    [JsonObject]
    [Immutable]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "()}" )]
    [Serializable]
    public sealed class Page : IEquatable<Page>, IEnumerable<Paragraph> {

        [NotNull]
        [JsonProperty]
        private List<Paragraph> Paragraphs { get; } = new List<Paragraph>();

        public static Page Empty { get; } = new Page();

        private Page() { }

        public Page( [NotNull] IEnumerable<Paragraph> paragraphs ) {
            if ( paragraphs is null ) {
                throw new ArgumentNullException( paramName: nameof( paragraphs ) );
            }

            this.Paragraphs.AddRange( collection: paragraphs.Where( predicate: paragraph => paragraph != null ) );
        }

        public Boolean Equals( [CanBeNull] Page other ) {
            if ( other is null ) {
                return default;
            }

            return ReferenceEquals( objA: this, objB: other ) || this.Paragraphs.SequenceEqual( second: other.Paragraphs );
        }

        public IEnumerator<Paragraph> GetEnumerator() => this.Paragraphs.GetEnumerator();

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Paragraphs.GetHashCode();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}