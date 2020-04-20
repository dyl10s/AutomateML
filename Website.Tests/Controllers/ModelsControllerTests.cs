using Microsoft.VisualStudio.TestTools.UnitTesting;
using Website.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using NPoco;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Website.Models;
using Microsoft.ML.Data;

namespace Website.Controllers.Tests
{
    [TestClass()]
    public class ModelsControllerTests
    {
        ModelsController controller;
        Mock<IDatabase> MockDb;
        Mock<IConfiguration> MockConfig;
        ClaimsPrincipal User;

        [TestInitialize()]
        public void BeforeTests()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "mock"));

            MockDb = new Mock<IDatabase>();
            MockConfig = new Mock<IConfiguration>();
            controller = new ModelsController(MockDb.Object, MockConfig.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = User }
            };
        }

        [TestMethod()]
        public void GetTopTenTest()
        {
            Assert.IsTrue(controller.GetTopTen().Success);
        }

        [TestMethod()]
        public void SearchModelTest()
        {
            Assert.IsTrue(controller.SearchModel("test").Success);
        }

        [TestMethod()]
        public void GetModelTest()
        {
            Assert.IsTrue(controller.GetModel(1).Success);
        }

        [TestMethod()]
        public void VoteForModelTest()
        {
            Assert.IsTrue(controller.VoteForModel(1).Success);
        }

        [TestMethod()]
        public void HasVotedForModelTest()
        {
            Assert.IsTrue(controller.HasVotedForModel(1).Success);
        }

        [TestMethod()]
        public void GetModelVotesTest()
        {
            Assert.IsTrue(controller.GetModelVotes(1).Success);
        }

        [TestMethod()]
        //This needs a valid endpoint to run so we can assume it should always fail
        //indepth tests should be done manually
        public void StartTrainingTest()
        {
            MockConfig.Setup(x => x[It.IsAny<string>()]).Returns("");
            Assert.IsFalse(controller.StartTraining(new Models.TrainInput()).Success);
        }

        [TestMethod()]
        //This needs a valid model to run so we can assume it should always fail
        //indepth tests should be done manually
        public void GetPredictionTest()
        {
            Assert.IsFalse(controller.GetPrediction(new Models.PredictionInput()).Success);
        }
    }
}