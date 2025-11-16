using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Core.Entities
{
    public abstract class Entity
    {
        public long Id { get; protected set; }
    }
}
