// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DocumentCopyStatistics.cs" belongs to Rick@AIBrain.org and
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
// File "DocumentCopyStatistics.cs" was last formatted by Protiguous on 2018/06/04 at 3:46 PM.

namespace Librainian.ComputerSystems.FileSystem {

	using System;
	using System.Diagnostics;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;

	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class DocumentCopyStatistics {

		[JsonProperty]
		public UInt64 BytesCopied { get; set; }

		[JsonProperty]
		[CanBeNull]
		public Document DestinationDocument { get; set; }

		[JsonProperty]
		[CanBeNull]
		public String DestinationDocumentCRC64 { get; set; }

		[JsonProperty]
		[CanBeNull]
		public Document SourceDocument { get; set; }

		[JsonProperty]
		[CanBeNull]
		public String SourceDocumentCRC64 { get; set; }

		[JsonProperty]
		public DateTime TimeStarted { get; set; }

		[JsonProperty]
		public TimeSpan TimeTaken { get; set; }

		public Double BytesPerMillisecond() {
			if ( Math.Abs( this.TimeTaken.TotalMilliseconds ) < Double.Epsilon ) { return 0; }

			return this.BytesCopied / this.TimeTaken.TotalMilliseconds;
		}

		public Double MegabytesPerSecond() {
			if ( Math.Abs( this.TimeTaken.TotalSeconds ) < Double.Epsilon ) { return 0; }

			var mb = this.BytesCopied / ( Double ) Constants.Sizes.OneMegaByte;

			return mb / this.TimeTaken.TotalSeconds;
		}

		public Double MillisecondsPerByte() {
			if ( this.BytesCopied <= 0 ) { return 0; }

			return this.TimeTaken.TotalMilliseconds / this.BytesCopied;
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.SourceDocument?.FileName()} copied to {this.DestinationDocument.Folder} @ {this.MegabytesPerSecond()}MB/s";

	}

}