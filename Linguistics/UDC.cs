// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/UDC.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Linguistics {

    using System;

    /// <summary>Universal Decimal Classification</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Universal_Decimal_Classification"></seealso>
    /// <example>
    ///     539. 120 Theoretical problems of elementary particles physics. Theories and models of
    ///     fundamental interactions.
    ///     539. 120.2 Symmetries of quantum physics
    ///     539. 120.224 Reflection in time and space
    /// </example>
    public class Udc {

        //TODO Should a Book be aware of its own UDC?

        public static readonly Udc Unknown = new Udc( String.Empty );

        public Udc( String notation ) {
            this.Notation = notation;
        }

        public String Notation {
            get; private set;
        }
    }
}