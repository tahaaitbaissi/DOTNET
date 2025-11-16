using System;
using System.Collections.Generic;

namespace CarRental.Core.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        // Private constructor for EF Core
        private Money() { }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Money amount cannot be negative.");
            }
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency must be specified.");
            }

            Amount = amount;
            Currency = currency.ToUpper();
        }

        // Example of an immutable operation
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
            {
                throw new InvalidOperationException("Cannot add Money of different currencies.");
            }

            // Returns a NEW Money object
            return new Money(Amount + other.Amount, Currency);
        }

        // This is the method required by the 'ValueObject' base class
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}