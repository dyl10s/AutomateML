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
    public class ModelFieldTests
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
            var results = ModelField.InsertUpdate(MockDb.Object, new ModelField());
            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void InsertUpdateTestUpdate()
        {
            var results = ModelField.InsertUpdate(MockDb.Object, new ModelField()
            {
                ModelId = 1
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void GetFieldsByModelIdTest()
        {
            MockDb.Setup(x => x.Fetch<ModelField>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(new List<ModelField>()
            {
                new ModelField()
                {
                    ModelFieldId = 1
                }
            });

            var results = ModelField.GetFieldsByModelId(MockDb.Object, 1);
            Assert.AreEqual(1, results.Item[0].ModelFieldId);
        }

        [TestMethod()]
        public void InsertBulkTest()
        {
            var results = ModelField.InsertBulk(MockDb.Object, 
                new List<ModelField>()
                {
                    new ModelField()
                    {
                        ModelId = 1
                    }
                }
            );

            Assert.IsTrue(results.Success);
        }
    }
}