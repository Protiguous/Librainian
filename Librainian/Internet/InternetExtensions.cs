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
// "Librainian/InternetExtensions.cs" was last cleaned by Rick on 2016/08/15 at 4:59 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class InternetExtensions {

        public static Boolean IsValidIp( this String ip ) {
            if ( !Regex.IsMatch( ip, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}" ) ) {
                return false;   //TODO precompile this regex
            }

            var ips = ip.Split( '.' );
            if ( ips.Length == 4 || ips.Length == 6 ) {
                return Int32.Parse( ips[ 0 ] ) < 256 && ( Int32.Parse( ips[ 1 ] ) < 256 ) & ( Int32.Parse( ips[ 2 ] ) < 256 ) & ( Int32.Parse( ips[ 3 ] ) < 256 );
            }

            return false;
        }

        private static Regex ValidateURLRegex { get; } = new Regex( @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled );

        public static Boolean IsValidUrl( this String text ) => ValidateURLRegex.IsMatch( text );

        public static async Task<TextReader> DoRequestAsync( [NotNull] this WebRequest request ) {
            if ( request == null ) {
                throw new ArgumentNullException( nameof( request ) );
            }

            var result = await Task.Factory.FromAsync( ( asyncCallback, state ) => ( ( HttpWebRequest )state ).BeginGetResponse( asyncCallback, state ), asyncResult => ( ( HttpWebRequest )asyncResult.AsyncState ).EndGetResponse( asyncResult ), request );
            var stream = result.GetResponseStream();
            return stream != null ? new StreamReader( stream ) : TextReader.Null;
        }

        public static async Task<TextReader> DoRequestAsync( [NotNull] this Uri uri ) {
            if ( uri == null ) {
                throw new ArgumentNullException( nameof( uri ) );
            }

            var request = WebRequest.CreateHttp( uri );

            //request.AllowReadStreamBuffering = true;
            var textReader = await request.DoRequestAsync();
            return textReader;
        }

        public static async Task<T> DoRequestJsonAsync<T>( [NotNull] this WebRequest request ) {
            if ( request == null ) {
                throw new ArgumentNullException( nameof( request ) );
            }

            var reader = await DoRequestAsync( request ).ConfigureAwait( false ); 
            var response = await reader.ReadToEndAsync().ConfigureAwait( false ); 
            return JsonConvert.DeserializeObject<T>( response );
        }

        public static async Task<T> DoRequestJsonAsync<T>( [NotNull] Uri uri ) {
            var reader = await DoRequestAsync( uri ).ConfigureAwait(false);
            var response = await reader.ReadToEndAsync().ConfigureAwait( false );
            return JsonConvert.DeserializeObject<T>( response );
        }

        /// <summary>Convert network bytes to a string</summary>
        /// <exception cref="ArgumentException"></exception>
        [NotNull]
        public static String FromNetworkBytes( [NotNull] this IEnumerable<Byte> data ) {
            var listData = data as IList<Byte> ?? data.ToList();

            var len = IPAddress.NetworkToHostOrder( BitConverter.ToInt16( listData.Take( 2 ).ToArray(), 0 ) );
            if ( listData.Count < 2 + len ) {
                throw new ArgumentException( "Too few bytes in packet" );
            }

            return Encoding.UTF8.GetString( listData.Skip( 2 ).Take( len ).ToArray() );
        }

        /// <summary>Return the machine's hostname</summary>
        [NotNull]
        public static String GetHostName() => Dns.GetHostName();

        public static JObject GetNonAsync( Uri uri ) {
            var httpClient = new HttpClient();
            var content = httpClient.GetStringAsync( uri ).Result;  //TODO bad
            return JObject.Parse( content );
        }

        public static String GetWebPage2( [NotNull] this String url ) {
            try {
                var request = WebRequest.Create( url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                using ( var response = request.GetResponse() as HttpWebResponse ) {
                    var dataStream = response?.GetResponseStream();
                    if ( dataStream != null ) {
                        try {
                            using ( var reader = new StreamReader( dataStream ) ) {
                                var responseFromServer = reader.ReadToEnd();
                                return responseFromServer;
                            }
                        }
                        finally {
                            dataStream.Dispose();
                        }
                    }
                }
            }
            catch {
                throw new Exception( $"Unable to connect to {url}." );
            }

            return null;
        }

        [ItemCanBeNull]
        public static async Task<String> GetWebPageAsync( this Uri url ) {
            try {
                var request = WebRequest.Create( url );
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;
                using ( var response = await request.GetResponseAsync() ) {
                    using ( var dataStream = response.GetResponseStream() ) {
                        if ( dataStream != null ) {
                            using ( var reader = new StreamReader( dataStream ) ) {
                                var responseFromServer = reader.ReadToEnd();
                                return responseFromServer;
                            }
                        }
                    }
                }
            }
            catch {
                $"Unable to connect to {url}.".Error();
            }

            return null;
        }

        /// <summary>Convert a string to network bytes</summary>
        [NotNull]
        public static IEnumerable<Byte> ToNetworkBytes( [NotNull] this String data ) {
            var bytes = Encoding.UTF8.GetBytes( data );

            var len = IPAddress.HostToNetworkOrder( ( Int16 )bytes.Length );

            return BitConverter.GetBytes( len ).Concat( bytes );
        }
    }
}
