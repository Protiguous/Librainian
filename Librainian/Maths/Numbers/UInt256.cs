﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "UInt256.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.


namespace Librainian.Maths.Numbers;

using System;
using System.Globalization;
using System.Net;
using System.Numerics;
using Exceptions;
using ExtendedNumerics;
using Utilities;

/// <summary>
///     <para>Pulled from the BitcoinSharp project.</para>
/// </summary>
public readonly record struct UInt256 : IComparable<UInt256> {

	/// <summary>
	/// Precomputed for speed.
	/// </summary>
	private readonly Int32? _hashCode;

	private readonly UInt64 _part1; // parts are big-endian

	private readonly UInt64 _part2; // parts are big-endian

	private readonly UInt64 _part3; // parts are big-endian

	private readonly UInt64 _part4; // parts are big-endian

	public static UInt256 Zero { get; } = new( Array.Empty<Byte>() );

	private UInt256( UInt64 part1, UInt64 part2, UInt64 part3, UInt64 part4, Boolean preCalcHashcode = true ) {
		this._part1 = part1;
		this._part2 = part2;
		this._part3 = part3;
		this._part4 = part4;

        if ( preCalcHashcode ) {
			this._hashCode = HashCode.Combine( this._part1, this._part2, this._part3, this._part4 );
		}
		
	}

	public UInt256( Byte[] value!!, Boolean preCalcHashcode = true ) {

		if ( value[32] is > 32 and not ( 33 and 0 ) ) {
			throw new ArgumentOutOfRangeException( nameof( value ) );
		}

		if ( value.Length < 32 ) {
			value = value.Concat( new Byte[32 - value.Length] );
		}

		// convert parts and store
		this._part1 = value.ToUInt64( 24 );
		this._part2 = value.ToUInt64( 16 );
		this._part3 = value.ToUInt64( 8 );
		this._part4 = value.ToUInt64( 0 );

        if ( preCalcHashcode ) {
            this._hashCode = HashCode.Combine( this._part1, this._part2, this._part3, this._part4 );
        }
    }

	public UInt256( Int32 value )  {
		if ( value.NotAny() ) {
			throw new ArgumentOutOfRangeException( nameof( value ) );
		}

        this = new UInt256( value.GetBytes() );
    }

	public UInt256( Int64 value )  {
        if ( value.NotAny() ) {
            throw new ArgumentOutOfRangeException( nameof( value ) );
        }

        this = new UInt256( value.GetBytes() );
	}

	public UInt256( UInt32 value )  {
		if ( value.NotAny() ) {
            throw new ArgumentOutOfRangeException( nameof( value ) );
        }

        this = new UInt256( value.GetBytes() );
    }

	public UInt256( UInt64 value ) {
        Guard( value );

        this = new UInt256( value.GetBytes() );
    }

    private static void Guard<T>( T value ) {
        if ( value.NotAny() ) {
            throw new ArgumentOutOfRangeException( nameof( value ) );
        }
    }

    public UInt256( BigInteger value ) {
		if ( value < BigInteger.Zero ) {
			throw new ArgumentOutOfRangeException( nameof( value ) );
		}

		this = FromByteArray( value.ToByteArray() );
	}

	public static UInt256 DivRem( UInt256 dividend, UInt256 divisor, out UInt256 remainder ) {
		var result = new UInt256( BigInteger.DivRem( dividend.ToBigInteger(), divisor.ToBigInteger(), out var remainderBigInt ) );
		remainder = new UInt256( remainderBigInt );

		return result;
	}

	public static explicit operator BigInteger( UInt256 value ) => value.ToBigInteger();
	public static explicit operator BigDecimal( UInt256 value ) => value.ToBigInteger();
	public static explicit operator Double( UInt256 value ) => ( Double )value.ToBigInteger();

	public static UInt256 FromByteArray( Byte[] buffer ) {
		unchecked {
			if ( buffer.Length != 32 ) {
				throw new ArgumentOutOfRangeException( nameof( buffer ) );
			}

			var part1 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 0 ) );
			var part2 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 8 ) );
			var part3 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 16 ) );
			var part4 = ( UInt64 )IPAddress.HostToNetworkOrder( BitConverter.ToInt64( buffer, 24 ) );

			return new UInt256( part1, part2, part3, part4 );
		}
	}

	public static implicit operator UInt256( Byte value ) => new( value );

	public static implicit operator UInt256( Int32 value ) => new( value );

	public static implicit operator UInt256( Int64 value ) => new( value );

	public static implicit operator UInt256( SByte value ) => new( value );

	public static implicit operator UInt256( Int16 value ) => new( value );

	public static implicit operator UInt256( UInt32 value ) => new( value );

	public static implicit operator UInt256( UInt64 value ) => new( value );

	public static implicit operator UInt256( UInt16 value ) => new( value );

	public static Double Log( UInt256 value, Double baseValue ) => BigInteger.Log( value.ToBigInteger(), baseValue );

	//public static Boolean operator !=( UInt256 left, UInt256 right ) => !( left == right );

	public static UInt256 operator %( UInt256 dividend, UInt256 divisor ) => new( dividend.ToBigInteger() % divisor.ToBigInteger() );

	public static UInt256 operator *( UInt256 left, UInt256 right ) => new( left.ToBigInteger() * right.ToBigInteger() );

	public static UInt256 operator /( UInt256 dividend, UInt256 divisor ) => new( dividend.ToBigInteger() / divisor.ToBigInteger() );

	public static UInt256 operator ~( UInt256 value ) => new( ~value._part1, ~value._part2, ~value._part3, ~value._part4 );

	public static Boolean operator <( UInt256 left, UInt256 right ) {
		if ( left._part1 < right._part1 ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 < right._part2 ) ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 < right._part3 ) ) {
			return true;
		}

		return ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 < right._part4 );
	}

	public static Boolean operator <=( UInt256 left, UInt256 right ) {
		if ( left._part1 < right._part1 ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 < right._part2 ) ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 < right._part3 ) ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 < right._part4 ) ) {
			return true;
		}

		return left.Equals( right );
	}

	//public static Boolean operator ==( UInt256 left, UInt256 right ) =>
	//	left._part1 == right._part1 && left._part2 == right._part2 && left._part3 == right._part3 && left._part4 == right._part4;

	public static Boolean operator >( UInt256 left, UInt256 right ) {
		if ( left._part1 > right._part1 ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 > right._part2 ) ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 > right._part3 ) ) {
			return true;
		}

		return ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 > right._part4 );
	}

	public static Boolean operator >=( UInt256 left, UInt256 right ) {
		if ( left._part1 > right._part1 ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 > right._part2 ) ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 > right._part3 ) ) {
			return true;
		}

		if ( ( left._part1 == right._part1 ) && ( left._part2 == right._part2 ) && ( left._part3 == right._part3 ) && ( left._part4 > right._part4 ) ) {
			return true;
		}

		return left == right;
	}

	public static UInt256 operator >>( UInt256 value, Int32 shift ) => new( value.ToBigInteger() >> shift );

	public static UInt256 Parse( String? value ) => new( BigInteger.Parse( "0" + value ).ToByteArray() );

	public static UInt256 Parse( String? value, IFormatProvider? provider ) => new( BigInteger.Parse( "0" + value, provider ).ToByteArray() );

	public static UInt256 Parse( String? value, NumberStyles style ) => new( BigInteger.Parse( "0" + value, style ).ToByteArray() );

	public static UInt256 Parse( String? value, NumberStyles style, IFormatProvider? provider ) => new( BigInteger.Parse( "0" + value, style, provider ).ToByteArray() );

	public static UInt256 Pow( UInt256 value, Int32 exponent ) => new( BigInteger.Pow( value.ToBigInteger(), exponent ) );

	public Int32 CompareTo( UInt256 other ) {
		if ( this == other ) {
			return 0;
		}

		if ( this < other ) {
			return -1;
		}

		if ( this > other ) {
			return +1;
		}

		throw new Exception();
	}

	/*
    public override Boolean Equals( Object? obj ) {
        if ( obj is UInt256 other ) {
            return other._part1 == this._part1 && other._part2 == this._part2 && other._part3 == this._part3 && other._part4 == this._part4;
        }

        return false;
    }
    */

	[NeedsTesting]
	public override Int32 GetHashCode() => this._hashCode;

	public BigInteger ToBigInteger() => new( this.ToByteArray().Concat( 0 ) );

	public Byte[] ToByteArray() {
		var buffer = new Byte[32];
		Buffer.BlockCopy( this._part4.GetBytes(), 0, buffer, 0, 8 );
		Buffer.BlockCopy( this._part3.GetBytes(), 0, buffer, 8, 8 );
		Buffer.BlockCopy( this._part2.GetBytes(), 0, buffer, 16, 8 );
		Buffer.BlockCopy( this._part1.GetBytes(), 0, buffer, 24, 8 );

		return buffer;
	}

	public void ToByteArray( Byte[] buffer!!, Int32 offset ) {
		Buffer.BlockCopy( this._part4.GetBytes(), 0, buffer, 0 + offset, 8 );
		Buffer.BlockCopy( this._part3.GetBytes(), 0, buffer, 8 + offset, 8 );
		Buffer.BlockCopy( this._part2.GetBytes(), 0, buffer, 16 + offset, 8 );
		Buffer.BlockCopy( this._part1.GetBytes(), 0, buffer, 24 + offset, 8 );
	}

	//TODO properly taken into account host endianness
	public Byte[] ToByteArrayBigEndian() {
		unchecked {
			var buffer = new Byte[32];

			Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part1 ) ), 0, buffer, 0, 8 );

			Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part2 ) ), 0, buffer, 8, 8 );

			Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part3 ) ), 0, buffer, 16, 8 );

			Buffer.BlockCopy( BitConverter.GetBytes( IPAddress.HostToNetworkOrder( ( Int64 )this._part4 ) ), 0, buffer, 24, 8 );

			return buffer;
		}
	}

	public override String ToString() => this.ToHexNumberString();
}