using CarRental.Core.Interfaces.Services;
using System;

namespace CarRental.Infrastructure.Common
{
    /// <summary>
    /// Default implementation of IDateTimeProvider that returns actual system time.
    /// This abstraction allows for easy mocking in unit tests.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Gets the current UTC date and time
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Gets today's date in UTC (time component is 00:00:00)
        /// </summary>
        public DateTime Today => DateTime.UtcNow.Date;
    }
}

