using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NPoco;
using Website.Models;
using Website.Objects;

namespace Website.Controllers
{
    [Route("api/[Controller]/[Action]")]
    public class ModelFieldController : Controller
    {
        IDatabase Db;

        public ModelFieldController(IDatabase db)
        {
            Db = db;
        }

        public ReturnResult<List<ModelField>> GetModelFields(int modelId)
        {
            return ModelField.GetFieldsByModelId(Db, modelId);
        }
    }
}