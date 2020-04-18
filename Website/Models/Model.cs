using Microsoft.ML;
using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Website.Objects;
using System.Dynamic;
using Microsoft.ML.Data;

namespace Website.Models
{
    [PrimaryKey("ModelId")]
    public class Model
    {
        public int ModelId { get; set; }
        [StringLength(100, ErrorMessage = "The model name should be no longer then 100 characters.")]
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? FileStoreId { get; set; }
        public double Accuracy { get; set; }
        public int Rows { get; set; }
        [StringLength(500, ErrorMessage = "The description should be no longer then 500 characters.")]
        public string Description { get; set; }
        [ResultColumn]
        public string Username { get; set; }

        public static ReturnResult<Model> InsertUpdate(IDatabase db, Model model)
        {
            var results = new ReturnResult<Model>();

            try
            {
                if (model.ModelId == 0)
                {
                    db.Insert(model);
                }
                else
                {
                    db.Update(model);
                }

                results.Success = true;
                results.Item = model;
            }
            catch (Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error saving the model.";
                results.Exception = e;
            }

            return results;
        }
        
        public static ReturnResult<List<Model>> SearchModelsByName(IDatabase db, string query)
        {
            ReturnResult<List<Model>> results = new ReturnResult<List<Model>>();

            try
            {
                results.Item = db.Fetch<Model>
                    ("SELECT m.*, a.Username FROM Model m JOIN Account a ON a.AccountId = m.CreatedBy WHERE Name LIKE @0", "%" + query + "%");

                results.Success = true;
            }
            catch(Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error completing the search";
                results.Exception = e;
            }

            return results;
        }

        public static ReturnResult<dynamic> PredictModel(IDatabase db, PredictionInput input)
        {
            var results = new ReturnResult<dynamic>();
            var fileName = Guid.NewGuid().ToString();

            try
            {
                var model = GetModelById(db, input.ModelId);
                var modelInfo = ModelField.GetFieldsByModelId(db, model.Item.ModelId);
                var modelFile = FileStore.GetById(db, model.Item.FileStoreId.Value);

                MLContext mlContext = new MLContext();
                DataViewSchema predictionPipelineSchema;

                IDataView LoadedData = null;

                var columnData = new List<TextLoader.Column>();
                var curItem = 0;
                foreach (var c in modelInfo.Item)
                {
                    var newColData = new TextLoader.Column()
                    {
                        DataKind = (DataKind)c.DataTypeId,
                        Name = c.Name,
                        Source = new TextLoader.Range[] { new TextLoader.Range(curItem) }
                    };

                    columnData.Add(newColData);
                    curItem++;
                }

                File.WriteAllText(Path.GetTempPath() + fileName, input.CsvData);

                LoadedData = mlContext.Data.LoadFromTextFile(
                    Path.GetTempPath() + fileName,
                    columnData.ToArray(),
                    separatorChar: ',',
                    hasHeader: false,
                    allowQuoting: true
                    );

                var outputColumn = modelInfo.Item.Single(x => x.IsOutput == true);

                using (MemoryStream ms = new MemoryStream(modelFile.Item.Data))
                {
                    var predictionPipeline = mlContext.Model.Load(ms, out predictionPipelineSchema);
                    IDataView predictions = predictionPipeline.Transform(LoadedData);

                    if((DataKind)outputColumn.DataTypeId == DataKind.Single)
                    {
                        var hasLabel = predictions.Schema.GetColumnOrNull("PredictedLabel");
                        Single[] predictionOut = null;
                        if(hasLabel == null)
                        {
                            predictionOut = predictions.GetColumn<Single>("Score").ToArray();
                        }
                        else
                        {
                            predictionOut = predictions.GetColumn<Single>("PredictedLabel").ToArray();
                        }
                        results.Item = predictionOut[0];
                    }
                    else if((DataKind)outputColumn.DataTypeId == DataKind.String)
                    {
                        var predictionOut = predictions.GetColumn<String>("PredictedLabel").ToArray();
                        results.Item = predictionOut[0];
                    }
                    else if ((DataKind)outputColumn.DataTypeId == DataKind.Boolean)
                    {
                        var predictionOut = predictions.GetColumn<Boolean>("PredictedLabel").ToArray();
                        results.Item = predictionOut[0];
                    }
                };

                results.Success = true;
            }
            catch(Exception e)
            {
                results.Success = false;
            }


            //Delete the prediction file
            try
            {
                if(File.Exists(Path.GetTempPath() + fileName))
                {
                    File.Delete(Path.GetTempPath() + fileName);
                }
            }
            catch (Exception e){}

            return results;
        }

        public static ReturnResult<Model> GetModelById(IDatabase db, int modelId)
        {
            var results = new ReturnResult<Model>();

            try
            {
                results.Item = db.Single<Model>
                    ("SELECT m.*, a.Username FROM Model m JOIN Account a ON a.AccountId = m.CreatedBy WHERE ModelId = @0", modelId);
                results.Success = true;
            }
            catch(Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "There was an error getting the model";
                results.Exception = e;
            }

            return results;
        }

        public static ReturnResult<List<Model>> GetTopModels(IDatabase db, int modelCount)
        {
            ReturnResult<List<Model>> results = new ReturnResult<List<Model>>();

            try
            {
                results.Item = db.Fetch<Model>(@$"
                        SELECT TOP {modelCount} m.*, COUNT(mv.ModelVoteId) as 'Votes' FROM Model m LEFT JOIN ModelVote mv ON mv.ModelId = m.ModelId 
                        GROUP BY m.ModelId, m.FileStoreId, m.Accuracy, m.CreatedBy, m.CreatedOn, m.[Description], m.[Name], m.[Rows]
                        ORDER BY COUNT(mv.ModelVoteId) DESC
                        ");
                results.Success = true;
            }
            catch(Exception e)
            {
                results.Exception = e;
                results.ErrorMessage = "Error loading top models";
                results.Success = false;
            }

            return results;
            
        }

        public static ReturnResult<Model> TrainModel(IDatabase db, TrainInput input, int curUserId, string TrainModelURL)
        {
            ReturnResult<Model> modelBuilderResults = null;
            using (WebClient client = new WebClient())
            {
                modelBuilderResults = JsonConvert.DeserializeObject<ReturnResult<Model>>
                    (client.UploadString(new Uri(TrainModelURL), JsonConvert.SerializeObject(input)));
            }

            if (!modelBuilderResults.Success)
            {
                return modelBuilderResults;
            }

            //Save the model to the server and save details to the database
            Model curModel = new Model()
            {
                CreatedBy = curUserId,
                CreatedOn = DateTime.UtcNow,
                Name = input.Title,
                FileStoreId = modelBuilderResults.Item.FileStoreId,
                Accuracy = modelBuilderResults.Item.Accuracy,
                Rows = modelBuilderResults.Item.Rows,
                Description = input.Description
            };

            var modelInsertReults = Model.InsertUpdate(db, curModel);

            if (modelInsertReults.Success == false)
            {
                return modelInsertReults;
            }

            List<ModelField> modelFields = new List<ModelField>();

            foreach (var col in input.Columns)
            {
                modelFields.Add(new ModelField()
                {
                    DataTypeId = col.Type,
                    IsOutput = col.ColumnName == input.LabelColumn,
                    ModelId = curModel.ModelId,
                    Name = col.ColumnName
                });
            }

            var insertModelFieldsResults = ModelField.InsertBulk(db, modelFields);

            if (insertModelFieldsResults.Success)
            {
                return modelInsertReults;
            }
            else
            {
                return new ReturnResult<Model>()
                {
                    Success = false,
                    Exception = insertModelFieldsResults.Exception,
                    ErrorMessage = insertModelFieldsResults.ErrorMessage
                };
            }
        }
    }
}
