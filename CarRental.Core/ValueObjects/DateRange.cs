using System;
using System.Collections.Generic;

namespace CarRental.Core.ValueObjects
{
    public class DateRange : ValueObject
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        // Private constructor for EF Core
        private DateRange() { }

        public DateRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            Start = start;
            End = end;
        }

        public int DurationInDays()
        {
            return (End - Start).Days;
        }

        public bool Overlaps(DateRange other)
        {
            return Start < other.End && other.Start < End;
        }

        // This is the method required by the 'ValueObject' base class
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}