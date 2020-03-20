﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DetectSSD.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "DetectSSD.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.OperatingSystem {

    using System;
    using System.Runtime.InteropServices;

    public static class DetectSSD {

        /// <summary>Returns true if the disk/drive has seek penalty.</summary>
        /// <param name="diskNumber"></param>
        /// <returns></returns>
        public static Boolean? IncursSeekPenalty( this Byte diskNumber ) {
            var sDrive = @"\\.\PhysicalDrive" + diskNumber;

            var hDrive = NativeMethods.CreateFileW( lpFileName: sDrive, dwDesiredAccess: 0, // No access to drive
                dwShareMode: NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, lpSecurityAttributes: IntPtr.Zero,
                dwCreationDisposition: NativeMethods.OPEN_EXISTING, dwFlagsAndAttributes: NativeMethods.FILE_ATTRIBUTE_NORMAL, hTemplateFile: IntPtr.Zero );

            if ( hDrive.IsInvalid ) {
                //Debug.WriteLine( "CreateFile failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE( deviceType: NativeMethods.IOCTL_STORAGE_BASE, function: 0x500, method: NativeMethods.METHOD_BUFFERED,
                access: NativeMethods.FILE_ANY_ACCESS ); // From winioctl.h

            var query_seek_penalty = new NativeMethods.STORAGE_PROPERTY_QUERY {
                PropertyId = NativeMethods.StorageDeviceSeekPenaltyProperty, QueryType = NativeMethods.PropertyStandardQuery
            };

            var query_seek_penalty_desc = new NativeMethods.DEVICE_SEEK_PENALTY_DESCRIPTOR();

            var querySeekPenaltyResult = NativeMethods.DeviceIoControl( hDevice: hDrive, dwIoControlCode: IOCTL_STORAGE_QUERY_PROPERTY, lpInBuffer: ref query_seek_penalty,
                nInBufferSize: ( UInt32 ) Marshal.SizeOf( structure: query_seek_penalty ), lpOutBuffer: ref query_seek_penalty_desc,
                nOutBufferSize: ( UInt32 ) Marshal.SizeOf( structure: query_seek_penalty_desc ), lpBytesReturned: out var returned_query_seek_penalty_size,
                lpOverlapped: IntPtr.Zero );

            hDrive.Close();

            if ( !querySeekPenaltyResult ) {
                //Debug.WriteLine( "DeviceIoControl failed: " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            return query_seek_penalty_desc.IncursSeekPenalty;
        }

        /// <summary>Method for nominal media rotation rate (Administrative privilege is required)</summary>
        /// <param name="diskNumber"></param>
        public static Boolean? IsRotateDevice( this Byte diskNumber ) {
            var sDrive = @"\\.\PhysicalDrive" + diskNumber;

            var hDrive = NativeMethods.CreateFileW( lpFileName: sDrive,
                dwDesiredAccess: NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, // Administrative privilege is required
                dwShareMode: NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, lpSecurityAttributes: IntPtr.Zero,
                dwCreationDisposition: NativeMethods.OPEN_EXISTING, dwFlagsAndAttributes: NativeMethods.FILE_ATTRIBUTE_NORMAL, hTemplateFile: IntPtr.Zero );

            if ( hDrive.IsInvalid ) {
                //Debug.WriteLine( "CreateFile failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            var ioctlAtaPassThrough = CTL_CODE( deviceType: NativeMethods.IOCTL_SCSI_BASE, function: 0x040b, method: NativeMethods.METHOD_BUFFERED,
                access: NativeMethods.FILE_READ_ACCESS | NativeMethods.FILE_WRITE_ACCESS ); // From ntddscsi.h

            var idQuery = new NativeMethods.ATAIdentifyDeviceQuery {
                data = new UInt16[ 256 ]
            };

            idQuery.header.Length = ( UInt16 ) Marshal.SizeOf( structure: idQuery.header );
            idQuery.header.AtaFlags = ( UInt16 ) NativeMethods.ATA_FLAGS_DATA_IN;
            idQuery.header.DataTransferLength = ( UInt32 ) ( idQuery.data.Length * 2 ); // Size of "data" in bytes
            idQuery.header.TimeOutValue = 3;                                            // Sec
            idQuery.header.DataBufferOffset = Marshal.OffsetOf( t: typeof( NativeMethods.ATAIdentifyDeviceQuery ), fieldName: "data" );
            idQuery.header.PreviousTaskFile = new Byte[ 8 ];
            idQuery.header.CurrentTaskFile = new Byte[ 8 ];
            idQuery.header.CurrentTaskFile[ 6 ] = 0xec; // ATA IDENTIFY DEVICE

            var result = NativeMethods.DeviceIoControl( hDevice: hDrive, dwIoControlCode: ioctlAtaPassThrough, lpInBuffer: ref idQuery,
                nInBufferSize: ( UInt32 ) Marshal.SizeOf( structure: idQuery ), lpOutBuffer: ref idQuery, nOutBufferSize: ( UInt32 ) Marshal.SizeOf( structure: idQuery ),
                lpBytesReturned: out var retvalSize, lpOverlapped: IntPtr.Zero );

            hDrive.Close();

            if ( !result ) {
                //Debug.WriteLine( "DeviceIoControl failed. " + NativeMethods.GetErrorMessage( Marshal.GetLastWin32Error() ) );
                return null;
            }

            // Word index of nominal media rotation rate
            const Int32 kNominalMediaRotRateWordIndex = 217;
            const Int32 nonRotateDevice = 1;

            if ( idQuery.data[ kNominalMediaRotRateWordIndex ] == nonRotateDevice ) {
                //Debug.WriteLine( $"The disk #{diskNumber} is a NON-ROTATE device." );
                return false;
            }

            //Debug.WriteLine( "This disk is ROTATE device." );
            return true;
        }

        public static Boolean? IsDiskSSD( this Byte diskNumber ) => diskNumber.IncursSeekPenalty() == false ? ( Boolean? ) true : null;

        private static UInt32 CTL_CODE( UInt32 deviceType, UInt32 function, UInt32 method, UInt32 access ) =>
            ( deviceType << 16 ) | ( access << 14 ) | ( function << 2 ) | method;

    }

}