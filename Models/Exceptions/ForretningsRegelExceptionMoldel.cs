using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Exceptions
{
        public class ForretningsRegelExceptionModel : Exception
        {
            public ForretningsRegelExceptionModel() : base() { }

            public ForretningsRegelExceptionModel(string message) : base(message) { }

            public ForretningsRegelExceptionModel(string message, Exception innerException)
                : base(message, innerException) { }
        }
}
