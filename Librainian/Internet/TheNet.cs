// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TheNet.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "TheNet.cs" was last formatted by Protiguous on 2019/08/08 at 8:02 AM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Measurement.Time;
    using Newtonsoft.Json;

    public static class TheNet {

        [ItemCanBeNull]
        public static async Task<T> DeserializeJson<T>( [NotNull] this Uri uri, TimeSpan? timeout = null ) {
            if ( uri is null ) {
                throw new ArgumentNullException( paramName: nameof( uri ) );
            }

            if ( timeout is null ) {
                timeout = TimeSpan.Zero + Seconds.Seven;
            }

            var response = await uri.GetWebPageAsync( timeout.Value ).ConfigureAwait( false );

            return JsonConvert.DeserializeObject<T>( response );
        }

        /// <summary>Convert network bytes to a string</summary>
        /// <exception cref="ArgumentException"></exception>
        [NotNull]
        public static String FromNetworkBytes( [NotNull] this IEnumerable<Byte> data ) {
            var listData = data as IList<Byte> ?? data.ToList();

            var len = IPAddress.NetworkToHostOrder( network: BitConverter.ToInt16( listData.Take( count: 2 ).ToArray(), startIndex: 0 ) );

            if ( listData.Count < 2 + len ) {
                throw new ArgumentException( "Too few bytes in packet" );
            }

            return Encoding.UTF8.GetString( bytes: listData.Skip( count: 2 ).Take( count: len ).ToArray() );
        }

        /// <summary>Return the machine's hostname</summary>
        [NotNull]
        public static String GetHostName() => Dns.GetHostName();

        [CanBeNull]
        public static String GetWebPage( [CanBeNull] this String url ) {
            try {
                if ( Uri.TryCreate( url, UriKind.Absolute, out var uri ) ) {
                    using ( var client = new WebClient {
                        Encoding = Encoding.Unicode,
                        CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore )
                    } ) {
                        return client.DownloadString( uri );
                    }
                }

                throw new Exception( $"Unable to connect to {url}." );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        [CanBeNull]
        public static async Task<String> GetWebPageAsync( [NotNull] this String url, TimeSpan timeout ) {
            if ( String.IsNullOrWhiteSpace( value: url ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( url ) );
            }

            try {
                if ( Uri.TryCreate( url, UriKind.Absolute, out var uri ) ) {
                    return await uri.GetWebPageAsync( timeout ).ConfigureAwait( false );
                }

                throw new Exception( $"Unable to parse the url:{url}." );
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return null;
        }

        [CanBeNull]
        public static Task<String> GetWebPageAsync( [NotNull] this Uri uri, TimeSpan timeout ) {
            if ( uri is null ) {
                throw new ArgumentNullException( paramName: nameof( uri ) );
            }

            try {
                var client = new WebClient {
                    Encoding = Encoding.UTF8,
                    CachePolicy = new RequestCachePolicy( RequestCacheLevel.NoCacheNoStore )
                };

                client.SetTimeout( timeout );

                return client.DownloadStringTaskAsync( uri );
            }
            catch ( Exception exception ) {
                exception.Log();

                return Task.FromResult<String>( null );
            }
        }

        /// <summary>Convert a string to network bytes</summary>
        [NotNull]
        public static IEnumerable<Byte> ToNetworkBytes( [NotNull] this String data ) {
            if ( String.IsNullOrEmpty( value: data ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( data ) );
            }

            var bytes = Encoding.UTF8.GetBytes( s: data );

            var hostToNetworkOrder = IPAddress.HostToNetworkOrder( host: ( Int16 )bytes.Length );

            return BitConverter.GetBytes( hostToNetworkOrder ).Concat( second: bytes );
        }
    }
}