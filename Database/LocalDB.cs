﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "LocalDB.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has been
// overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/LocalDB.cs" was last cleaned by Protiguous on 2018/05/15 at 1:34 AM.

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using FileSystem;
    using JetBrains.Annotations;
    using Magic;

    public class LocalDb : ABetterClassDispose {

        /// <summary>
        /// </summary>
        /// <param name="databaseName">    </param>
        /// <param name="databaseLocation"></param>
        /// <param name="timeoutForReads"> </param>
        /// <param name="timeoutForWrites"></param>
        // ReSharper disable once NotNullMemberIsNotInitialized
        [SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
        public LocalDb( [NotNull] String databaseName, [CanBeNull] Folder databaseLocation = null, TimeSpan? timeoutForReads = null, TimeSpan? timeoutForWrites = null ) {
            if ( String.IsNullOrWhiteSpace( databaseName ) ) { throw new ArgumentNullException( nameof( databaseName ) ); }

            if ( databaseLocation is null ) { databaseLocation = new Folder( Environment.SpecialFolder.LocalApplicationData, Application.ProductName ); }

            this.ReadTimeout = timeoutForReads.GetValueOrDefault( TimeSpan.FromMinutes( 1 ) );
            this.WriteTimeout = timeoutForWrites.GetValueOrDefault( TimeSpan.FromMinutes( 1 ) );

            this.DatabaseName = databaseName;

            this.DatabaseLocation = databaseLocation;

            if ( !this.DatabaseLocation.Exists() ) {
                this.DatabaseLocation.Create();
                this.DatabaseLocation.Info.SetCompression( false );
            }

            "Building SQL connection string...".Info();

            this.DatabaseMdf = new Document( this.DatabaseLocation, $"{this.DatabaseName}.mdf" );
            this.DatabaseLog = new Document( this.DatabaseLocation, $"{this.DatabaseName}_log.ldf" ); //TODO does localdb even use a log file?

            this.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=master;Integrated Security=True;";

            if ( !this.DatabaseMdf.Exists() ) {
                using ( var connection = new SqlConnection( this.ConnectionString ) ) {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = String.Format( "CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", this.DatabaseName, this.DatabaseMdf.FullPathWithFileName );
                    command.ExecuteNonQuery();
                }
            }

            this.ConnectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog={this.DatabaseName};AttachDBFileName={this.DatabaseMdf.FullPathWithFileName};";

            // ReSharper disable once UseObjectOrCollectionInitializer
            this.Connection = new SqlConnection( this.ConnectionString );
            this.Connection.InfoMessage += ( sender, args ) => args.Message.Info();
            this.Connection.StateChange += ( sender, args ) => $"{args.OriginalState} -> {args.CurrentState}".Info();
            this.Connection.Disposed += ( sender, args ) => $"Disposing SQL connection {args}".Info();

            $"Attempting connection to {this.DatabaseMdf}...".Info();
            this.Connection.Open();
            this.Connection.ServerVersion.Info();
            this.Connection.Close();
        }

        [NotNull]
        public SqlConnection Connection { get; }

        [NotNull]
        public String ConnectionString { get; }

        [NotNull]
        public Folder DatabaseLocation { get; }

        [NotNull]
        public Document DatabaseLog { get; }

        [NotNull]
        public Document DatabaseMdf { get; }

        [NotNull]
        public String DatabaseName { get; }

        public TimeSpan ReadTimeout { get; }

        public TimeSpan WriteTimeout { get; }

        public async Task DetachDatabaseAsync() {
            try {
                if ( this.Connection.State == ConnectionState.Closed ) { await this.Connection.OpenAsync(); }

                using ( var cmd = this.Connection.CreateCommand() ) {
                    cmd.CommandText = String.Format( "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db N'{0}'", this.DatabaseName );
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch ( SqlException exception ) { exception.More(); }
            catch ( DbException exception ) { exception.More(); }
        }

        public override void DisposeManaged() => this.DetachDatabaseAsync().Wait( this.ReadTimeout + this.WriteTimeout );
    }

    ///// <summary>
    /////     work in progress. reiventing the same damn wheel. again."
    ///// </summary>
    //public static class LocalDB {
    // ///
    // <summary>/// ///</summary>
    // public static ISqlLocalDbProvider Provider { get; } = new SqlLocalDbProvider();

    // /// <summary> /// /// </summary> private static Lazy<Folder> datebaseBaseFolder = new Lazy<Folder>();

    // //public static readonly ConcurrentDictionary<String, Document> DataPointers = new ConcurrentDictionary<String, Document>(); //public static readonly ConcurrentDictionary<String, Document> LogPointers = new
    // ConcurrentDictionary<String, Document>();

    // static LocalDB() { //const string name = "Properties"; //var instance = GetInstance( name ); //var mdf = new Document( Path.Combine( PersistenceExtensions.DataFolder.Value.FullName, String.Format( "{0}.mdf", name )
    // ) ); //var ldf = new Document( Path.Combine( PersistenceExtensions.DataFolder.Value.FullName, String.Format( "{0}.ldf", name ) ) );

    // //var list = new[ ] { mdf, ldf }.ToList(); //InstanceFiles[ name ].AddRange( list );

    // //Builders[ name ].SetPhysicalFileName( mdf.FullPathWithFileName );

    // //instance.Start(); }

    // ///
    // <summary>
    // /// Instance names in SQL Local DB are case-insensitive ///
    // </summary>
    // ///
    // <param name="name"> </param>
    // ///
    // <param name="where"></param>
    // ///
    // <returns></returns>
    // public static Boolean Start( [CanBeNull] String name, Folder where ) { if ( String.IsNullOrWhiteSpace( name ) ) { return false; }

    // try { var localDbInstance = Provider.CreateInstance( name ); var connectionStringBuilder = localDbInstance.CreateConnectionStringBuilder(); connectionStringBuilder.SetPhysicalFileName(); localDbInstance.Start();

    // return true;

    // } catch ( Exception) { return false; } }

    // ///// <summary> ///// ///// </summary> //public static ConcurrentDictionary<string, ISqlLocalDbInstance> Instances { get; } //= new ConcurrentDictionary<String, ISqlLocalDbInstance>();

    // ///// <summary> ///// ///// </summary> //public static ConcurrentSet<ConcurrentList<Document>> InstanceFiles { get; } //= new ConcurrentSet<ConcurrentList<Document>>();

    // ///// <summary> ///// ///// </summary> //public static ConcurrentDictionary<string, DbConnectionStringBuilder> Builders { get; } //= new ConcurrentDictionary<String, DbConnectionStringBuilder>();

    // //[CanBeNull] //public static ISqlLocalDbInstance Instance { // get { // return instance; // } // set { // value.Should().NotBeNull(); // instance = value; // if ( null != instance ) { // instance.Start(); //
    // OutputFolder = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), DatabaseDirectory ); // var mdfFilename = String.Format( "{0}.mdf", DatabaseName ); // DatabaseMdfPath = Path.Combine(
    // OutputFolder, mdfFilename ); // DatabaseLogPath = Path.Combine( OutputFolder, String.Format( "{0}_log.ldf", DatabaseName ) );

    // // } // } //}

    // public static object DatabaseName { get; private set; } public static String OutputFolder { get; set; }

    // [NotNull] public static ISqlLocalDbInstance GetInstance( this String instanceName ) { ISqlLocalDbInstance result; if ( Instances.TryGetValue( instanceName, out result ) ) { return result; } Instances[ instanceName
    // ] = Provider.GetOrCreateInstance( instanceName ); result = Instances[ instanceName ]; result.Start(); return result; }

    // [NotNull] public static DbConnectionStringBuilder GetConnectionStringBuilder( this String instanceName ) { DbConnectionStringBuilder result; if ( Builders.TryGetValue( instanceName, out result ) ) { return result; }

    // Builders[ instanceName ] = GetInstance( instanceName ).CreateConnectionStringBuilder();

    // return Builders[ instanceName ]; }

    // //private static Lazy<ISqlLocalDbInstance> instanceLazy = new Lazy<ISqlLocalDbInstance>( ()
    // => Instance );

    // public static Boolean TryPut<TData>( String genericThingHere ) => false;

    //	public static Boolean TryGet<TData>( String genericThingHere, out TData result ) {
    //		//get data from localdb?
    //		//how?
    //		result = default(TData);
    //		return false;
    //	}
    //}
}