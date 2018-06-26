// Copyright � 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DynamicExtensions.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "DynamicExtensions.cs" was last formatted by Protiguous on 2018/06/04 at 3:41 PM.

namespace Librainian.AmazedSaint {

	using System;
	using System.Linq;
	using System.Xml.Linq;
	using JetBrains.Annotations;

	/// <summary>
	///     Extension methods for our ElasticObject. See
	///     http: //amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
	/// </summary>
	public static class DynamicExtensions {

		/// <summary>
		///     Build an expando from an XElement
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		[NotNull]
		public static ElasticObject ElasticFromXElement( [NotNull] XElement el ) {
			var exp = new ElasticObject();

			if ( !String.IsNullOrEmpty( el.Value ) ) { exp.InternalValue = el.Value; }

			exp.InternalName = el.Name.LocalName;

			foreach ( var a in el.Attributes() ) { exp.CreateOrGetAttribute( a.Name.LocalName, a.Value ); }

			var textNode = el.Nodes().FirstOrDefault();

			if ( textNode is XText ) { exp.InternalContent = textNode.ToString(); }

			foreach ( var child in el.Elements().Select( ElasticFromXElement ) ) {
				child.InternalParent = exp;
				exp.AddElement( child );
			}

			return exp;
		}

		/// <summary>
		///     Converts an XElement to the expando
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		[NotNull]
		public static dynamic ToElastic( [NotNull] this XElement e ) => ElasticFromXElement( e );

		/// <summary>
		///     Converts an expando to XElement
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static XElement ToXElement( this ElasticObject e ) => XElementFromElastic( e );

		/// <summary>
		///     Returns an XElement from an ElasticObject
		/// </summary>
		/// <param name="elastic"></param>
		/// <returns></returns>
		[NotNull]
		public static XElement XElementFromElastic( [NotNull] ElasticObject elastic ) {
			var exp = new XElement( elastic.InternalName );

			foreach ( var a in elastic.GetAttributes().Where( a => a.Value.InternalValue != null ) ) { exp.Add( new XAttribute( a.Key, a.Value.InternalValue ) ); }

			if ( elastic.InternalContent is String s ) { exp.Add( new XText( s ) ); }

			foreach ( var child in elastic.GetElements().Select( XElementFromElastic ) ) { exp.Add( child ); }

			return exp;
		}

	}

}