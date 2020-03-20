﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Location.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Location.cs" was last formatted by Protiguous on 2020/03/16 at 3:10 PM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>Points to a local file, a directory, a LAN file, or an internet file.</summary>
    /// <remarks>(Stored internally as a string)</remarks>
    public class Location : IEquatable<Location>, IComparable<Location>, IComparable {

        public Int32 CompareTo( [CanBeNull] Object? obj ) {
            if ( obj is null ) {
                return 1;
            }

            if ( ReferenceEquals( objA: this, objB: obj ) ) {
                return 0;
            }

            if ( !( obj is Location ) ) {
                throw new ArgumentException( message: $"Object must be of type {nameof( Location )}" );
            }

            return this.CompareTo( other: ( Location ) obj );
        }

        public Int32 CompareTo( [CanBeNull] Location other ) {
            if ( ReferenceEquals( objA: this, objB: other ) ) {
                return 0;
            }

            if ( other is null ) {
                return 1;
            }

            return String.Compare( strA: this.Address, strB: other.Address, comparisonType: StringComparison.Ordinal );
        }

        public Boolean Equals( Location other ) => other != null && this.Address == other.Address;

        private Int32 HashCode => this.Address.GetHashCode();

        /// <summary>A local file, LAN, UNC, or a URI.</summary>
        [NotNull]
        public String Address { get; }

        private Location() => this.Address = String.Empty;

        public Location( [NotNull] String location ) {
            if ( String.IsNullOrWhiteSpace( value: location ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( location ) );
            }

            location = location.Trim();

            if ( String.IsNullOrWhiteSpace( value: location ) ) {
                throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( location ) );
            }

            this.Address = new Uri( uriString: location ).AbsoluteUri;
        }

        public static Boolean Equals( [CanBeNull] Location left, [CanBeNull] Location right ) {
            if ( left is null && right is null ) {
                return true;
            }

            if ( left != null && right is null ) {
                return default;
            }

            if ( left is null ) {
                return default;
            }

            return left.HashCode == right.HashCode;
        }

        public static Boolean operator !=( [CanBeNull] Location left, [CanBeNull] Location right ) => !Equals( left: left, right: right );

        public static Boolean operator <( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) < 0;

        public static Boolean operator <=( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) <= 0;

        public static Boolean operator ==( [CanBeNull] Location left, [CanBeNull] Location right ) => Equals( left: left, right: right );

        public static Boolean operator >( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) > 0;

        public static Boolean operator >=( [CanBeNull] Location left, [CanBeNull] Location right ) => Comparer<Location>.Default.Compare( x: left, y: right ) >= 0;

        public override Boolean Equals( Object obj ) => this.Equals( other: obj as Location );

        public override Int32 GetHashCode() => this.HashCode;

    }

}