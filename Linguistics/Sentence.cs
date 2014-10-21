﻿// This notice must be kept visible in the source.
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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Sentence.cs" was last cleaned by Rick on 2014/10/21 at 5:02 AM

namespace Librainian.Linguistics {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Annotations;
    using Collections;
    using Extensions;
    using FluentAssertions;
    using Parsing;

    /// <summary>
    /// A <see cref="Sentence" /> is an ordered sequence of <see cref="Word" /> .
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></seealso>
    /// <seealso cref="Paragraph"></seealso>
    [DataContract( IsReference = true )]
    [Immutable]
    public sealed class Sentence : IEquatable<Sentence>, IEnumerable<Word> {
        public const UInt64 Level = Word.Level << 1;

        [NotNull]
        [DataMember]
        private readonly List<Word> _tokens = new List<Word>();

        static Sentence() {
            Level.Should().BeGreaterThan( Word.Level );
        }

        /// <summary>
        /// A <see cref="Sentence" /> is an ordered sequence of words.
        /// </summary>
        /// <param name="sentence"></param>
        public Sentence( [NotNull] String sentence ) : this( sentence.ToWords().Select( word => new Word( word ) ) ) {
        }

        /// <summary>
        /// A <see cref="Sentence" /> is an ordered sequence of words.
        /// </summary>
        /// <param name="words"></param>
        public Sentence( [NotNull] IEnumerable<Word> words ) {
            if ( words == null ) {
                throw new ArgumentNullException( "words" );
            }
            this._tokens.AddRange( words );
            this._tokens.Fix();
        }

        public static implicit operator String( Sentence sentence ) {
            return sentence._tokens.ToStrings( " " );
        }

        public Boolean Equals( [NotNull] Sentence other ) {
            if ( other == null ) {
                throw new ArgumentNullException( "other" );
            }
            return ReferenceEquals( this, other ) || this.SequenceEqual( other );
        }

        public IEnumerator<Word> GetEnumerator() {
            return this._tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public IEnumerable<Tuple<int, String>> Possibles() {
            var wordCount = this.Count();
            for ( var slider = wordCount ; slider > 0 ; slider-- ) {
                for ( var skip = 0 ; skip < wordCount ; skip++ ) {
                    var words = this.Skip( skip ).Take( slider );
                    var partialSentence = new Sentence( words );
                    var asString = partialSentence.ToString();
                    if ( String.IsNullOrEmpty( asString ) ) {
                        continue;
                    }
                    yield return new Tuple<int, String>( 2, asString );

                    foreach ( var possibles in partialSentence.SelectMany( word => word.Possibles() ) ) {
                        yield return possibles;
                    }
                }
            }
        }

        [NotNull]
        public Word TakeFirst() {
            try {
                return this._tokens.TakeFirst() ?? new Word( String.Empty );
            }
            finally {
                this._tokens.Fix();
            }
        }

        public override String ToString() {
            return this._tokens.ToStrings( " " );
        }
    }
}