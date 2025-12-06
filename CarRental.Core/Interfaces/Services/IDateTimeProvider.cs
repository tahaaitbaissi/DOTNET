using System;

namespace CarRental.Core.Interfaces.Services
{
    /// <summary>
    /// Provides date and time abstraction for testability
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current UTC date and time
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets today's date in UTC
        /// </summary>
        DateTime Today { get; }
    }
}

