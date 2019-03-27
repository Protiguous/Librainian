﻿#region License & Information
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
// "Librainian/SimpleWallet.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.USD {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Annotations;
    using JetBrains.Annotations;

    /// <summary>
    ///     A simple, thread-safe,  System.Decimal-based wallet.
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class SimpleWallet : ISimpleWallet {
        [NotNull] private readonly ReaderWriterLockSlim _access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private  Decimal _balance;

        public SimpleWallet() => this.Timeout = TimeSpan.FromMinutes( 1 );

        [UsedImplicitly]
        public String Formatted => this.ToString();

        /// <summary>
        ///     <para>Timeout went reading or writing to the b<see cref="Balance" />.</para>
        ///     <para>Defaults to one minute.</para>
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public  Decimal Balance {
            get {
                try {
                    if ( !this._access.TryEnterReadLock( this.Timeout ) ) {
                        throw new TimeoutException( "Unable to get balance" );
                    }
                    return this._balance;
                }
                finally {
                    if ( this._access.IsReadLockHeld ) {
                        this._access.ExitReadLock();
                    }
                }
            }
            private set {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    throw new TimeoutException( "Unable to set the balance" );
                }
                this._balance = value;
                this._access.ExitWriteLock();
                var onAnyUpdate = this.OnAnyUpdate;
                onAnyUpdate?.Invoke( value );
            }
        }

        public Label LabelToFlashOnChanges { get; set; }

        public Action<Decimal > OnBeforeDeposit { get; set; }
        public Action<Decimal > OnAfterDeposit { get; set; }

        public Action<Decimal > OnBeforeWithdraw { get; set; }
        public Action<Decimal > OnAfterWithdraw { get; set; }

        public Action<Decimal > OnAnyUpdate { get; set; }

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        public Boolean TryAdd( Decimal amount, Boolean sanitize = true ) => throw new NotImplementedException();

        public Boolean TryAdd( Currency.SimpleWallet wallet, Boolean sanitize = true ) => throw new NotImplementedException();

        public Boolean TryDeposit(Decimal amount, Boolean sanitizeAmount = false ) {
            if ( amount <Decimal.Zero ) {
                return false;
            }
            try {
                var onBeforeDeposit = this.OnBeforeDeposit;
                onBeforeDeposit?.Invoke( amount );
                this.Balance += amount;
                return true;
            }
            finally {
                this.OnAfterDeposit( amount );
            }
        }

        public Boolean TryTransfer( Decimal amount, ref Currency.SimpleWallet intoWallet, Boolean sanitize = true ) => throw new NotImplementedException();

        public void TryUpdateBalance( Currency.SimpleWallet simpleWallet ) {
            throw new NotImplementedException();
        }

        public Boolean TryWithdraw(Decimal amount, Boolean sanitizeAmount = false ) {
            if ( amount <Decimal.Zero ) {
                return false;
            }
            try {
                var onBeforeWithdraw = this.OnBeforeWithdraw;
                onBeforeWithdraw?.Invoke( amount );
                if ( !this._access.TryEnterWriteLock( this.Timeout ) || this._balance < amount ) {
                    return false;
                }
                this._balance -= amount;
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }
                var onAfterWithdraw = this.OnAfterWithdraw;
                onAfterWithdraw?.Invoke( amount );
            }
        }

        public Boolean TryWithdraw( Currency.SimpleWallet wallet ) => throw new NotImplementedException();

        public Boolean TryUpdateBalance(Decimal amount, Boolean sanitizeAmount = true ) {
            try {
                if ( !this._access.TryEnterWriteLock( this.Timeout ) ) {
                    return false;
                }
                this._balance = amount;
                return true;
            }
            finally {
                if ( this._access.IsWriteLockHeld ) {
                    this._access.ExitWriteLock();
                }
                var onAnyUpdate = this.OnAnyUpdate;
                onAnyUpdate?.Invoke( amount );
            }
        }

        public override String ToString() => this.Balance.ToString( "C" );

    }
}
