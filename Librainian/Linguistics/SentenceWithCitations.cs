﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "SentenceWithCitations.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Linguistics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using Newtonsoft.Json;
using Parsing;

/// <summary>A <see cref="SentenceWithCitations" /> is an ordered sequence of <see cref="Word" /> .</summary>
/// <see cref="http://wikipedia.org/wiki/Sentence_(linguistics)"></see>
/// <see cref="Paragraph"></see>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "()}" )]
[Serializable]
public record SentenceWithCitations : Sentence, IHasCitations {
	static SentenceWithCitations() => Empty = ( SentenceWithCitations )Parse( $"{StartOfSentence}{String.Empty}{EndOfSentence}" );

	/// <summary>A <see cref="SentenceWithCitations" /> is an ordered sequence of words.</summary>
	/// <param name="sentence"></param>
	private SentenceWithCitations( String sentence ) : this( sentence.ToWords() ) { }

	/// <summary>A <see cref="SentenceWithCitations" /> is an ordered sequence of words.</summary>
	/// <param name="words"></param>
	public SentenceWithCitations( IEnumerable<Word> words ) : base( words ) {
		foreach ( var word in words ) {
			this.Words.Add( word );
		}

		this.Words.TrimExcess();
	}

	/// <summary>A <see cref="SentenceWithCitations" /> is an ordered sequence of words.</summary>
	/// <param name="words"></param>
	public SentenceWithCitations( IEnumerable<String> words ) : this( words.ToStrings( ParsingConstants.Strings.Singlespace ).ToWords() ) { }

	[JsonProperty]
	private List<Word> Words { get; } = new();

	public new static SentenceWithCitations Empty { get; }

	//public Boolean Equals( Sentence? other ) => Equals( this, other );

	public Lazy<HashSet<ICitation>?>? Citations { get; set; }

	public Int32 CompareTo( SentenceWithCitations? other ) => String.Compare( this.ToString(), other?.ToString(), StringComparison.Ordinal );

	public static Boolean Equals( SentenceWithCitations? left, SentenceWithCitations? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		return left.Words.SequenceEqual( right.Words );
	}

	public IEnumerable<ICitation>? GetCitations() => this.Citations?.Value;

	/// <summary></summary>
	public override Int32 GetHashCode() => this.Words.GetHashCode();

	public override String ToString() => this.Words.ToStrings( ParsingConstants.Strings.Singlespace );

	public static Boolean operator <=( SentenceWithCitations left, SentenceWithCitations right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( SentenceWithCitations left, SentenceWithCitations right ) => left.CompareTo( right ) >= 0;

	public static Boolean operator <( SentenceWithCitations left, SentenceWithCitations right ) => left.CompareTo( right ) < 0;

	public static Boolean operator >( SentenceWithCitations left, SentenceWithCitations right ) => left.CompareTo( right ) > 0;
}