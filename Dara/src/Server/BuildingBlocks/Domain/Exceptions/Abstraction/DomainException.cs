using System;

namespace Dara.BuildingBlocks.Domain.Exceptions.Abstraction
{
    public abstract class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
            
        }
    }
}