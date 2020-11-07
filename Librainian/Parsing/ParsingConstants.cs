// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ParsingConstants.cs" last formatted on 2020-10-19 at 7:23 AM.

#nullable enable

namespace Librainian.Parsing {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using JetBrains.Annotations;
	using Measurement.Time;

	public static class ParsingConstants {
		/// <summary>
		///     Two spaces as a <see cref="string" />.
		/// </summary>
		[NotNull]
		public const String Doublespace = Singlespace + Singlespace;

		/// <summary> ~`!@#$%^&*()-_=+?:,./\[]{}|' </summary>
		[NotNull]
		public const String Symbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

		/// <summary>
		///     The tab char as a <see cref="String" />.
		/// </summary>
		public const String Tab = "\t";

		/// <summary>
		///     The ' char as a <see cref="String" />.
		/// </summary>
		public const String SingleQuote = "'";

		/// <summary>
		///     The " char as a <see cref="String" />.
		/// </summary>
		public const String DoubleQuote = "\"";

		/// <summary>
		///     Char x0D as a <see cref="String" />.
		/// </summary>
		public const String CR = "\r";

		/// <summary>
		///     Char x0A as a <see cref="String" />.
		/// </summary>
		public const String LF = "\n";

		/// <summary>
		///     The chars x0D0A as a <see cref="String" />.
		/// </summary>
		public const String CRLF = CR + LF;

		/// <summary>
		///     A single space char as a <see cref="String" />.
		/// </summary>
		public const String Singlespace = " ";

		/// <summary>
		///     Char x0D as a <see cref="String" />.
		/// </summary>
		public const String CarriageReturn = CR;

		/// <summary>
		///     Char x0A as a <see cref="String" />.
		/// </summary>
		public const String LineFeed = LF;

		/// <summary>
		///     (<see cref="Char" />)0x0000
		/// </summary>
		public const Char NullChar = ( Char )0x0000;

		/// <summary>N, 0, no, false, fail, failed, failure, bad</summary>
		[NotNull]
		[ItemNotNull]
		public static readonly String[] FalseStrings = {
			"N", "0", "no", "false", Boolean.FalseString, "fail", "failed", "failure", "bad"
		};

		/// <summary>Y, 1</summary>
		[NotNull]
		public static readonly Char[] TrueChars = {
			'Y', '1'
		};

		/// <summary>Y, 1, yes, true, Success, good, Go, Positive, Continue</summary>
		[NotNull]
		[ItemNotNull]
		public static readonly String[] TrueStrings = {
			"Y", "1", "yes", "true", Boolean.TrueString, nameof( Status.Success ), "good", nameof( Status.Go ), nameof( Status.Positive ), nameof( Status.Continue )
		};

		[NotNull]
		public static readonly Regex UpperCaseRegeEx = new Regex( @"^[A-Z]+$", RegexOptions.Compiled, Minutes.One );

		[NotNull]
		[ItemNotNull]
		public static Lazy<String> AllLetters { get; } = new Lazy<String>( () =>
			new String( Enumerable.Range( UInt16.MinValue, UInt16.MaxValue ).Select( i => ( Char )i ).Where( Char.IsLetter ).Distinct().OrderBy( c => c ).ToArray() ) );

		/// <summary>The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.</summary>
		[NotNull]
		public static IEnumerable<String> UriRfc3986CharsToEscape { get; } = new[] {
			"!", "*", "'", "(", ")"
		};

		public static class English {
			[NotNull]
			public const String Numbers = "0123456789";

			[NotNull]
			public static String[] Vowels { get; } = "A,AI,AU,E,EA,EE,I,IA,IO,O,OA,OI,OO,OU,U".Split( ',' );

			[NotNull]
			public static String[] Consonants { get; } = "B,C,CH,CL,D,F,FF,G,GH,GL,J,K,L,LL,M,MN,N,P,PH,PS,R,RH,S,SC,SH,SK,ST,T,TH,V,W,X,Y,Z".Split( ',' );

			/// <summary></summary>
			[NotNull]
			[ItemNotNull]
			public static String[] TensMap { get; } = {
				"zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"
			};

			/// <summary></summary>
			[NotNull]
			[ItemNotNull]
			public static String[] UnitsMap { get; } = {
				"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
				"ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
			};

			public static class Alphabet {
				[NotNull]
				public const String Lowercase = "abcdefghijklmnopqrstuvwxyz";

				/// <summary>ABCDEFGHIJKLMNOPQRSTUVWXYZ</summary>
				[NotNull]
				public const String Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			}
		}

		public static class RegexPatterns {
			[NotNull]
			public const String MatchGitignoreCommentLine = @"^[#].*";

			[NotNull]
			public const String MatchBlankLine = @"^\s*$";

			[NotNull]
			public const String MatchMoney = @"//\$\s*[-+]?([0-9]{0,3}(,[0-9]{3})*(\.[0-9]+)?)";

			[NotNull]
			public const String SplitByEnglish = @"(?:\p{Lu}(?:\.\p{Lu})+)(?:,\s*\p{Lu}(?:\.\p{Lu})+)*";

			/// <summary>Regex pattern for words that don't start with a number</summary>
			[NotNull]
			public const String SplitByWordNotNumber = @"([a-zA-Z]\w+)\W*";
		}
	}
}