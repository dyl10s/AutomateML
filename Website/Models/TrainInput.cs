using System.Collections.Generic;
using Website.Objects;

namespace Website.Models
{
    public class TrainInput
    {
        public string Title { get; set; }
        public string LabelColumn { get; set; }
        public char Separator { get; set; }
        public List<ColumnInformation> Columns { get; set; }
        public string Data { get; set; }
        public bool HasHeaders { get; set; }
        public string Description { get; set; }
        public ModelTypes ModelType { get; set; }
        //This is only for telling the model builder what ID the output should be linked to
        public int ModelId { get; set; }

        public enum ModelTypes
        {
            Binary = 1,
            Multiclass = 2,
            Regression = 3
        }
    }
}
