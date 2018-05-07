﻿// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/AES.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.Runtime.InteropServices;

    public class AES {
        private IntPtr _algHandle;
        private IntPtr _keyHandle;

        [DllImport( dllName: "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern UInt32 BCryptCloseAlgorithmProvider( [In] IntPtr phAlgorithm, [In] Int32 dwFlags );

        [DllImport( dllName: "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern UInt32 BCryptDecrypt( [In] [Out] IntPtr hKey, [In] Byte[] pbInput, [In] Int32 cbInput, [In] IntPtr pPaddingInfo, [In] Byte[] pbIV, [In] Int32 cbIV, [Out] Byte[] pbOutput, [In] Int32 cbOutput,
            [In] [Out] ref Int32 pcbResult, [In] Int32 dwFlags );

        [DllImport( dllName: "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern UInt32 BCryptEncrypt( [In] [Out] IntPtr hKey, [In] Byte[] pbInput, [In] Int32 cbInput, [In] IntPtr pPaddingInfo, [In] Byte[] pbIV, [In] Int32 cbIV, [Out] Byte[] pbOutput, [In] Int32 cbOutput,
            [In] [Out] ref Int32 pcbResult, [In] Int32 dwFlags );

        [DllImport( dllName: "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern UInt32 BCryptGenerateSymmetricKey( [In] IntPtr hAlgorithm, [In] [Out] ref IntPtr phKey, [Out] Byte[] pbKeyObject, [In] Int32 cbKeyObject, [In] Byte[] pbSecret, [In] Int32 cbSecret,
            [In] Int32 dwFlags );

        [DllImport( dllName: "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern UInt32 BCryptGetProperty( [In] IntPtr hObject, [In] String pszProperty, [Out] Byte[] pbOutput, [In] Int32 cbOutput, [In] [Out] ref Int32 pcbResult, [In] Int32 dwFlags );

        [DllImport( dllName: "Bcrypt.dll", CharSet = CharSet.Unicode, SetLastError = true )]
        public static extern UInt32 BCryptOpenAlgorithmProvider( [In] [Out] ref IntPtr phAlgorithm, [In] String pszAlgId, [In] String pszImplementation, [In] Int32 dwFlags );

        public UInt32 Close() {
            var status = BCryptCloseAlgorithmProvider( phAlgorithm: this._algHandle, dwFlags: 0 );
            return status;
        }

        public UInt32 Decrypt( Int32 pcbCipherText, Byte[] pbCipherText ) {

            //Initialize Initialization Vector
            Byte[] pbIV2 = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            //Initialize Plain Text Byte Count
            var pcbPlainText = 0;

            //Get Plain Text Byte Count
            BCryptDecrypt( hKey: this._keyHandle, pbInput: pbCipherText, cbInput: pcbCipherText, pPaddingInfo: IntPtr.Zero, pbIV: pbIV2, cbIV: pbIV2.Length, pbOutput: null, cbOutput: 0, pcbResult: ref pcbPlainText,
                dwFlags: 0 );

            //Allocate Plain Text Buffer
            var pbPlainText = new Byte[pcbPlainText];

            //Decrypt The Data
            var status = BCryptDecrypt( hKey: this._keyHandle, pbInput: pbCipherText, cbInput: pcbCipherText, pPaddingInfo: IntPtr.Zero, pbIV: pbIV2, cbIV: pbIV2.Length, pbOutput: pbPlainText,
                cbOutput: pbPlainText.Length, pcbResult: ref pcbPlainText, dwFlags: 0 );

            return status;
        }

        public UInt32 Encrypt( Byte[] pbData ) {

            //Initialize Data To Encrypt
            //Byte[] pbData = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            //Initialize Initialization Vector
            Byte[] pbIV = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F }; //16 bytes.

            //Initialize PaddingInfo

            //Initialize Cipher Text Byte Count
            var pcbCipherText = 0;

            //Get Cipher Text Byte Count
            BCryptEncrypt( hKey: this._keyHandle, pbInput: pbData, cbInput: pbData.Length, pPaddingInfo: IntPtr.Zero, pbIV: pbIV, cbIV: pbIV.Length, pbOutput: null, cbOutput: 0, pcbResult: ref pcbCipherText,
                dwFlags: 0 );

            //Allocate Cipher Text Buffer
            var pbCipherText = new Byte[pcbCipherText];

            //Encrypt The Data
            var status = BCryptEncrypt( hKey: this._keyHandle, pbInput: pbData, cbInput: pbData.Length, pPaddingInfo: IntPtr.Zero, pbIV: pbIV, cbIV: pbIV.Length, pbOutput: pbCipherText, cbOutput: pcbCipherText,
                pcbResult: ref pcbCipherText, dwFlags: 0 );

            return status;
        }

        public UInt32 Open() {

            //Open the Algorithm Provider

            //Initialize AlgHandle
            this._algHandle = IntPtr.Zero;

            //Initialize Status
            BCryptOpenAlgorithmProvider( phAlgorithm: ref this._algHandle, pszAlgId: "AES", pszImplementation: "Microsoft Primitive Provider", dwFlags: 0 );

            //Allocate DWORD for ObjectLength
            var pbObjectLength = new Byte[4];

            //Initialize ObjectLength Byte Count
            var pcbObjectLength = 0;

            //Get Algorithm Properties(BCRYPT_OBJECT_LENGTH)
            BCryptGetProperty( hObject: this._algHandle, pszProperty: "ObjectLength", pbOutput: pbObjectLength, cbOutput: pbObjectLength.Length, pcbResult: ref pcbObjectLength, dwFlags: 0 );

            //Initialize KeyHandle
            this._keyHandle = IntPtr.Zero;

            //Initialize Key Object Size with ObjectLength
            var keyObjectSize = ( pbObjectLength[3] << 24 ) | ( pbObjectLength[2] << 16 ) | ( pbObjectLength[1] << 8 ) | pbObjectLength[0];

            //Initialize AES Key
            Byte[] pbKey = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            //Allocate KeyObject With Key Object Size
            var pbKeyObject = new Byte[keyObjectSize];

            //Generate Symmetric Key Object
            var status = BCryptGenerateSymmetricKey( hAlgorithm: this._algHandle, phKey: ref this._keyHandle, pbKeyObject: pbKeyObject, cbKeyObject: keyObjectSize, pbSecret: pbKey, cbSecret: pbKey.Length, dwFlags: 0 );

            return status;
        }
    }
}