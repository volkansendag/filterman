using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuteDev.Database
{
    /// <summary>
    /// filtre operator degerleri - enum. (volkansendag - 13.01.2016)
    /// </summary>
    internal enum Op
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        NotEquals,
        NotContains
    }
}
