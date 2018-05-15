﻿// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ABetterClassDispose.cs" was last cleaned by Protiguous on 2018/05/14 at 6:41 PM

namespace Librainian.Magic {

    using System;
    using FluentAssertions;

    /// <summary>
    /// <para>A better class for implementing the <see cref="IDisposable"/> pattern.</para>
    /// <para>Implement <see cref="DisposeManaged"/> and <see cref="DisposeNative"/>.</para>
    /// </summary>
    /// <remarks>ABCD (hehe). Designed by Rick Harker</remarks>
    public class ABetterClassDispose : IDisposable {

        ~ABetterClassDispose() => this.Dispose();

        public Boolean HasDisposedManaged { get; private set; }

        public Boolean HasDisposedNative { get; private set; }

        public Boolean IsDisposed => this.HasDisposedManaged && this.HasDisposedNative;

        public void Dispose() {
            try {
                if ( !this.HasDisposedManaged ) {
                    try {
                        this.DisposeManaged();
                    }
                    catch ( Exception exception ) {
                        exception.Break();
                    }
                    finally {
                        this.HasDisposedManaged = true;
                    }
                }

                if ( !this.HasDisposedNative ) {
                    try {
                        this.DisposeNative();
                    }
                    catch ( Exception exception ) {
                        exception.Break();
                    }
                    finally {
                        this.HasDisposedNative = true;
                    }
                }
            }
            finally {
                this.HasDisposedManaged.Should().BeTrue();
                this.HasDisposedNative.Should().BeTrue();
                GC.SuppressFinalize( this );
            }
        }

        /// <summary>
        /// Dispose any disposable managed fields or properties.
        /// </summary>
        public virtual void DisposeManaged() {
            this.HasDisposedManaged = true; //yay or nay?
        }

        /// <summary>
        /// Dispose of COM objects, Handles, etc. Then set those objects to null if possible.
        /// </summary>
        public virtual void DisposeNative() {
            this.HasDisposedNative = true; //yay or nay?
        }
    }
}