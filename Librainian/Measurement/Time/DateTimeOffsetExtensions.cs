// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries,
// repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper licenses and/or copyrights.
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DateTimeOffsetExtensions.cs" last formatted on 2022-02-17 at 11:22 AM by Protiguous.


namespace Librainian.Measurement.Time;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Maths;
using PooledAwait;

public static class DateTimeOffsetExtensions {

	public static DateTimeOffset Ago( this DateTimeOffset dateTime, TimeSpan timeSpan ) => dateTime - timeSpan;

	/// <summary>
	///     Returns the given <see cref="DateTimeOffset" /> with hour and minutes set At given values.
	/// </summary>
	/// <param name="current">The current <see cref="DateTimeOffset" /> to be changed.</param>
	/// <param name="hour">   The hour to set time to.</param>
	/// <param name="minute"> The minute to set time to.</param>
	/// <returns><see cref="DateTimeOffset" /> with hour and minute set to given values.</returns>
	public static DateTimeOffset At( this DateTimeOffset current, Int32 hour, Int32 minute ) => current.SetTime( hour, minute );

	/// <summary>
	///     Returns the given <see cref="DateTimeOffset" /> with hour and minutes and seconds set At given values.
	/// </summary>
	/// <param name="current">The current <see cref="DateTimeOffset" /> to be changed.</param>
	/// <param name="hour">   The hour to set time to.</param>
	/// <param name="minute"> The minute to set time to.</param>
	/// <param name="second"> The second to set time to.</param>
	/// <returns><see cref="DateTimeOffset" /> with hour and minutes and seconds set to given values.</returns>
	public static DateTimeOffset At( this DateTimeOffset current, Int32 hour, Int32 minute, Int32 second ) => current.SetTime( hour, minute, second );

	/// <summary>
	///     Returns the given <see cref="DateTimeOffset" /> with hour and minutes and seconds and milliseconds set At given
	///     values.
	/// </summary>
	/// <param name="current">     The current <see cref="DateTimeOffset" /> to be changed.</param>
	/// <param name="hour">        The hour to set time to.</param>
	/// <param name="minute">      The minute to set time to.</param>
	/// <param name="second">      The second to set time to.</param>
	/// <param name="milliseconds">The milliseconds to set time to.</param>
	/// <returns><see cref="DateTimeOffset" /> with hour and minutes and seconds set to given values.</returns>
	public static DateTimeOffset At( this DateTimeOffset current, Int32 hour, Int32 minute, Int32 second, Int32 milliseconds ) =>
		current.SetTime( hour, minute, second, milliseconds );

	public static DateTimeOffset Average( this IEnumerable<DateTimeOffset> dates ) {
		if ( dates is null ) {
			throw new ArgumentEmptyException( nameof( dates ) );
		}

		var ticks = dates.Select( time => time.Ticks ).Average();

		return new DateTimeOffset( ( Int64 )ticks, TimeSpan.Zero );
	}

	public static async Task<DateTimeOffset> Average( this IAsyncEnumerable<DateTimeOffset> dates, CancellationToken cancellationToken ) {
		if ( dates is null ) {
			throw new ArgumentEmptyException( nameof( dates ) );
		}

		var ticks = 0L;

		await foreach ( var date in dates.WithCancellation( cancellationToken ).ConfigureAwait( false ) ) {
			ticks = ticks == 0L ? date.UtcTicks : ( ticks + date.UtcTicks ).Half();
		}

		return new DateTimeOffset( ticks, TimeSpan.Zero );
	}

	/// <summary>
	///     The fastest time a task context switch should take?
	/// </summary>
	public static async PooledValueTask<TimeSpan> AwaitContextSwitch() {
		var stopwatch = Stopwatch.StartNew();

		await Task.Run( async () => await Task.Delay( 1 ).ConfigureAwait( false ) ).ConfigureAwait( false );

		return stopwatch.Elapsed;
	}

	/// <summary>
	///     Returns the Start of the given <paramref name="date" />.
	/// </summary>
	/// <param name="date"></param>
	public static DateTimeOffset BeginningOfDay( this DateTimeOffset date ) => new( date.Year, date.Month, date.Day, 0, 0, 0, 0, TimeSpan.Zero );

	public static Boolean Between( this DateTimeOffset dt, DateTimeOffset rangeBeg, DateTimeOffset rangeEnd ) => ( rangeBeg <= dt ) && ( dt <= rangeEnd );

	public static TimeSpan DateTimePrecision() {
		Int64 now;
		var then = DateTimeOffset.UtcNow.Ticks;

		do {
			now = DateTimeOffset.UtcNow.Ticks;
		} while ( then == now );

		return TimeSpan.FromTicks( now - then );
	}

	/// <summary>
	///     Decreases the <see cref="DateTimeOffset" /> by given <see cref="TimeSpan" /> value.
	/// </summary>
	/// <param name="startDate"></param>
	/// <param name="toSubtract"></param>
	public static DateTimeOffset DecreaseTime( this DateTimeOffset startDate, TimeSpan toSubtract ) => startDate - toSubtract;

	/// <summary>
	///     <para>Returns the last millisecond of the given <paramref name="date" />.</para>
	/// </summary>
	/// <param name="date"></param>
	public static DateTimeOffset EndOfDay( this DateTimeOffset date ) => new( date.Year, date.Month, date.Day, 23, 59, 59, 999, TimeSpan.Zero );

	//public static int Comparison( this Minutes minutes, Milliseconds milliseconds ) {
	//    var left = minutes.Value;
	//    var right = new Minutes( milliseconds: milliseconds ).Value;
	//    return left.CompareTo( right );
	//}
	/// <summary>
	///     Sets the day of the <see cref="DateTimeOffset" /> to the first day in that month.
	/// </summary>
	/// <param name="current">The current <see cref="DateTimeOffset" /> to be changed.</param>
	/// <returns>given <see cref="DateTimeOffset" /> with the day part set to the first day in that month.</returns>
	public static DateTimeOffset FirstDayOfMonth( this DateTimeOffset current ) => current.SetDay( 1 );

	public static DateTimeOffset FirstDayOfTheMonth( this DateTimeOffset date ) => new( date.Year, date.Month, 1, 0, 0, 0, TimeSpan.Zero );

	/// <summary>
	///     Returns a DateTimeOffset adjusted to the beginning of the week.
	/// </summary>
	/// <param name="dateTime">The DateTimeOffset to adjust</param>
	/// <returns>A DateTimeOffset instance adjusted to the beginning of the current week</returns>
	/// <remarks>the beginning of the week is controlled by the current Culture</remarks>
	public static DateTimeOffset FirstDayOfWeek( this DateTimeOffset dateTime ) {
		var currentCulture = CultureInfo.CurrentCulture;
		var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
		var offset = ( dateTime.DayOfWeek - firstDayOfWeek ) < 0 ? 7 : 0;
		var numberOfDaysSinceBeginningOfTheWeek = ( dateTime.DayOfWeek + offset ) - firstDayOfWeek;

		return dateTime.AddDays( -numberOfDaysSinceBeginningOfTheWeek );
	}

	/// <summary>
	///     Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 =&gt;
	///     2011-01-01T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTimeOffset to adjust</param>
	public static DateTimeOffset FirstDayOfYear( this DateTimeOffset current ) => current.SetDate( current.Year, 1, 1 );

	public static DateTimeOffset From( this DateTimeOffset dateTime, TimeSpan timeSpan ) => dateTime + timeSpan;

	/// <summary>
	///     returns seconds since 1970-01-01 as a <see cref="DateTimeOffset" />.
	/// </summary>
	/// <param name="timestamp"></param>
	public static DateTimeOffset FromUNIXTimestamp( this UInt64 timestamp ) => DateTimeOffset.UnixEpoch.AddSeconds( timestamp );

	/// <summary>
	///     returns seconds since 1970-01-01 as a <see cref="DateTimeOffset" />.
	/// </summary>
	/// <param name="timestamp"></param>
	public static DateTimeOffset FromUNIXTimestamp( this Int32 timestamp ) => DateTimeOffset.UnixEpoch.AddSeconds( timestamp );

	/// <summary>
	///     returns seconds since 1970-01-01 as a <see cref="DateTimeOffset" />.
	/// </summary>
	/// <param name="timestamp"></param>
	public static DateTimeOffset FromUNIXTimestamp( this Int64 timestamp ) => DateTimeOffset.UnixEpoch.AddSeconds( timestamp );

	/// <summary>
	///     Return how many years old the person is in <see cref="Years" />.
	/// </summary>
	/// <param name="dateOfBirth"></param>
	public static Years GetAge( this DateTimeOffset dateOfBirth ) {
		//this seems to work for 99% of cases, but it still feels hacky.
		//what about leap-year birthdays?
		//what about other calendars?
		var today = DateTimeOffset.Now;

		var a = ( ( ( today.Year * 100 ) + today.Month ) * 100 ) + today.Day;
		var b = ( ( ( dateOfBirth.Year * 100 ) + dateOfBirth.Month ) * 100 ) + dateOfBirth.Day;

		return new Years( ( a - b ) / 10000 );
	}

	public static Int32 GetQuarter( this DateTimeOffset date ) => ( ( date.Month - 1 ) / 3 ) + 1;

	/// <summary>
	///     Accurate to within how many nanoseconds?
	/// </summary>
	public static Int64 GetTimerAccuracy() => 1000000000L / Stopwatch.Frequency;

	/// <summary>
	///     Example: Console.WriteLine( 3.Hours().FromNow() );
	/// </summary>
	/// <param name="hours"></param>
	public static TimeSpan Hours( this Double hours ) => TimeSpan.FromHours( hours );

	/// <summary>
	///     Example: Console.WriteLine( 3.Hours().FromNow() );
	/// </summary>
	/// <param name="hours"></param>
	public static TimeSpan Hours( this Int32 hours ) => TimeSpan.FromHours( hours );

	/// <summary>
	///     Increases the <see cref="DateTimeOffset" /> object with given <see cref="TimeSpan" /> value.
	/// </summary>
	/// <param name="startDate"></param>
	/// <param name="toAdd"></param>
	public static DateTimeOffset IncreaseTime( this DateTimeOffset startDate, TimeSpan toAdd ) => startDate + toAdd;

	/// <summary>
	///     Determines whether the specified <see cref="DateTimeOffset" /> value is After then current value.
	/// </summary>
	/// <param name="current">      The current value.</param>
	/// <param name="toCompareWith">Value to compare with.</param>
	/// <returns><c>true</c> if the specified current is after; otherwise, <c>false</c>.</returns>
	public static Boolean IsAfter( this DateTimeOffset current, DateTimeOffset toCompareWith ) => current > toCompareWith;

	/// <summary>
	///     Determines whether the specified <see cref="DateTimeOffset" /> is before then current value.
	/// </summary>
	/// <param name="current">      The current value.</param>
	/// <param name="toCompareWith">Value to compare with.</param>
	/// <returns><c>true</c> if the specified current is before; otherwise, <c>false</c>.</returns>
	public static Boolean IsBefore( this DateTimeOffset current, DateTimeOffset toCompareWith ) => current < toCompareWith;

	/// <summary>
	///     Determine if a <see cref="DateTimeOffset" /> is in the future.
	/// </summary>
	/// <param name="dateTime">The date to be checked.</param>
	/// <returns><c>true</c> if <paramref name="dateTime" /> is in the future; otherwise <c>false</c>.</returns>
	public static Boolean IsInFuture( this DateTimeOffset dateTime ) => dateTime.ToUniversalTime() > DateTimeOffset.UtcNow;

	/// <summary>
	///     Determine if a <see cref="DateTimeOffset" /> is in the past.
	/// </summary>
	/// <param name="dateTime">The date to be checked.</param>
	/// <returns><c>true</c> if <paramref name="dateTime" /> is in the past; otherwise <c>false</c>.</returns>
	public static Boolean IsInPast( this DateTimeOffset dateTime ) => dateTime.ToUniversalTime() < DateTimeOffset.UtcNow;

	/// <summary>
	///     <para>Determines if the specified year is a leap year.</para>
	/// </summary>
	/// <param name="year">Year to test.</param>
	/// <copyright>
	///     Tommy
	///     Dugger & Jared Chavez
	/// </copyright>
	public static Boolean IsLeapYear( this Int64 year ) {
		// not divisible by 4? not a leap year
		if ( ( year % 4 ) != 0 ) {
			return false;
		}

		// divisible by 4 and not divisible by 100? always a leap year
		if ( ( year % 100 ) != 0 ) {
			return true;
		}

		// divisible by 4 and 100? Only a leap year if also divisible by 400
		return ( year % 400 ) == 0;
	}

	public static Boolean IsWeekend( this DateTimeOffset date ) => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

	public static Boolean IsWorkingDay( this DateTimeOffset date ) => date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;

	/// <summary>
	///     Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 =&gt;
	///     2011-12-25T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTimeOffset to adjust</param>
	public static DateTimeOffset LastDayOfWeeek( this DateTimeOffset current ) => current.FirstDayOfWeek().AddDays( 6 );

	/// <summary>
	///     untested.
	/// </summary>
	/// <param name="date"></param>
	public static DateTimeOffset LastDayOfWeek( this DateTimeOffset date ) {
		var month = date.Month;

		while ( ( month == date.Month ) && ( date.DayOfWeek != DayOfWeek.Saturday ) ) {
			date = date.AddDays( 1 );
		}

		if ( date.Month != month ) {
			date = date.AddDays( -1 );
		}

		return date;
	}

	/// <summary>
	///     Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 =&gt;
	///     2011-12-31T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTimeOffset to adjust</param>
	public static DateTimeOffset LastDayOfYear( this DateTimeOffset current ) => current.SetDate( current.Year, 12, 31 );

	/// <summary>
	///     Adds the given number of business days to the <see cref="DateTimeOffset" />.
	/// </summary>
	/// <param name="current">The date to be changed.</param>
	/// <returns>A <see cref="DateTimeOffset" /> increased by a given number of business days.</returns>
	public static void MakeNextBusinessDay( ref this DateTimeOffset current ) {
		while ( current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ) {
			current += TimeSpan.FromDays( 1 );
		}
	}

	/// <summary>
	///     Returns original <see cref="DateTimeOffset" /> value with time part set to midnight (alias for
	///     <see cref="BeginningOfDay" /> method).
	/// </summary>
	/// <param name="value"></param>
	public static DateTimeOffset Midnight( this DateTimeOffset value ) => value.BeginningOfDay();

	/// <summary>
	///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
	/// </summary>
	/// <param name="milliseconds"></param>
	public static TimeSpan Milliseconds( this Int64 milliseconds ) => TimeSpan.FromMilliseconds( milliseconds );

	/// <summary>
	///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
	/// </summary>
	/// <param name="milliseconds"></param>
	public static TimeSpan Milliseconds( this Double milliseconds ) => TimeSpan.FromMilliseconds( milliseconds );

	/// <summary>
	///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
	/// </summary>
	/// <param name="milliseconds"></param>
	public static TimeSpan Milliseconds( this Milliseconds milliseconds ) => milliseconds;

	/// <summary>
	///     Example: Console.WriteLine( 3.Minutes().FromNow() );
	/// </summary>
	/// <param name="minutes"></param>
	public static TimeSpan Minutes( this Int32 minutes ) => TimeSpan.FromMinutes( minutes );

	/// <summary>
	///     Example: Console.WriteLine( 3.Minutes().FromNow() );
	/// </summary>
	/// <param name="minutes"></param>
	public static TimeSpan Minutes( this Double minutes ) => TimeSpan.FromMinutes( minutes );

	/// <summary>
	///     Returns first next occurrence of specified <see cref="DayOfWeek" />.
	/// </summary>
	/// <param name="start"></param>
	/// <param name="day"></param>
	public static DateTimeOffset Next( this DateTimeOffset start, DayOfWeek day ) {
		do {
			start = start.NextDay();
		} while ( start.DayOfWeek != day );

		return start;
	}

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> increased by 24 hours ie Next Day.
	/// </summary>
	/// <param name="start"></param>
	public static DateTimeOffset NextDay( this DateTimeOffset start ) => start + 1.Days();

	public static DateTimeOffset NextWorkday( this DateTimeOffset date ) {
		var nextDay = date.AddDays( 1 );
		while ( !nextDay.IsWorkingDay() ) {
			nextDay = nextDay.AddDays( 1 );
		}

		return nextDay;
	}

	/// <summary>
	///     Returns original <see cref="DateTimeOffset" /> value with time part set to Noon (12:00:00h).
	/// </summary>
	/// <param name="value">The <see cref="DateTimeOffset" /> find Noon for.</param>
	/// <returns>A <see cref="DateTimeOffset" /> value with time part set to Noon (12:00:00h).</returns>
	public static DateTimeOffset Noon( this DateTimeOffset value ) => value.SetTime( 12, 0, 0, 0 );

	/// <summary>
	///     Returns first next occurrence of specified <see cref="DayOfWeek" />.
	/// </summary>
	/// <param name="start"></param>
	/// <param name="day"></param>
	public static DateTimeOffset Previous( this DateTimeOffset start, DayOfWeek day ) {
		do {
			start = start.PreviousDay();
		} while ( start.DayOfWeek != day );

		return start;
	}

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> decreased by 24h period ie Previous Day.
	/// </summary>
	/// <param name="start"></param>
	public static DateTimeOffset PreviousDay( this DateTimeOffset start ) => start - 1.Days();

	public static DateTimeOffset Round( this DateTimeOffset dateTime, RoundTo rt ) {
		DateTimeOffset rounded;

		switch ( rt ) {
			case RoundTo.Second: {
					rounded = new DateTimeOffset( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Offset );

					if ( dateTime.Millisecond >= 500 ) {
						rounded = rounded.AddSeconds( 1 );
					}

					break;
				}

			case RoundTo.Minute: {
					rounded = new DateTimeOffset( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Offset );

					if ( dateTime.Second >= 30 ) {
						rounded = rounded.AddMinutes( 1 );
					}

					break;
				}

			case RoundTo.Hour: {
					rounded = new DateTimeOffset( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Offset );

					if ( dateTime.Minute >= 30 ) {
						rounded = rounded.AddHours( 1 );
					}

					break;
				}

			case RoundTo.Day: {
					rounded = new DateTimeOffset( dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Offset );

					if ( dateTime.Hour >= 12 ) {
						rounded = rounded.AddDays( 1 );
					}

					break;
				}

			default: {
					throw new ArgumentOutOfRangeException( nameof( rt ) );
				}
		}

		return rounded;
	}

	/// <summary>
	///     Example: Console.WriteLine( 3.Seconds().FromNow() );
	/// </summary>
	/// <param name="seconds"></param>
	public static TimeSpan Seconds( this Int32 seconds ) => TimeSpan.FromSeconds( seconds );

	/// <summary>
	///     Example: Console.WriteLine( 3.Seconds().FromNow() );
	/// </summary>
	/// <param name="seconds"></param>
	public static TimeSpan Seconds( this Double seconds ) => TimeSpan.FromSeconds( seconds );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Year part.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="year"></param>
	public static DateTimeOffset SetDate( this DateTimeOffset value, Int32 year ) =>
		new( year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Year and Month part.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="year"></param>
	/// <param name="month"></param>
	public static DateTimeOffset SetDate( this DateTimeOffset value, Int32 year, Int32 month ) =>
		new( year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Year, Month and Day part.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="day"></param>
	public static DateTimeOffset SetDate( this DateTimeOffset value, Int32 year, Int32 month, Int32 day ) =>
		new( year, month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Day part.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="day"></param>
	public static DateTimeOffset SetDay( this DateTimeOffset value, Int32 day ) =>
		new( value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Hour part.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="hour"></param>
	public static DateTimeOffset SetHour( this DateTimeOffset originalDate, Int32 hour ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Millisecond part.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="millisecond"></param>
	public static DateTimeOffset SetMillisecond( this DateTimeOffset originalDate, Int32 millisecond ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, originalDate.Second, millisecond, originalDate.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Minute part.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="minute"></param>
	public static DateTimeOffset SetMinute( this DateTimeOffset originalDate, Int32 minute ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Month part.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="month"></param>
	public static DateTimeOffset SetMonth( this DateTimeOffset value, Int32 month ) =>
		new( value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Second part.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="second"></param>
	public static DateTimeOffset SetSecond( this DateTimeOffset originalDate, Int32 second ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, second, originalDate.Millisecond, originalDate.Offset );

	/// <summary>
	///     Returns the original <see cref="DateTimeOffset" /> with Hour part changed to supplied hour parameter.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="hour"></param>
	public static DateTimeOffset SetTime( this DateTimeOffset originalDate, Int32 hour ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Offset );

	/// <summary>
	///     Returns the original <see cref="DateTimeOffset" /> with Hour and Minute parts changed to supplied hour and minute
	///     parameters.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	public static DateTimeOffset SetTime( this DateTimeOffset originalDate, Int32 hour, Int32 minute ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Offset );

	/// <summary>
	///     Returns the original <see cref="DateTimeOffset" /> with Hour, Minute and Second parts changed to supplied hour,
	///     minute
	///     and second parameters.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	/// <param name="second"></param>
	public static DateTimeOffset SetTime( this DateTimeOffset originalDate, Int32 hour, Int32 minute, Int32 second ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, originalDate.Millisecond, originalDate.Offset );

	/// <summary>
	///     Returns the original <see cref="DateTimeOffset" /> with Hour, Minute, Second and Millisecond parts changed to
	///     supplied
	///     hour, minute, second and millisecond parameters.
	/// </summary>
	/// <param name="originalDate"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	/// <param name="second"></param>
	/// <param name="millisecond"></param>
	public static DateTimeOffset SetTime( this DateTimeOffset originalDate, Int32 hour, Int32 minute, Int32 second, Int32 millisecond ) =>
		new( originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, millisecond, originalDate.Offset );

	/// <summary>
	///     Returns <see cref="DateTimeOffset" /> with changed Year part.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="year"></param>
	public static DateTimeOffset SetYear( this DateTimeOffset value, Int32 year ) =>
		new( year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset );

	/// <summary>
	///     <para>
	///         Throws an <see cref="OverflowException" /> if the <paramref name="value" /> is lower than
	///         <see cref="Decimal.MinValue" /> or higher than
	///         <see
	///             cref="Decimal.MaxValue" />
	///         .
	///     </para>
	/// </summary>
	/// <param name="value"></param>
	/// <exception cref="OverflowException"></exception>
	public static void ThrowIfOutOfDecimalRange( this Double value ) {
		if ( value < ( Double )Decimal.MinValue ) {
			throw new OverflowException( Constants.ValueIsTooLow );
		}

		if ( value > ( Double )Decimal.MaxValue ) {
			throw new OverflowException( Constants.ValueIsTooHigh );
		}
	}

	/// <summary>
	///     Seconds since 1970-01-01
	/// </summary>
	/// <param name="date"></param>
	public static Int64 ToUnixTimestamp( this DateTimeOffset date ) {
		var diff = date - DateTimeOffset.UnixEpoch;

		return ( Int64 )diff.TotalSeconds;
	}

	/// <summary>
	///     Return how many years old the person is in <see cref="Years" />.
	/// </summary>
	/// <param name="dateOfBirth"></param>
	public static Years YearsFrom( this DateTimeOffset dateOfBirth ) => new Seconds( ( DateTimeOffset.UtcNow - dateOfBirth ).TotalSeconds ).ToYears();

    public static TimeSpan GetStep( this DateTime from, DateTime to ) {
        var diff = from >= to ? from - to : to - from;

        if ( diff.TotalDays > 1 ) {
            return Days.One;
        }

        if ( diff.TotalHours > 1 ) {
            return Time.Hours.One;
        }

        if ( diff.TotalMinutes > 1 ) {
            return Time.Minutes.One;
        }

        if ( diff.TotalSeconds > 1 ) {
            return Time.Seconds.One;
        }

        return Time.Milliseconds.One;
    }

    /// <summary>
    ///     Return each <see cref="DateTime" /> between <paramref name="from" /> and <paramref name="to" />, stepped by a
    ///     <see cref="TimeSpan" /> ( <paramref name="step" />).
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to">  </param>
    /// <param name="step"></param>
    /// <remarks>//TODO Untested code!</remarks>
    /// <example>
    ///     var now = DateTime.UtcNow; var then = now.AddMinutes( 10 ); var minutes = now.To( then, TimeSpan.FromMinutes( 1 )
    ///     ); foreach ( var dateTime in minutes ) {
    ///     Console.WriteLine( dateTime ); }
    /// </example>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="step" />
    /// </exception>
    [Pure]
    public static IEnumerable<DateTime> To( this DateTime from, DateTime to, TimeSpan? step = null ) {
        step ??= from.GetStep( to );

        if ( step == TimeSpan.Zero ) {
            throw new ArgumentOutOfRangeException( nameof( step ), $"{nameof( step )} must not equal zero." );
        }

        if ( from > to ) {
            for ( var dateTime = from; dateTime >= to; dateTime -= step.Value ) {
                yield return dateTime;
            }
        }
        else {
            for ( var dateTime = from; dateTime <= to; dateTime += step.Value ) {
                yield return dateTime;
            }
        }
    }

}