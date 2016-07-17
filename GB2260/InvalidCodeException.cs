using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB2260
{
    public class InvalidCodeException: Exception
    {
        public InvalidCodeException(string message): base(message)
        {
        }
    }
}
