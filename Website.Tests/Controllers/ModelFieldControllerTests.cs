using Microsoft.VisualStudio.TestTools.UnitTesting;
using Website.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NPoco;
using Website.Models;

namespace Website.Controllers.Tests
{
    [TestClass()]
    public class ModelFieldControllerTests
    {
        ModelFieldController controller;
        Mock<IDatabase> MockDb;

        [TestInitialize()]
        public void BeforeTests()
        {
            MockDb = new Mock<IDatabase>();
            controller = new ModelFieldController(MockDb.Object);
        }

        [TestMethod()]
        public void GetModelFieldsTest()
        {
            var results = controller.GetModelFields(0);
            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void GetModelFieldsTestBad()
        {
            MockDb.Setup(x => x.Fetch<ModelField>(It.IsAny<string>(), It.IsAny<object[]>())).Throws(new Exception());
            var results = controller.GetModelFields(0);
            Assert.IsFalse(results.Success);
        }
    }
}