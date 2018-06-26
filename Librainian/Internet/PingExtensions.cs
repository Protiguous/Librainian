﻿// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "PingExtensions.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "PingExtensions.cs" was last formatted by Protiguous on 2018/06/26 at 1:11 AM.

namespace Librainian.Internet {

	using System;
	using System.Net;
	using System.Net.NetworkInformation;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	/// <summary>
	///     <para>Extension methods for working with Ping asynchronously.</para>
	///     <para></para>
	/// </summary>
	public static class PingExtensions {

		/// <summary>The core implementation of SendTask.</summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <param name="sendAsync">
		///     A delegate that initiates the asynchronous send. The provided TaskCompletionSource must
		///     be passed as the user-supplied state to the actual Ping.SendAsync method.
		/// </param>
		/// <returns></returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		private static Task<PingReply> SendTaskCore( [NotNull] Ping ping, Object userToken, Action<TaskCompletionSource<PingReply>> sendAsync ) {

			// Validate we're being used with a real smtpClient. The rest of the arg validation will
			// happen in the call to sendAsync.
			if ( ping is null ) {
				throw new ArgumentNullException( nameof( ping ) );
			}

			// Create a TaskCompletionSource to represent the operation
			var tcs = new TaskCompletionSource<PingReply>( userToken );

			// Register a handler that will transfer completion results to the TCS Task
			void Handler( Object sender, PingCompletedEventArgs e ) => tcs.HandleCompletion( e, () => e.Reply, () => ping.PingCompleted -= Handler );

			ping.PingCompleted += Handler;

			// Try to start the async operation. If starting it fails (due to parameter validation)
			// unregister the handler before allowing the exception to propagate.
			try {
				sendAsync( tcs );
			}
			catch ( Exception exc ) {
				ping.PingCompleted -= Handler;
				tcs.TrySetException( exc );
			}

			// Return the task to represent the asynchronous operation
			return tcs.Task;
		}

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="address">
		///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, IPAddress address, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="hostNameOrAddress">
		///     A String that identifies the computer that is the destination for the ICMP echo message.
		///     The value specified for this parameter can be a host name or a String representation of
		///     an IP address.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, String hostNameOrAddress, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="address">
		///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
		/// </param>
		/// <param name="timeout">
		///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
		///     message) to wait for the ICMP echo reply message.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, IPAddress address, Int32 timeout, Object userToken ) => SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, timeout, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="hostNameOrAddress">
		///     A String that identifies the computer that is the destination for the ICMP echo message.
		///     The value specified for this parameter can be a host name or a String representation of
		///     an IP address.
		/// </param>
		/// <param name="timeout">
		///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
		///     message) to wait for the ICMP echo reply message.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, String hostNameOrAddress, Int32 timeout, Object userToken ) =>
			SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, timeout, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="address">
		///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
		/// </param>
		/// <param name="timeout">
		///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
		///     message) to wait for the ICMP echo reply message.
		/// </param>
		/// <param name="buffer">
		///     A Byte array that contains data to be sent with the ICMP echo message and returned in
		///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, IPAddress address, Int32 timeout, Byte[] buffer, Object userToken ) =>
			SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, timeout, buffer, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="hostNameOrAddress">
		///     A String that identifies the computer that is the destination for the ICMP echo message.
		///     The value specified for this parameter can be a host name or a String representation of
		///     an IP address.
		/// </param>
		/// <param name="timeout">
		///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
		///     message) to wait for the ICMP echo reply message.
		/// </param>
		/// <param name="buffer">
		///     A Byte array that contains data to be sent with the ICMP echo message and returned in
		///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, String hostNameOrAddress, Int32 timeout, Byte[] buffer, Object userToken ) =>
			SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, timeout, buffer, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="address">
		///     An IPAddress that identifies the computer that is the destination for the ICMP echo message.
		/// </param>
		/// <param name="timeout">
		///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
		///     message) to wait for the ICMP echo reply message.
		/// </param>
		/// <param name="buffer">
		///     A Byte array that contains data to be sent with the ICMP echo message and returned in
		///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
		/// </param>
		/// <param name="options">
		///     A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP
		///     echo message packet.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, IPAddress address, Int32 timeout, Byte[] buffer, PingOptions options, Object userToken ) =>
			SendTaskCore( ping, userToken, tcs => ping.SendAsync( address, timeout, buffer, options, tcs ) );

		/// <summary>
		///     Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message.
		/// </summary>
		/// <param name="ping">The Ping.</param>
		/// <param name="hostNameOrAddress">
		///     A String that identifies the computer that is the destination for the ICMP echo message.
		///     The value specified for this parameter can be a host name or a String representation of
		///     an IP address.
		/// </param>
		/// <param name="timeout">
		///     An Int32 value that specifies the maximum number of milliseconds (after sending the echo
		///     message) to wait for the ICMP echo reply message.
		/// </param>
		/// <param name="buffer">
		///     A Byte array that contains data to be sent with the ICMP echo message and returned in
		///     the ICMP echo reply message. The array cannot contain more than 65,500 bytes.
		/// </param>
		/// <param name="options">
		///     A PingOptions object used to control fragmentation and Time-to-Live values for the ICMP
		///     echo message packet.
		/// </param>
		/// <param name="userToken">A user-defined object stored in the resulting Task.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
		public static Task<PingReply> SendTask( [NotNull] this Ping ping, String hostNameOrAddress, Int32 timeout, Byte[] buffer, PingOptions options, Object userToken ) =>
			SendTaskCore( ping, userToken, tcs => ping.SendAsync( hostNameOrAddress, timeout, buffer, options, tcs ) );
	}
}