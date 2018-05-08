﻿// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks goes
// to the Authors.
//
// Donations and royalties can be paid via
// PayPal: paypal@Protiguous.com
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// 
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Litoshi.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Currency.LTC {

    using System;
    using System.Diagnostics;

#pragma warning disable IDE0009 // Member access should be qualified.
    [DebuggerDisplay( "{" + nameof( Formatted ) + ",nq}" )]
#pragma warning restore IDE0009 // Member access should be qualified.
    public struct Litoshi : ICoin {

        public Decimal FaceValue => 0.00000001M;

        public String Formatted => $"Ł{this.FaceValue:f8}";
    }
}