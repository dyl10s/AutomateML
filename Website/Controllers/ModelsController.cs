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

        public ReturnResult<List<Model>> GetTopTen()
        {
            return Model.GetTopModels(Db, 10);
        }

        public ReturnResult<List<Model>> SearchModel(string query)
        {
            return Model.SearchModelsByName(Db, query);
        }

        public ReturnResult<Model> GetModel(int modelId)
        {
            return Model.GetModelById(Db, modelId);
        }

        [HttpPost, Authorize]
        public ReturnResult<bool> VoteForModel(int modelId)
        {
            return ModelVote.ToggleVote(Db, Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), modelId);
        }

        [HttpGet, Authorize]
        public ReturnResult<bool> HasVotedForModel(int modelId)
        {
            return ModelVote.HasVotedForModel(Db, Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), modelId);
        }

        [HttpGet]
        public ReturnResult<int> GetModelVotes(int modelId)
        {
            return ModelVote.GetModelVotes(Db, modelId);
        }

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

        [HttpPost, Authorize]
        public ReturnResult<dynamic> GetPrediction([FromBody]PredictionInput input)
        {
            var results = Model.PredictModel(Db, input);

            return results;
        }
    }
}