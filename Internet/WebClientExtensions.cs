﻿// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/WebClientExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///     <para>Extension methods for working with WebClient asynchronously.</para>
    ///     <para>Copyright (c) Microsoft Corporation. All rights reserved.</para>
    /// </summary>
    public static class WebClientExtensions {

        /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task<Byte[]> DownloadDataTask( this WebClient webClient, String address ) => DownloadDataTask( webClient, new Uri( address ) );

        /// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task<Byte[]> DownloadDataTask( this WebClient webClient, Uri address ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Byte[]>( address );

            // Setup the callback event handler
            DownloadDataCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.DownloadDataCompleted -= handler );
            webClient.DownloadDataCompleted += handler;

            // Start the async work
            try {
                webClient.DownloadDataAsync( address, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.DownloadDataCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Downloads the resource with the specified URI to a local file, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <param name="fileName">The name of the local file that is to receive the data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task DownloadFileTask( this WebClient webClient, String address, String fileName ) => DownloadFileTask( webClient, new Uri( address ), fileName );

        /// <summary>Downloads the resource with the specified URI to a local file, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <param name="fileName">The name of the local file that is to receive the data.</param>
        /// <returns>A Task that contains the downloaded data.</returns>
        public static Task DownloadFileTask( this WebClient webClient, Uri address, String fileName ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Object>( address );

            // Setup the callback event handler
            AsyncCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => null, () => webClient.DownloadFileCompleted -= handler );
            webClient.DownloadFileCompleted += handler;

            // Start the async work
            try {
                webClient.DownloadFileAsync( address, fileName, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.DownloadFileCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Downloads the resource with the specified URI as a String, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded String.</returns>
        public static Task<String> DownloadStringTask( this WebClient webClient, String address ) => DownloadStringTask( webClient, new Uri( address ) );

        /// <summary>Downloads the resource with the specified URI as a String, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI from which to download data.</param>
        /// <returns>A Task that contains the downloaded String.</returns>
        public static Task<String> DownloadStringTask( this WebClient webClient, Uri address ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<String>( address );

            // Setup the callback event handler
            DownloadStringCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.DownloadStringCompleted -= handler );
            webClient.DownloadStringCompleted += handler;

            // Start the async work
            try {
                webClient.DownloadStringAsync( address, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.DownloadStringCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenReadTask( this WebClient webClient, String address ) => OpenReadTask( webClient, new Uri( address ) );

        /// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenReadTask( this WebClient webClient, Uri address ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Stream>( address );

            // Setup the callback event handler
            OpenReadCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.OpenReadCompleted -= handler );
            webClient.OpenReadCompleted += handler;

            // Start the async work
            try {
                webClient.OpenReadAsync( address, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.OpenReadCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <param name="method">The HTTP method that should be used to open the stream.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenWriteTask( this WebClient webClient, String address, String method ) => OpenWriteTask( webClient, new Uri( address ), method );

        /// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI for which the stream should be opened.</param>
        /// <param name="method">The HTTP method that should be used to open the stream.</param>
        /// <returns>A Task that contains the opened stream.</returns>
        public static Task<Stream> OpenWriteTask( this WebClient webClient, Uri address, String method ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Stream>( address );

            // Setup the callback event handler
            OpenWriteCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.OpenWriteCompleted -= handler );
            webClient.OpenWriteCompleted += handler;

            // Start the async work
            try {
                webClient.OpenWriteAsync( address, method, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.OpenWriteCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Uploads data to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadDataTask( this WebClient webClient, String address, String method, Byte[] data ) => UploadDataTask( webClient, new Uri( address ), method, data );

        /// <summary>Uploads data to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadDataTask( this WebClient webClient, Uri address, String method, Byte[] data ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Byte[]>( address );

            // Setup the callback event handler
            UploadDataCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.UploadDataCompleted -= handler );
            webClient.UploadDataCompleted += handler;

            // Start the async work
            try {
                webClient.UploadDataAsync( address, method, data, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.UploadDataCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Uploads a file to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the file should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the file.</param>
        /// <param name="fileName">A path to the file to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadFileTask( this WebClient webClient, String address, String method, String fileName ) => UploadFileTask( webClient, new Uri( address ), method, fileName );

        /// <summary>Uploads a file to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the file should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the file.</param>
        /// <param name="fileName">A path to the file to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<Byte[]> UploadFileTask( this WebClient webClient, Uri address, String method, String fileName ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<Byte[]>( address );

            // Setup the callback event handler
            UploadFileCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.UploadFileCompleted -= handler );
            webClient.UploadFileCompleted += handler;

            // Start the async work
            try {
                webClient.UploadFileAsync( address, method, fileName, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.UploadFileCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }

        /// <summary>Uploads data in a String to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<String> UploadStringTask( this WebClient webClient, String address, String method, String data ) => UploadStringTask( webClient, new Uri( address ), method, data );

        /// <summary>Uploads data in a String to the specified resource, asynchronously.</summary>
        /// <param name="webClient">The WebClient.</param>
        /// <param name="address">The URI to which the data should be uploaded.</param>
        /// <param name="method">The HTTP method that should be used to upload the data.</param>
        /// <param name="data">The data to upload.</param>
        /// <returns>A Task containing the data in the response from the upload.</returns>
        public static Task<String> UploadStringTask( this WebClient webClient, Uri address, String method, String data ) {

            // Create the task to be returned
            var tcs = new TaskCompletionSource<String>( address );

            // Setup the callback event handler
            UploadStringCompletedEventHandler handler = null;
            handler = ( sender, e ) => EapCommon.HandleCompletion( tcs, e, () => e.Result, () => webClient.UploadStringCompleted -= handler );
            webClient.UploadStringCompleted += handler;

            // Start the async work
            try {
                webClient.UploadStringAsync( address, method, data, tcs );
            }
            catch ( Exception exc ) {

                // If something goes wrong kicking off the async work, unregister the callback and
                // cancel the created task
                webClient.UploadStringCompleted -= handler;
                tcs.TrySetException( exc );
            }

            // Return the task that represents the async operation
            return tcs.Task;
        }
    }
}