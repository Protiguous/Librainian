﻿// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ImmutableList.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "ImmutableList.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.Collections.Lists {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Librainian.Extensions;

    /// <summary>
    ///     <para>A list that has been written to be observationally immutable.</para>
    ///     <para>A mutable array is used as the backing store for the list, but no mutable operations are offered.</para>
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the list.</typeparam>
    /// <remarks>http://joeduffyblog.com/2007/11/11/immutable-types-for-c/</remarks>
    [Immutable]
    public sealed class ImmutableList<T> : IList<T>, IPossibleThrowable {

        [NotNull]
        private T[] Array { get; }

        /// <summary>Retrieves the immutable count of the list.</summary>
        public Int32 Count => this.Array.Length;

        /// <summary>Whether the list is read only: because the list is immutable, this is always true.</summary>
        public Boolean IsReadOnly => true;

        /// <summary>If set to false, anything that would normally try to mutate this, the <see cref="Exception" /> is ignored.</summary>
        public ThrowSetting ThrowExceptions { get; set; }

        /// <summary>Accesses the element at the specified index. Because the list is immutable, the setter will always throw an exception.</summary>
        /// <param name="index">The index to access.</param>
        /// <returns>The element at the specified index.</returns>
        [CanBeNull]
        public T this[ Int32 index ] {
            get => this.Array[ index ];

            // ReSharper disable once ValueParameterNotUsed
            set => this.ThrowNotMutable();
        }

        /// <summary>Create a new list.</summary>
        public ImmutableList() => this.Array = new T[ 0 ];

        /// <summary>Create a new list, copying elements from the specified array.</summary>
        /// <param name="arrayToCopy">An array whose contents will be copied.</param>
        public ImmutableList( [NotNull] T[] arrayToCopy ) {
            if ( arrayToCopy is null ) {
                throw new ArgumentNullException( paramName: nameof( arrayToCopy ) );
            }

            this.Array = new T[ arrayToCopy.Length ];
            Buffer.BlockCopy( src: arrayToCopy, srcOffset: 0, dst: this.Array, dstOffset: 0, count: arrayToCopy.Length );
        }

        /// <summary>Create a new list, copying elements from the specified enumerable.</summary>
        /// <param name="enumerableToCopy">An enumerable whose contents will be copied.</param>
        public ImmutableList( [NotNull] IEnumerable<T> enumerableToCopy ) => this.Array = enumerableToCopy.ToArray();

        private void ThrowNotMutable() {
            if ( this.ThrowExceptions == ThrowSetting.Throw ) {
                throw new InvalidOperationException( message: "Cannot mutate an immutable list." );
            }
        }

        /// <summary>Checks whether the specified item is contained in the list.</summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is found, false otherwise.</returns>
        public Boolean Contains( T item ) => System.Array.IndexOf( array: this.Array, value: item ) != -1;

        /// <summary>Copies the list and adds a new value at the end.</summary>
        /// <param name="value">The value to add.</param>
        /// <returns>A modified copy of this list.</returns>
        [NotNull]
        public ImmutableList<T> CopyAndAdd( [CanBeNull] T value ) {
            var newArray = new T[ this.Array.Length + 1 ];
            this.Array.CopyTo( array: newArray, index: 0 );
            newArray[ this.Array.Length ] = value;

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>Returns a new, cleared (empty) immutable list.</summary>
        /// <returns>A modified copy of this list.</returns>
        [NotNull]
        public ImmutableList<T> CopyAndClear() => new ImmutableList<T>( arrayToCopy: new T[ 0 ] );

        /// <summary>Copies the list and inserts a particular element.</summary>
        /// <param name="index">The index at which to insert an element.</param>
        /// <param name="item"> The element to insert.</param>
        /// <returns>An immutable copy of this modified list.</returns>
        [NotNull]
        public ImmutableList<T> CopyAndInsert( Int32 index, [CanBeNull] T item ) {
            var newArray = new T[ this.Array.Length + 1 ];
            Buffer.BlockCopy( src: this.Array, srcOffset: 0, dst: newArray, dstOffset: 0, count: index );
            newArray[ index ] = item;
            Buffer.BlockCopy( src: this.Array, srcOffset: index, dst: newArray, dstOffset: index + 1, count: this.Array.Length - index );

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>Copies the list and removes a particular element.</summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>A modified copy of this list.</returns>
        [NotNull]
        public ImmutableList<T> CopyAndRemove( [CanBeNull] T item ) {
            var index = this.IndexOf( item: item );

            if ( index == -1 ) {
                throw new ArgumentException( message: "Item not found in list." );
            }

            return this.CopyAndRemoveAt( index: index );
        }

        /// <summary>Copies the list and removes a particular element.</summary>
        /// <param name="index">The index of the element to remove.</param>
        /// <returns>A modified copy of this list.</returns>
        [NotNull]
        public ImmutableList<T> CopyAndRemoveAt( Int32 index ) {
            var newArray = new T[ this.Array.Length - 1 ];
            Buffer.BlockCopy( src: this.Array, srcOffset: 0, dst: newArray, dstOffset: 0, count: index );
            Buffer.BlockCopy( src: this.Array, srcOffset: index + 1, dst: newArray, dstOffset: index, count: this.Array.Length - index - 1 );

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>Copies the list and modifies the specific value at the index provided.</summary>
        /// <param name="index">The index whose value is to be changed.</param>
        /// <param name="item"> The value to store at the specified index.</param>
        /// <returns>A modified copy of this list.</returns>
        [NotNull]
        public ImmutableList<T> CopyAndSet( Int32 index, [CanBeNull] T item ) {
            var newArray = new T[ this.Array.Length ];
            this.Array.CopyTo( array: newArray, index: 0 );
            newArray[ index ] = item;

            return new ImmutableList<T>( arrayToCopy: newArray );
        }

        /// <summary>Copies the contents of this list to a destination array.</summary>
        /// <param name="array">The array to copy elements to.</param>
        /// <param name="index">The index at which copying begins.</param>
        public void CopyTo( T[] array, Int32 index ) => this.Array.CopyTo( array: array, index: index );

        /// <summary>Retrieves an enumerator for the list’s collections.</summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<T> GetEnumerator() => ( ( IEnumerable<T> )this.Array ).GetEnumerator();

        /// <summary>Finds the index of the specified element.</summary>
        /// <param name="item">An item to search for.</param>
        /// <returns>The index of the item, or -1 if it was not found.</returns>
        public Int32 IndexOf( T item ) => System.Array.IndexOf( array: this.Array, value: item );

        /// <summary>This method is unsupported on this type, because it is immutable.</summary>
        void ICollection<T>.Add( T item ) => this.ThrowNotMutable();

        /// <summary>This method is unsupported on this type, because it is immutable.</summary>
        void ICollection<T>.Clear() => this.ThrowNotMutable();

        /// <summary>Retrieves an enumerator for the list’s collections.</summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>This method is unsupported on this type, because it is immutable.</summary>
        void IList<T>.Insert( Int32 index, T item ) => this.ThrowNotMutable();

        /// <summary>This method is unsupported on this type, because it is immutable.</summary>
        Boolean ICollection<T>.Remove( T item ) {
            this.ThrowNotMutable();

            return default;
        }

        /// <summary>This method is unsupported on this type, because it is immutable.</summary>
        void IList<T>.RemoveAt( Int32 index ) => this.ThrowNotMutable();
    }
}