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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/LocalDB.cs" was last cleaned by Rick on 2014/08/20 at 12:15 PM

#endregion License & Information

namespace Librainian.Database {

    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;

    public class LocalDb : IDisposable {
        public const string DatabaseDirectory = "Data";

        public LocalDb( string databaseName = null ) {
            this.DatabaseName = string.IsNullOrWhiteSpace( databaseName ) ? Guid.NewGuid().ToString( "N" ) : databaseName;
            this.CreateDatabase();
        }

        public string ConnectionStringName { get; private set; }

        public string DatabaseName { get; private set; }

        public string OutputFolder { get; private set; }

        public string DatabaseMdfPath { get; private set; }

        public string DatabaseLogPath { get; private set; }

        public void Dispose() {
            this.DetachDatabase();
        }

        public IDbConnection OpenConnection() {
            return new SqlConnection( this.ConnectionStringName );
        }

        private void CreateDatabase() {
            this.OutputFolder = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), DatabaseDirectory );
            var mdfFilename = string.Format( "{0}.mdf", this.DatabaseName );
            this.DatabaseMdfPath = Path.Combine( this.OutputFolder, mdfFilename );
            this.DatabaseLogPath = Path.Combine( this.OutputFolder, String.Format( "{0}_log.ldf", this.DatabaseName ) );

            // Create Data Directory If It Doesn't Already Exist.
            if ( !Directory.Exists( this.OutputFolder ) ) {
                Directory.CreateDirectory( this.OutputFolder );
            }

            // If the database does not already exist, create it.
            var connectionString = String.Format( @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True" );
            using ( var connection = new SqlConnection( connectionString ) ) {
                connection.Open();
                var cmd = connection.CreateCommand();
                this.DetachDatabase();
                cmd.CommandText = String.Format( "CREATE DATABASE {0} ON (NAME = N'{0}', FILENAME = '{1}')", this.DatabaseName, this.DatabaseMdfPath );
                cmd.ExecuteNonQuery();
            }

            // Open newly created, or old database.
            this.ConnectionStringName = String.Format( @"Data Source=(LocalDB)\v11.0;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", this.DatabaseName, this.DatabaseMdfPath );
        }

        private void DetachDatabase() {
            try {
                var connectionString = String.Format( @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True" );
                using ( var connection = new SqlConnection( connectionString ) ) {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = string.Format( "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '{0}'", this.DatabaseName );
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
            finally {
                if ( File.Exists( this.DatabaseMdfPath ) ) {
                    File.Delete( this.DatabaseMdfPath );
                }
                if ( File.Exists( this.DatabaseLogPath ) ) {
                    File.Delete( this.DatabaseLogPath );
                }
            }
        }
    }
}