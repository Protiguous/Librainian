﻿
namespace Librainian.Measurement.Time.FluentTime {
    using System;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public static class Last {

        public static DateTime Friday() {
            return GetLastOfDay( DayOfWeek.Friday );
        }

        public static DateTime Monday() {
            return GetLastOfDay( DayOfWeek.Monday );
        }

        public static DateTime Saturday() {
            return GetLastOfDay( DayOfWeek.Saturday );
        }

        public static DateTime Sunday() {
            return GetLastOfDay( DayOfWeek.Sunday );
        }

        public static DateTime Thursday() {
            return GetLastOfDay( DayOfWeek.Thursday );
        }

        public static DateTime Tuesday() {
            return GetLastOfDay( DayOfWeek.Tuesday );
        }

        public static DateTime Wednesday() {
            return GetLastOfDay( DayOfWeek.Wednesday );
        }

        private static DateTime GetLastOfDay( DayOfWeek dayOfWeek ) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta >= 0 ? delta - 7 : delta );
            return result;
        }
    }
}