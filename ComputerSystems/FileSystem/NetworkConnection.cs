// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "NetworkConnection.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "NetworkConnection.cs" was last formatted by Protiguous on 2018/06/04 at 3:47 PM.

namespace Librainian.ComputerSystems.FileSystem {

	using System;
	using System.ComponentModel;
	using System.Net;
	using System.Net.NetworkInformation;
	using System.Runtime.InteropServices;
	using System.Threading;
	using JetBrains.Annotations;
	using Measurement.Time;
	using OperatingSystem;

	public enum ResourceDisplaytype {

		Generic = 0x0,

		Domain = 0x01,

		Server = 0x02,

		Share = 0x03,

		File = 0x04,

		Group = 0x05,

		Network = 0x06,

		Root = 0x07,

		Shareadmin = 0x08,

		Directory = 0x09,

		Tree = 0x0a,

		Ndscontainer = 0x0b

	}

	public enum ResourceDisplayType {

		RESOURCEDISPLAYTYPE_GENERIC,

		RESOURCEDISPLAYTYPE_DOMAIN,

		RESOURCEDISPLAYTYPE_SERVER,

		RESOURCEDISPLAYTYPE_SHARE,

		RESOURCEDISPLAYTYPE_FILE,

		RESOURCEDISPLAYTYPE_GROUP,

		RESOURCEDISPLAYTYPE_NETWORK,

		RESOURCEDISPLAYTYPE_ROOT,

		RESOURCEDISPLAYTYPE_SHAREADMIN,

		RESOURCEDISPLAYTYPE_DIRECTORY,

		RESOURCEDISPLAYTYPE_TREE,

		RESOURCEDISPLAYTYPE_NDSCONTAINER

	}

	public enum ResourceScope {

		Connected = 1,

		GlobalNetwork,

		Remembered,

		Recent,

		Context

	}

	public enum ResourceType {

		Any = 0,

		Disk = 1,

		Print = 2,

		Reserved = 8

	}

	public enum ResourceUsage {

		RESOURCEUSAGE_CONNECTABLE = 0x00000001,

		RESOURCEUSAGE_CONTAINER = 0x00000002,

		RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,

		RESOURCEUSAGE_SIBLING = 0x00000008,

		RESOURCEUSAGE_ATTACHED = 0x00000010,

		RESOURCEUSAGE_ALL = RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED

	}

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public class NetResource {

		[MarshalAs( UnmanagedType.LPWStr )]
		public String Comment;

		public ResourceDisplaytype DisplayType;

		[MarshalAs( UnmanagedType.LPWStr )]
		public String LocalName;

		[MarshalAs( UnmanagedType.LPWStr )]
		public String Provider;

		[MarshalAs( UnmanagedType.LPWStr )]
		public String RemoteName;

		public ResourceType ResourceType;

		public ResourceScope Scope;

		public Int32 Usage;

	}

	[StructLayout( LayoutKind.Sequential )]
	public class NETRESOURCE {

		public ResourceDisplayType dwDisplayType = 0;

		public ResourceScope dwScope = 0;

		public ResourceType dwType = 0;

		public ResourceUsage dwUsage = 0;

		public String lpComment = null;

		public String lpLocalName = null;

		public String lpProvider = null;

		public String lpRemoteName = null;

	}

	public class NetworkConnection : IDisposable {

		public void Dispose() {
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		private String NetworkName { get; }

		~NetworkConnection() { this.Dispose( false ); }

		protected virtual void Dispose( Boolean disposing ) => NativeMethods.WNetCancelConnection2( this.NetworkName, 0, true );

		public static Boolean IsNetworkConnected( Int32 retries = 3 ) {
			var counter = retries;

			while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
				--counter;
				$"Network disconnected. Waiting {Seconds.One}. {counter} retries left...".WriteLine();
				Thread.Sleep( Seconds.One );
			}

			return NetworkInterface.GetIsNetworkAvailable();
		}

		public NetworkConnection( String networkName, [NotNull] NetworkCredential credentials ) {
			this.NetworkName = networkName;

			var netResource = new NetResource { Scope = ResourceScope.GlobalNetwork, ResourceType = ResourceType.Disk, DisplayType = ResourceDisplaytype.Share, RemoteName = networkName };

			var userName = String.IsNullOrEmpty( credentials.Domain ) ? credentials.UserName : $@"{credentials.Domain}\{credentials.UserName}";

			var result = NativeMethods.WNetAddConnection2( ref netResource, credentials.Password, userName, 0 );

			if ( result != 0 ) { throw new Win32Exception( result, "Error connecting to remote share" ); }
		}

	}

}