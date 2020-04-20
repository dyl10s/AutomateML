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
    public class ModelVoteTests
    {
        Mock<IDatabase> MockDb;

        [TestInitialize()]
        public void BeforeTests()
        {
            MockDb = new Mock<IDatabase>();
        }

        [TestMethod()]
        public void GetModelVotesTest()
        {
            MockDb.Setup(x => x.Single<int>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(2);
            var results = ModelVote.GetModelVotes(MockDb.Object, 1);
            Assert.AreEqual(2, results.Item);
        }

        [TestMethod()]
        public void HasVotedForModelTestYes()
        {
            MockDb.Setup(x => x.Single<int>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(1);
            var results = ModelVote.HasVotedForModel(MockDb.Object, 1, 1);
            Assert.IsTrue(results.Item);
        }

        [TestMethod()]
        public void HasVotedForModelTestNo()
        {
            MockDb.Setup(x => x.Single<int>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(0);
            var results = ModelVote.HasVotedForModel(MockDb.Object, 1, 1);
            Assert.IsFalse(results.Item);
        }

        [TestMethod()]
        public void ToggleVoteTestRemove()
        {
            MockDb.Setup(x => x.Single<int>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(1);
            var results = ModelVote.ToggleVote(MockDb.Object, 1, 1);
            Assert.IsFalse(results.Item);
        }

        [TestMethod()]
        public void ToggleVoteTestAdd()
        {
            MockDb.Setup(x => x.Single<int>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(0);
            var results = ModelVote.ToggleVote(MockDb.Object, 1, 1);
            Assert.IsTrue(results.Item);
        }
    }
}