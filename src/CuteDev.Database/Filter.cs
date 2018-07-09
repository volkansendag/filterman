using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuteDev.Database
{
    /// <summary>
    /// filtre birimini tasir. (volkansendag - 13.01.2016)
    /// </summary>
    internal class Filter
    {
        public string PropertyName { get; set; }
        public Op Operation { get; set; }
        public object Value { get; set; }
    }
}
