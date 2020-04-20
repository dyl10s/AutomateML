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
    public class FileStoreTests
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
            var results = FileStore.InsertUpdate(MockDb.Object, new FileStore()
            {
                FileStoreId = 1,
                Data = new byte[] { }
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void InsertUpdateTestUpdate()
        {
            var results = FileStore.InsertUpdate(MockDb.Object, new FileStore()
            {
                Data = new byte[] { }
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            MockDb.Setup(x => x.SingleById<FileStore>(1)).Returns(new FileStore()
            {
                FileStoreId = 100
            });

            var results = FileStore.GetById(MockDb.Object, 1);
            Assert.AreEqual(100, results.Item.FileStoreId);
        }

        [TestMethod()]
        public void GetByIdTestBad()
        {
            MockDb.Setup(x => x.SingleById<FileStore>(1)).Throws(new Exception());

            var results = FileStore.GetById(MockDb.Object, 1);
            Assert.IsFalse(results.Success);
        }
    }
}