﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AbandonableTask.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "AbandonableTask.cs" was last formatted by Protiguous on 2020/02/09 at 2:02 PM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary></summary>
    /// <see cref="http://stackoverflow.com/a/4749401/956364" />
    public sealed class AbandonableTask {

        [NotNull]
        private Action _beginWork { get; }

        [NotNull]
        private Action _blockingWork { get; }

        private CancellationToken _cancellationToken { get; }

        public Action<Task> AfterComplete { get; }

        private AbandonableTask( [NotNull] Action beginWork, [NotNull] Action blockingWork, [CanBeNull] Action<Task> afterComplete, CancellationToken cancellationToken ) {
            this._cancellationToken = cancellationToken;
            this._beginWork = beginWork ?? throw new ArgumentNullException( nameof( beginWork ) );
            this._blockingWork = blockingWork ?? throw new ArgumentNullException( nameof( blockingWork ) );
            this.AfterComplete = afterComplete;
        }

        private void RunTask() {
            this._beginWork.Invoke();

            var innerTask = new Task( this._blockingWork, this._cancellationToken, TaskCreationOptions.LongRunning );
            innerTask.Start();

            innerTask.Wait( this._cancellationToken );

            if ( innerTask.IsCompleted ) {
                this.AfterComplete?.Invoke( innerTask );
            }
        }

        [NotNull]
        public static Task Start( [NotNull] Action blockingWork, [NotNull] Action beginWork, [NotNull] Action<Task> afterComplete, CancellationToken cancellationToken ) {
            if ( blockingWork is null ) {
                throw new ArgumentNullException( nameof( blockingWork ) );
            }

            if ( beginWork is null ) {
                throw new ArgumentNullException( paramName: nameof( beginWork ) );
            }

            if ( afterComplete is null ) {
                throw new ArgumentNullException( paramName: nameof( afterComplete ) );
            }

            var worker = new AbandonableTask( beginWork, blockingWork, afterComplete, cancellationToken );
            var outerTask = new Task( worker.RunTask, cancellationToken );
            outerTask.Start();

            return outerTask;
        }

    }

}