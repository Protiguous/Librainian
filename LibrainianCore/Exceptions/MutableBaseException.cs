// Copyright � 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "MutableBaseException.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "MutableBaseException.cs" was last formatted by Protiguous on 2020/03/16 at 3:04 PM.

namespace Librainian.Exceptions {

    using System;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    [Serializable]
    public class MutableBaseException : ImmutableFailureException {

        protected MutableBaseException( [NotNull] SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo: serializationInfo,
            streamingContext: streamingContext ) { }

        internal MutableBaseException( [NotNull] Type type, [NotNull] Exception inner ) : base( type: type, message: FormatMessage( type: type ), inner: inner ) {
            if ( type is null ) {
                throw new ArgumentNullException( paramName: nameof( type ) );
            }

            if ( inner is null ) {
                throw new ArgumentNullException( paramName: nameof( inner ) );
            }
        }

        [NotNull]
        private static String FormatMessage( [NotNull] Type type ) {
            if ( type is null ) {
                throw new ArgumentNullException( paramName: nameof( type ) );
            }

            return $"'{type}' is mutable because its base type ('[{type.BaseType}]') is mutable.";
        }
    }
}