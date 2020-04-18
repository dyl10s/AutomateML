using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Objects;

namespace Website.Models
{
    [PrimaryKey("ModelFieldId")]
    public class ModelField
    {
        public int ModelFieldId { get; set; }
        public int ModelId { get; set; }
        public string Name { get; set; }
        public int DataTypeId { get; set; }
        public bool IsOutput { get; set; }

        public static ReturnResult<ModelField> InsertUpdate(IDatabase db, ModelField modelField)
        {
            var results = new ReturnResult<ModelField>();

            try
            {
                if (modelField.ModelFieldId == 0)
                {
                    db.Insert(modelField);
                }
                else
                {
                    db.Update(modelField);
                }

                results.Success = true;
                results.Item = modelField;
            }
            catch (Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error saving the model.";
                results.Exception = e;
            }

            return results;
        }

        public static ReturnResult<List<ModelField>> GetFieldsByModelId(IDatabase db, int modelId)
        {
            var results = new ReturnResult<List<ModelField>>();

            try
            {
                results.Item = db.Fetch<ModelField>("SELECT * FROM ModelField WHERE ModelId = @0", modelId);
                results.Success = true;
            }
            catch(Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error getting the model fields";
                results.Exception = e;
            }

            return results;
        }

        public static ReturnResult<List<ModelField>> InsertBulk(IDatabase db, List<ModelField> modelFields)
        {
            var results = new ReturnResult<List<ModelField>>();

            try
            {
                db.InsertBulk(modelFields);

                results.Success = true;
                results.Item = modelFields;
            }
            catch (Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error saving the model.";
                results.Exception = e;
            }

            return results;
        }
    }
}
