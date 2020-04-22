using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.AutoML;
using System.Collections.Generic;
using Website.Objects;
using NPoco;
using System.Data.SqlClient;
using Website.Models;

namespace ModelBuilderServerless
{
    public static class Train
    {
        //When debugging this is stored in local.settings.json under a subsection "Values"
        public static IDatabase db = new Database(
                    $"Data Source={Environment.GetEnvironmentVariable("DatabaseServer")};Initial Catalog={Environment.GetEnvironmentVariable("DatabaseName")};User ID={Environment.GetEnvironmentVariable("DatabaseUsername")};Password={Environment.GetEnvironmentVariable("DatabasePassword")};MultipleActiveResultSets=True;",
                    DatabaseType.SqlServer2012,
                    SqlClientFactory.Instance
                    );

        [FunctionName("Train")]
        public static ReturnResult<Model> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            var dataFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                db.BeginTransaction();

                MLContext context = new MLContext();

                TrainInput input = null;

                using (StreamReader reader = new StreamReader(req.Body))
                {
                    input = JsonConvert.DeserializeObject<TrainInput>(reader.ReadToEnd());
                }

                File.WriteAllText(dataFilePath, input.Data);
                
                IDataView LoadedData = null;

                var columnData = new List<TextLoader.Column>();
                foreach (var c in input.Columns)
                {
                    //data type 1 is for ignore
                    if (c.Type != 1)
                    {
                        var newColData = new TextLoader.Column()
                        {
                            DataKind = (DataKind)c.Type,
                            Name = c.ColumnName,
                            Source = new TextLoader.Range[] { new TextLoader.Range(c.ColumnIndex) }
                        };

                        columnData.Add(newColData);
                    }
                }

                LoadedData = context.Data.LoadFromTextFile(
                    dataFilePath,
                    columnData.ToArray(),
                    separatorChar: input.Separator,
                    hasHeader: input.HasHeaders,
                    allowQuoting: true
                    );

                LoadedData = context.Data.ShuffleRows(LoadedData);

                /*
                 * Multiclass will be used in the case of binary experiments and multiclass experiments.
                 * This is because multiclass can accept all types as an output column. This will
                 * allow less interaction with the user and a better user experience.
                 */

                double bestRunMetric = 0;
                ITransformer bestModel = null;

                if(input.ModelType == TrainInput.ModelTypes.Multiclass)
                {
                    ExperimentResult<MulticlassClassificationMetrics> Results = null;
                    var settings = new MulticlassExperimentSettings()
                    {
                        MaxExperimentTimeInSeconds = 20
                    };
                    var training = context.Auto().CreateMulticlassClassificationExperiment(settings);
                    Results = training.Execute(LoadedData, labelColumnName: input.LabelColumn);
                    bestRunMetric = Results.BestRun.ValidationMetrics.MacroAccuracy;
                    bestModel = Results.BestRun.Model;
                }
                else if(input.ModelType == TrainInput.ModelTypes.Binary)
                {
                    ExperimentResult<BinaryClassificationMetrics> Results = null;
                    var settings = new BinaryExperimentSettings()
                    {
                        MaxExperimentTimeInSeconds = 20
                    };
                    var training = context.Auto().CreateBinaryClassificationExperiment(settings);
                    Results = training.Execute(LoadedData, labelColumnName: input.LabelColumn);
                    bestRunMetric = Results.BestRun.ValidationMetrics.Accuracy;
                    bestModel = Results.BestRun.Model;
                }
                else if(input.ModelType == TrainInput.ModelTypes.Regression)
                {
                    ExperimentResult<RegressionMetrics> Results = null;
                    var settings = new RegressionExperimentSettings()
                    {
                        MaxExperimentTimeInSeconds = 20
                    };
                    var training = context.Auto().CreateRegressionExperiment(settings);
                    Results = training.Execute(LoadedData, labelColumnName: input.LabelColumn);
                    bestRunMetric = Results.BestRun.ValidationMetrics.RSquared;
                    bestModel = Results.BestRun.Model;
                    if(bestRunMetric < 0)
                    {
                        bestRunMetric = 0;
                    }
                }
                else
                {
                    throw new Exception("Invalid model type");
                }
                

                var modelFileId = 0;

                using(MemoryStream ms = new MemoryStream())
                {
                    context.Model.Save(bestModel, LoadedData.Schema, ms);
                    //Save model to the database
                    FileStore modelSave = new FileStore()
                    {
                        Data = ms.ToArray()
                    };

                    modelFileId = FileStore.InsertUpdate(db, modelSave).Item.FileStoreId;
                }

                var resultModel = new Model()
                {
                    FileStoreId = modelFileId,
                    Accuracy = bestRunMetric,
                    Rows = input.Data.Trim().Split('\n').Length
                };

                db.CompleteTransaction();

                return new ReturnResult<Model>()
                {
                    Success = true,
                    Item = resultModel
                };
            }
            catch(Exception e)
            {
                db.AbortTransaction();
                log.LogError(e.Message);
                return new ReturnResult<Model>()
                {
                    Success = false,
                    Exception = e
                };
            }
        }
    }
}
