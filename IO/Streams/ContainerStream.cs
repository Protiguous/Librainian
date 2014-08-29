#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/ContainerStream.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.IO.Streams {
    using System;
    using System.IO;

    public abstract class ContainerStream : Stream {
        protected readonly Stream Stream;

        protected ContainerStream( Stream stream ) {
            if ( null == stream ) {
                throw new ArgumentNullException( "stream" );
            }
            this.Stream = stream;
        }

        protected Stream ContainedStream { get { return this.Stream; } }

        public override Boolean CanRead { get { return this.Stream.CanRead; } }

        public override Boolean CanSeek { get { return this.Stream.CanSeek; } }

        public override Boolean CanWrite { get { return this.Stream.CanWrite; } }

        public override long Length { get { return this.Stream.Length; } }

        public override long Position {
            get {
                //var str = this._stream as IsolatedStorageFileStream;
                //if ( null != str ) { return str.Position; }
                return this.Stream.Position;
            }
            set { this.Stream.Position = value; }
        }

        public override void Flush() {
            this.Stream.Flush();
        }

        public override int Read( byte[] buffer, int offset, int count ) {
            return this.Stream.Read( buffer, offset, count );
        }

        public override void Write( byte[] buffer, int offset, int count ) {
            this.Stream.Write( buffer, offset, count );
        }
    }
}