// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PriorityTime.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

namespace Librainian.Collections {

    using System;
    using Newtonsoft.Json;

    [Obsolete( message: "what is time for?" )]
    [JsonObject]
    public class PriorityTime {

        [JsonProperty]
        public Single Priority { get; set; }

        [JsonProperty]
        public DateTime Time { get; set; }
    }
}