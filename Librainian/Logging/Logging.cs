// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Logging.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Logging.cs" was last formatted by Protiguous on 2019/08/08 at 8:13 AM.

namespace Librainian.Logging {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime;
    using System.Threading;
    using System.Windows.Forms;
    using CommandLine;
    using Extensions;
    using JetBrains.Annotations;
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;
    using NLog.Windows.Forms;
    using Persistence;

    public static class Logging {

        [NotNull]
        public static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        [NotNull]
        public static ConcurrentDictionary<Control, Target> Targets { get; } = new ConcurrentDictionary<Control, Target>();

        /// <summary>
        ///     Add a <see cref="Target" /> or a <see cref="RichTextBox" />.
        /// </summary>
        /// <param name="minLogLevel"></param>
        /// <param name="maxLogLevel"></param>
        /// <param name="target">     </param>
        /// <param name="rtb">        </param>
        [DebuggerStepThrough]
        public static Boolean Setup( LogLevel minLogLevel, LogLevel maxLogLevel, Target target = null, [CanBeNull] RichTextBox rtb = null ) {

            switch ( target ) {
                case null when rtb is null:
                    "No target or control specified to setup!".Break();

                    return false;

                case null:
                    target = rtb.ToTarget();

                    break;
            }

            if ( !( target is null ) ) {
                LogManager.Configuration.AddTarget( target );
                LogManager.Configuration.AddRule( minLevel: minLogLevel, maxLevel: maxLogLevel, target: target, loggerNamePattern: "*" );

                return LogManager.Configuration.AllTargets.Contains( target );
            }

            $"Unable to set up target for {minLogLevel} to {maxLogLevel}".Break();

            return false;
        }

        /// <summary>
        ///     <para>Prints the <paramref name="message" /></para>
        ///     <para>Then calls <see cref="Debugger.Break" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <param name="message"></param>
        [CanBeNull]
        [DebuggerStepThrough]
        public static String Break<T>( this T _, [CanBeNull] String message = null ) {
            if ( !String.IsNullOrEmpty( message ) ) {
                message.Debug();
            }

            _.BreakIfDebug();

            return message;
        }

        [DebuggerStepThrough]
        public static void BreakIfFalse( this Boolean condition, [CanBeNull] String message = null ) {
            if ( !condition ) {
                Break( message );
            }
        }

        [DebuggerStepThrough]
        public static (Color fore, Color back) Colors( this LoggingLevel loggingLevel ) {

            switch ( loggingLevel ) {
                case LoggingLevel.Divine: {
                    return ( Color.Blue, Color.Aqua );
                }

                case LoggingLevel.SubspaceTear: {
                    return ( Color.HotPink, Color.Aqua ); //hotpink might actually look okay..
                }

                case LoggingLevel.Fatal: {

                    return ( Color.DarkRed, Color.Aqua );
                }

                case LoggingLevel.Critical: {

                    return ( Color.Red, Color.Aqua );
                }

                case LoggingLevel.Error: {

                    return ( Color.Red, Color.White );
                }

                case LoggingLevel.Warning: {

                    return ( Color.Goldenrod, Color.White );
                }

                case LoggingLevel.Diagnostic: {

                    return ( Color.Green, Color.White );
                }

                case LoggingLevel.Debug: {

                    return ( Color.DarkSeaGreen, Color.White );
                }

                case LoggingLevel.Exception: {

                    return ( Color.DarkOliveGreen, Color.AntiqueWhite );
                }

                default: throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null );
            }
        }

        [DebuggerStepThrough]
        public static void Debug<T>( this T obj ) => Logger.Debug( obj );

        //[DebuggerStepThrough]public static void Info<T>( this T obj ) => Logger.Info( obj );

        [DebuggerStepThrough]
        public static void Error<T>( this T obj ) => Logger.Error( obj );

        [DebuggerStepThrough]
        public static void Fatal<T>( this T obj ) => Logger.Fatal( obj );

        [DebuggerStepThrough]
        public static void Info( [CanBeNull] this String message ) {
            if ( !String.IsNullOrEmpty( message ) ) {
                Logger.Info( message );
            }
        }

        [DebuggerStepThrough]
        [NotNull]
        public static String LevelName( this LoggingLevel loggingLevel ) {
            switch ( loggingLevel ) {
                case LoggingLevel.Diagnostic: return nameof( LoggingLevel.Diagnostic );
                case LoggingLevel.Debug: return nameof( LoggingLevel.Debug );
                case LoggingLevel.Warning: return nameof( LoggingLevel.Warning );
                case LoggingLevel.Error: return nameof( LoggingLevel.Error );
                case LoggingLevel.Exception: return nameof( LoggingLevel.Exception );
                case LoggingLevel.Critical: return nameof( LoggingLevel.Critical );
                case LoggingLevel.Fatal: return nameof( LoggingLevel.Fatal );
                case LoggingLevel.SubspaceTear: return nameof( LoggingLevel.SubspaceTear );
                case LoggingLevel.Divine: return nameof( LoggingLevel.Divine );
                default: throw new ArgumentOutOfRangeException( nameof( loggingLevel ), loggingLevel, null );
            }
        }

        [Conditional("DEBUG")]
        [Conditional("TRACE")]
        [DebuggerStepThrough]
        public static void Log( this String message, Boolean breakinto = false ) {
            if ( Logger.IsDebugEnabled ) {
                System.Diagnostics.Debug.WriteLine( $"[{DateTime.Now:t}] {message}" );
                Logger.Debug( message );

                if ( breakinto && Debugger.IsAttached ) {
                    Debugger.Break();
                }
            }

            if ( Logger.IsTraceEnabled ) {
                Logger.Trace( message );
            }
        }

        [DebuggerStepThrough]
        public static void Log( this Exception exception, Boolean breakinto = false ) {
            System.Diagnostics.Debug.WriteLine( exception );
            Logger.Debug( exception );

            if ( breakinto && Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        [DebuggerStepThrough]
        public static T Log<T>( this T message, Boolean breakinto ) {
            Logger.Debug( message );

            if ( breakinto && Debugger.IsAttached ) {
                Debugger.Break();
            }

            return message;
        }

        /// <summary>
        ///     Write
        ///     <param name="object"></param>
        ///     as JSON to the <see cref="Logger" />.
        ///     <para>Append <paramref name="more" /> if it has text.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="object"></param>
        /// <param name="more"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T Log<T, M>( this T @object, [CanBeNull] M more ) {
            Console.Beep( 14000, 100 );
            var o = $"{@object.ToJSON()}";

            if ( more == null ) {
                Logger.Debug( o );
                if ( Debugger.IsAttached ) {
                    System.Diagnostics.Debug.WriteLine( $"Error=\"{@object}\"" );
                    Debugger.Break();
                }
            }
            else {
                var m = more.ToJSON();
                Logger.Debug( $"{o}; {m}" );
                if ( Debugger.IsAttached ) {
                    System.Diagnostics.Debug.WriteLine( $"Error=\"{@object}\"; {m}" );
                    Debugger.Break();
                }
            }

            return @object;
        }

        [DebuggerStepThrough]
        public static void Trace( this String message ) => Logger.Trace( message );

        [DebuggerStepThrough]
        public static void Trace( this Exception exception ) => Logger.Trace( exception );

        [DebuggerStepThrough]
        public static void Trace<T>( this T message ) => Logger.Trace( message );

        [DebuggerStepThrough]
        public static void Warn( this String message ) => Logger.Warn( message );

        [DebuggerStepThrough]
        public static void Warn( this Exception exception ) => Logger.Warn( exception );

        [DebuggerStepThrough]
        public static void Warn<T>( this T message ) => Logger.Warn( message );

        [DebuggerStepThrough]
        [CanBeNull]
        public static Target ToTarget( [CanBeNull] this RichTextBox rtb ) {
            if ( rtb is null ) {

                //throw new ArgumentNullException(nameof( rtb ),"The paramter 'rtb' was null." );
                return null;
            }

            if ( String.IsNullOrWhiteSpace( rtb.Name ) ) {
                throw new ArgumentNullException( nameof( rtb ), "No name given on this RichTextBox control." );
            }

            if ( Targets.TryGetValue( rtb, out var toTarget ) ) {
                return toTarget;
            }

            var controlName = rtb.Name.Trim();

            if ( LogManager.Configuration.FindTargetByName( controlName ) is RichTextBoxTarget target ) {
                Targets[ rtb ] = target;

                return target;
            }

            var form = rtb.FindForm();
            var formName = form?.Name ?? "Form_" + rtb.Name;

            target = new RichTextBoxTarget {
                Name = controlName,
                Layout = new CsvLayout {
                    WithHeader = false,
                    Delimiter = CsvColumnDelimiterMode.Tab,
                    Columns = {
                        new CsvColumn( "Time", layout: "${longdate}" ), new CsvColumn( "Message", layout: "${message}" )
                    }
                },
                ControlName = controlName,
                AutoScroll = true,
                FormName = formName,
                MaxLines = 256,
                UseDefaultRowColoringRules = true,
                AllowAccessoryFormCreation = true,
                MessageRetention = RichTextBoxTargetMessageRetentionStrategy.All,
                ShowMinimized = false,
                SupportLinks = true,
                TargetForm = form,
                TargetRichTextBox = rtb,
                ToolWindow = true
            };

            Targets[ rtb ] = target;

            return target;
        }

        /// <summary>
        /// <para>Adds program-wide exception handlers.</para>
        /// <para>Optimizes program startup.</para>
        /// <para>Starts logging, Debug and Trace.</para>
        /// <para>Performs a garbage cleanup.</para>
        /// <para>And start the form thread-loop.</para>
        /// </summary>
        public static void Run<TOpts>( [NotNull] Action<TOpts> run, [NotNull] IEnumerable<String> arguments ) {

            if ( run == null ) {
                throw new ArgumentNullException( paramName: nameof( run ) );
            }

            if ( arguments == null ) {
                throw new ArgumentNullException( paramName: nameof( arguments ) );
            }

            try {
                RunInternalCommon();

                Parser.Default.ParseArguments<TOpts>( arguments ).WithParsed( run ).WithNotParsed( HandleErrors );

            }
            catch ( Exception exception ) {
                exception.Log( breakinto: true );
            }

            void HandleErrors( IEnumerable<CommandLine.Error> errors ) {
                try {
                    if ( errors != null ) {
                        foreach ( var error in errors ) {
                            error.ToString().Log( breakinto: true );
                        }
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }
        }

        private static void RunInternalCommon() {
            AppDomain.CurrentDomain.UnhandledException += ( sender, e ) => ( e?.ExceptionObject as Exception ).Log( breakinto: true );
            Application.ThreadException += ( sender, e ) => e?.Exception.Log( breakinto: true );

            ProfileOptimization.SetProfileRoot( Application.ExecutablePath );
            ProfileOptimization.StartProfile( Application.ExecutablePath );

            System.Diagnostics.Debug.AutoFlush = true;
            System.Diagnostics.Trace.AutoFlush = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( defaultValue: false );

            Thread.CurrentThread.Name = "UI";

            InternalLogger.Reset();

            if ( LogManager.Configuration is null ) {
                InternalLogger.Trace( $"{nameof( Run )} creating a new {nameof( LoggingConfiguration )}..." );
                LogManager.Configuration = new LoggingConfiguration();
                InternalLogger.Trace( $"{nameof( Run )} created a new {nameof( LoggingConfiguration )}." );
            }

            Setup( LogLevel.Trace, LogLevel.Fatal, SomeTargets.TraceTarget.Value );

#if DEBUG
            LogManager.ThrowConfigExceptions = true;
            LogManager.ThrowExceptions = true;
#endif

            Compact();
            Thread.Yield();
            Compact();

            void Compact() {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect( 2, GCCollectionMode.Forced, true, true );
            }
        }

        public static void Run<T>() where T : Form, new() {
            RunInternalCommon();

            using ( var form = new T() ) {
                Application.Run( form );
            }
        }

    }
}