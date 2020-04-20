using Microsoft.VisualStudio.TestTools.UnitTesting;
using Website.Models;
using System;
using System.Collections.Generic;
using System.Text;
using NPoco;
using Moq;

namespace Website.Models.Tests
{
    [TestClass()]
    public class AccountTests
    {
        Mock<IDatabase> MockDb;

        [TestInitialize()]
        public void BeforeTests()
        {
            MockDb = new Mock<IDatabase>();
        }

        [TestMethod()]
        public void RegisterAccountTest()
        {
            var results = Account.RegisterAccount(MockDb.Object, new Account()
            {
                Email = "test@gmail.com",
                Password = "testpass",
                Username = "testuser"
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void RegisterAccountBadEmail()
        {
            var results = Account.RegisterAccount(MockDb.Object, new Account()
            {
                Email = "test.com",
                Password = "testpass",
                Username = "testuser"
            });

            Assert.IsFalse(results.Success);
        }

        [TestMethod()]
        public void RegisterAccountBadPassword()
        {
            var results = Account.RegisterAccount(MockDb.Object, new Account()
            {
                Email = "test.com",
                Password = "1",
                Username = "testuser"
            });

            Assert.IsFalse(results.Success);
        }

        [TestMethod()]
        public void RegisterAccountBadUsername()
        {
            var results = Account.RegisterAccount(MockDb.Object, new Account()
            {
                Email = "test.com",
                Password = "testpass",
                Username = "1"
            });

            Assert.IsFalse(results.Success);
        }

        [TestMethod()]
        public void RegisterAccountDuplicateUsername()
        {
            //Show we have a duplicate username
            MockDb.Setup(x => x.SingleOrDefault<int?>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(1);

            var results = Account.RegisterAccount(MockDb.Object, new Account()
            {
                Email = "test.com",
                Password = "testpass",
                Username = "testusername"
            });

            Assert.IsFalse(results.Success);
        }

        [TestMethod()]
        public void LoginTest()
        {
            MockDb.Setup(x => x.SingleOrDefault<Account>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(new Account());

            var results = Account.Login(MockDb.Object, new Account()
            {
                Email = "test@gmail.com",
                Password = "testpass"
            });

            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void LoginTestInvalidAccount()
        {
            var results = Account.Login(MockDb.Object, new Account()
            {
                Email = "test@gmail.com",
                Password = "testpass"
            });

            Assert.IsFalse(results.Success);
        }
    }
}