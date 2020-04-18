using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NPoco;
using Website.Models;

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

        public JsonResult GetModelFields(int modelId)
        {
            return Json(ModelField.GetFieldsByModelId(Db, modelId));
        }
    }
}