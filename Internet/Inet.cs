﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Inet.cs" belongs to Rick@AIBrain.org and
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
// File "Inet.cs" was last formatted by Protiguous on 2018/06/04 at 3:59 PM.

namespace Librainian.Internet {

	using System;
	using System.IO;
	using System.Net;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	public static class Inet {

		[ItemNotNull]
		private static async Task<Byte[]> GetUrlContentsAsync( [NotNull] String url ) {

			// The downloaded resource ends up in the variable named content.
			var content = new MemoryStream();

			// Send the request to the Internet resource and wait for the response. ReSharper
			// disable once PossibleNullReferenceException
			if ( !( WebRequest.Create( url ) is HttpWebRequest webReq ) ) { return content.ToArray(); }

			using ( var response = await webReq.GetResponseAsync() )

				// The previous statement abbreviates the following two statements.

				//Task<WebResponse> responseTask = webReq.GetResponseAsync();
				//using (WebResponse response = await responseTask)
			{
				// Get the data stream that is associated with the specified url.
				using ( var responseStream = response.GetResponseStream() ) {

					// Read the bytes in responseStream and copy them to content.
					if ( responseStream != null ) { await responseStream.CopyToAsync( content ); }

					// The previous statement abbreviates the following two statements.

					// CopyToAsync returns a Task, not a Task<T>.
					//Task copyTask = responseStream.CopyToAsync(content);

					// When copyTask is completed, content contains a copy of
					// responseStream.
					//await copyTask;
				}
			}

			// Return the result as a byte array.
			return content.ToArray();
		}

	}

}