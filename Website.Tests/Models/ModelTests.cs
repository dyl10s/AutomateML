using Microsoft.VisualStudio.TestTools.UnitTesting;
using Website.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NPoco;

namespace Website.Models.Tests
{
    [TestClass()]
    public class ModelTests
    {
        Mock<IDatabase> MockDb;

        [TestInitialize()]
        public void BeforeTests()
        {
            MockDb = new Mock<IDatabase>();
        }

        [TestMethod()]
        public void InsertUpdateTestInsert()
        {
            var results = Model.InsertUpdate(MockDb.Object, new Model());
            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void InsertUpdateTestUpdate()
        {
            var results = Model.InsertUpdate(MockDb.Object, new Model()
            {
                ModelId = 1
            });
            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void SearchModelsByNameTest()
        {
            MockDb.Setup(x => x.Fetch<Model>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(new List<Model>()
            {
                new Model()
                {
                    ModelId = 100
                }
            });

            var results = Model.SearchModelsByName(MockDb.Object, "test");
            Assert.AreEqual(100, results.Item[0].ModelId);
        }

        //This needs a valid model to run so we can assume it should always fail
        //indepth tests should be done manually
        [TestMethod()]
        public void PredictModelTest()
        {
            var results = Model.PredictModel(MockDb.Object, new PredictionInput());
            Assert.IsFalse(results.Success);
        }

        [TestMethod()]
        public void GetModelByIdTest()
        {
            MockDb.Setup(x => x.Single<Model>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(new Model()
            {
                ModelId = 100
            });

            var results = Model.GetModelById(MockDb.Object, 100);
            Assert.AreEqual(100, results.Item.ModelId);
        }

        [TestMethod()]
        public void GetTopModelsTest()
        {
            MockDb.Setup(x => x.Fetch<Model>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(new List<Model>()
            {
                new Model()
                {
                    ModelId = 100
                }
            });

            var results = Model.GetTopModels(MockDb.Object, 1);
            Assert.AreEqual(100, results.Item[0].ModelId);
        }

        //This needs a valid endpoint to run so we can assume it should always fail
        //indepth tests should be done manually
        [TestMethod()]
        public void TrainModelTest()
        {
            var results = Model.TrainModel(MockDb.Object, new TrainInput(), 1, "");
            Assert.IsFalse(results.Success);
        }
    }
}