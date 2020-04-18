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

        public JsonResult GetTopTen()
        {
            return Json(Model.GetTopModels(Db, 10));
        }

        public JsonResult SearchModel(string query)
        {
            return Json(Model.SearchModelsByName(Db, query));
        }

        public JsonResult GetModel(int modelId)
        {
            return Json(Model.GetModelById(Db, modelId));
        }

        [HttpPost, Authorize]
        public JsonResult VoteForModel(int modelId)
        {
            return Json(ModelVote.ToggleVote(Db, Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), modelId));
        }

        [HttpGet, Authorize]
        public JsonResult HasVotedForModel(int modelId)
        {
            return Json(ModelVote.HasVotedForModel(Db, Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), modelId));
        }

        [HttpGet]
        public JsonResult GetModelVotes(int modelId)
        {
            return Json(ModelVote.GetModelVotes(Db, modelId));
        }

        [HttpPost, Authorize]
        public JsonResult StartTraining([FromBody]TrainInput input)
        {
            var results = Model.TrainModel(
                                Db, 
                                input, 
                                Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                                Configuration["ModelBuilderUrl"]);

            return Json(results);
        }

        [HttpPost, Authorize]
        public JsonResult GetPrediction([FromBody]PredictionInput input)
        {
            var results = Model.PredictModel(Db, input);

            return Json(results);
        }
    }
}