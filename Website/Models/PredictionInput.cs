using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class PredictionInput
    {
        public int ModelId { get; set; }
        public string CsvData { get; set; }
    }
}
