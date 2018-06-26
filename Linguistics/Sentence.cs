﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Sentence.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Sentence.cs" was last formatted by Protiguous on 2018/06/04 at 4:02 PM.

namespace Librainian.Linguistics {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Collections;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Parsing;

	/// <summary>
	///     A <see cref="Sentence" /> is an ordered sequence of <see cref="Word" /> .
	/// </summary>
	/// <seealso cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></seealso>
	/// <seealso cref="Paragraph"></seealso>
	[JsonObject]
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
	[Serializable]
	public sealed class Sentence : IEquatable<Sentence>, IEnumerable<Word>, IComparable<Sentence> {

		public Int32 CompareTo( [NotNull] Sentence other ) => String.Compare( this.ToString(), other.ToString(), StringComparison.Ordinal );

		public IEnumerator<Word> GetEnumerator() => this.Words.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public Boolean Equals( Sentence other ) {
			if ( other is null ) { return false; }

			return ReferenceEquals( this, other ) || this.SequenceEqual( other );
		}

		public static Sentence Empty { get; } = new Sentence();

		//public static implicit operator String( Sentence sentence ) {return sentence != null ? sentence.Words.ToStrings( " " ) : String.Empty;}

		/// <summary></summary>
		[NotNull]
		[JsonProperty]
		private List<Word> Words { get; } = new List<Word>();

		public override Int32 GetHashCode() => this.Words.GetHashCode();

		[NotNull]
		public IEnumerable<Sentence> Possibles() => this.Words.ToArray().FastPowerSet().Select( words => new Sentence( words ) ).Where( sentence => !sentence.ToString().IsNullOrEmpty() );

		public override String ToString() => this.Words.ToStrings( " " );

		/// <summary></summary>
		public static readonly Sentence EndOfLine = new Sentence( "\0" );

		private Sentence() { }

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="sentence"></param>
		public Sentence( [NotNull] String sentence ) : this( sentence.ToWords().Select( word => new Word( word ) ) ) { }

		/// <summary>A <see cref="Sentence" /> is an ordered sequence of words.</summary>
		/// <param name="words"></param>
		public Sentence( [NotNull] IEnumerable<Word> words ) {
			if ( words is null ) { throw new ArgumentNullException( nameof( words ) ); }

			this.Words.AddRange( words.Where( word => word != null ) );
			this.Words.Fix();
		}

		//[NotNull]
		//public Word TakeFirst() {
		//    try {
		//        return this.Words.TakeFirst() ?? new Word( String.Empty );
		//    }
		//    finally {
		//        this.Words.Fix();
		//    }
		//}

	}

}