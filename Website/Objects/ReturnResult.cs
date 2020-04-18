using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Objects
{
    public class ReturnResult<T>
    {
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public T Item { get; set; }
    }
}
