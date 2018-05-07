﻿// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TypeOrClass.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Knowledge {

    using System;

    /// <summary></summary>
    /// <example>For example (rdf:type Morris Cat) means "Morris is a type of Cat"</example>
    public class TypeOrClass {

        public TypeOrClass( String label ) => this.Label = String.IsNullOrWhiteSpace( label ) ? Guid.NewGuid().ToString() : label;

	    public Domain Domain {
            get; private set;
        }

        /// <summary>The name of this type (All X are T).</summary>
        /// <example>Cat. Canine. Mammal</example>
        public String Label {
            get;
        }
    }
}