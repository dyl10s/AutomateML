using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using NPoco;
using Website.Models;
using Website.Objects;

namespace Website.Controllers
{
    [Route("api/[Controller]/[Action]")]
    public class ModelsController : Controller
    {
        IDatabase Db;
        IConfiguration Configuration;

        public ModelsController(IDatabase db, IConfiguration configuration)
        {
            Db = db;
            Configuration = configuration;
        }


        /// <summary>
        /// This gets the top 10 models by votes.
        /// </summary>
        [HttpGet]
        public ReturnResult<List<Model>> GetTopTen()
        {
            return Model.GetTopModels(Db, 10);
        }


        /// <summary>
        /// This searches for models based on title.
        /// </summary>
        [HttpGet]
        public ReturnResult<List<Model>> SearchModel(string query)
        {
            return Model.SearchModelsByName(Db, query);
        }

        /// <summary>
        /// This gets the entire model object with the modelId
        /// </summary>
        [HttpGet]
        public ReturnResult<Model> GetModel(int modelId)
        {
            return Model.GetModelById(Db, modelId);
        }

        /// <summary>
        /// [Requires Auth] This allows the user to vote for a model.
        /// </summary>
        [HttpPost, Authorize]
        public ReturnResult<bool> VoteForModel(int modelId)
        {
            return ModelVote.ToggleVote(Db, Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), modelId);
        }

        /// <summary>
        /// [Requires Auth] This checks if the user has voted for a model already
        /// </summary>
        [HttpGet, Authorize]
        public ReturnResult<bool> HasVotedForModel(int modelId)
        {
            return ModelVote.HasVotedForModel(Db, Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), modelId);
        }

        /// <summary>
        /// This gets the total number of model votes
        /// </summary>
        [HttpGet]
        public ReturnResult<int> GetModelVotes(int modelId)
        {
            return ModelVote.GetModelVotes(Db, modelId);
        }

        /// <summary>
        /// [Requires Auth] This trains a new model with the TrainInput parameter
        /// </summary>
        [HttpPost, Authorize]
        public ReturnResult<Model> StartTraining([FromBody]TrainInput input)
        {
            var results = Model.TrainModel(
                                Db, 
                                input, 
                                Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                                Configuration["ModelBuilderUrl"]);

            return results;
        }

        /// <summary>
        /// [Requires Auth] This makes a prediction on an already existing model
        /// </summary>
        [HttpPost, Authorize]
        public ReturnResult<dynamic> GetPrediction([FromBody]PredictionInput input)
        {
            var results = Model.PredictModel(Db, input);

            return results;
        }

        /// <summary>
        /// This makes a prediction on an already existing model used for intigration with other applications
        /// </summary>
        [HttpGet]
        public ReturnResult<dynamic> PublicPrediction(int modelId, string data)
        {
            var input = new PredictionInput()
            {
                CsvData = data,
                ModelId = modelId
            };

            var results = Model.PredictModel(Db, input);

            return results;
        }
    }
}