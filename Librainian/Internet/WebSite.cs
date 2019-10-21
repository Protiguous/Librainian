﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WebSite.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "WebSite.cs" was last formatted by Protiguous on 2019/08/08 at 8:03 AM.

namespace Librainian.Internet {

    using System;
    using System.Net;
    using Newtonsoft.Json;

    [JsonObject]
    public class WebSite {

        [JsonProperty]
        public String Document { get; set; }

        /// <summary>URI of the website.</summary>
        [JsonProperty]
        public Uri Location { get; set; }

        /// <summary>The <see cref="HttpWebRequest" />.</summary>
        public HttpWebRequest Request { get; set; }

        /// <summary>A count of the requests to scrape this website.</summary>
        [JsonProperty]
        public UInt64 RequestCount { get; set; }

        /// <summary>A count of the responses for this url.</summary>
        [JsonProperty]
        public UInt64 ResponseCount { get; set; }

        /// <summary>When this website was added.</summary>
        [JsonProperty]
        public DateTime WhenAddedToQueue { get; set; }

        /// <summary>When the last webrequest was started.</summary>
        [JsonProperty]
        public DateTime WhenRequestStarted { get; set; }

        /// <summary>When the last webrequest got a response.</summary>
        [JsonProperty]
        public DateTime WhenResponseCame { get; set; }
    }
}