using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppWaves
{
	public static class Utility
	{
		private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		private static readonly DateTime unixEpochLocal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

        /// <summary>
        /// Get a UTC date time from a unix epoch in seconds
        /// </summary>
        /// <param name="unixTimeStampSeconds">Unix epoch in seconds</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime UnixTimeStampToDateTimeSeconds(this double unixTimeStampSeconds)
        {
            return unixEpoch.AddSeconds(unixTimeStampSeconds);
        }

        /// <summary>
        /// Get a UTC date time from a unix epoch in milliseconds
        /// </summary>
        /// <param name="unixTimeStampSeconds">Unix epoch in milliseconds</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime UnixTimeStampToDateTimeMilliseconds(this double unixTimeStampMilliseconds)
        {
            return unixEpoch.AddMilliseconds(unixTimeStampMilliseconds);
        }
        public static DateTime UnixTimeStampToDateTimeMicroseconds(this double unixTimeStampMicroseconds)
        {
            return unixEpoch.AddMilliseconds(unixTimeStampMicroseconds / 1000);
        }

        /// <summary>
        /// Get a utc date time from a local unix epoch in seconds
        /// </summary>
        /// <param name="unixTimeStampSeconds">Unix epoch in seconds</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime UnixTimeStampLocalToDateTimeSeconds(this double unixTimeStampSeconds)
        {
            return unixEpochLocal.AddSeconds(unixTimeStampSeconds).ToUniversalTime();
        }

        /// <summary>
        /// Get a utc date time from a local unix epoch in milliseconds
        /// </summary>
        /// <param name="unixTimeStampSeconds">Unix epoch in milliseconds</param>
        /// <returns>Local DateTime</returns>
        public static DateTime UnixTimeStampLocalToDateTimeMilliseconds(this double unixTimeStampMilliseconds)
        {
            return unixEpochLocal.AddMilliseconds(unixTimeStampMilliseconds).ToUniversalTime();
        }

        /// <summary>
        /// Get a unix timestamp in seconds from a DateTime
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>Unix epoch in seconds</returns>
        public static double UnixTimestampFromDateTimeSeconds(this DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Local)
            {
                dt = dt.ToUniversalTime();
            }
            else if (dt.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Unable to create unix epoch from DateTime with unspecified date time kind");
            }
            return (dt - unixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Get a unix timestamp in milliseconds from a DateTime
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>Unix timestamp in milliseconds</returns>
        public static double UnixTimestampFromDateTimeMilliseconds(this DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Local)
            {
                dt = dt.ToUniversalTime();
            }
            else if (dt.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Unable to create unix epoch from DateTime with unspecified date time kind");
            }
            return (dt - unixEpoch).TotalMilliseconds;
        }

    }
}
