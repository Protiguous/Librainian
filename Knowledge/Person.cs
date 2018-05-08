﻿// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Person.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Knowledge {

    using System;
    using System.Collections.Generic;
    using Measurement.Time;
    using Newtonsoft.Json;

    [JsonObject]
    public class Person {

        [JsonProperty]
        public List<Person> Children = new List<Person>();

        [JsonProperty]
        public String Name;

        [JsonProperty]
        public List<Person> Parents = new List<Person>();

        [JsonProperty]
        public Date BirthDate {
            get; set;
        }
    }
}