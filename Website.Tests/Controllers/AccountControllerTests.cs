using Microsoft.VisualStudio.TestTools.UnitTesting;
using Website.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using NPoco;
using Moq;
using Website.Services;
using Website.Models;

namespace Website.Controllers.Tests
{
    [TestClass()]
    public class AccountControllerTests
    {
        AccountController controller;
        Mock<IDatabase> MockDb;
        Mock<ITokenBuilder> TokenBuilder;

        [TestInitialize()]
        public void BeforeTests()
        {
            MockDb = new Mock<IDatabase>();
            TokenBuilder = new Mock<ITokenBuilder>();
            controller = new AccountController(MockDb.Object, TokenBuilder.Object);
        }

        [TestMethod()]
        public void RegisterTest()
        {
            var results = controller.Register(new Models.Account()
            {
                Email = "test@gmail.com",
                Password = "testpass",
                Username = "testuser"
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void RegisterTestBad()
        {
            var results = controller.Register(new Models.Account()
            {
                Email = "",
                Password = "",
                Username = ""
            });

            Assert.IsFalse(results.Success);
        }

        [TestMethod()]
        public void LoginTest()
        {
            MockDb.Setup(x => x.SingleOrDefault<Account>
                (It.IsAny<string>(), It.IsAny<object[]>())).Returns(new Account()
                {
                    Username = "test"
                });

            var results = controller.Login(new Models.Account()
            {
                Password = "testpass",
                Username = "testuser"
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void LoginTestBad()
        {
            var results = controller.Login(new Models.Account()
            {
                Password = "testpass",
                Username = "testuser"
            });

            Assert.IsFalse(results.Success);
        }
    }
}