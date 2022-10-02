using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Validation
{
    public class MarketException : Exception
    {
        public MarketException()
        {
        }

        public MarketException(string message)
            : base(message)
        {
        }

        public MarketException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
