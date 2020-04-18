using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Objects
{
    public class ColumnInformation
    {
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }
        public int Type { get; set; }
    }
}
