using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelBuilderServerless;
using Moq;
using Newtonsoft.Json;
using NPoco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Website.Models;
using Website.Objects;

namespace ModelBuilderServerless.Tests
{
    [TestClass()]
    public class TrainTests
    {
        [TestMethod()]
        public void RunTestMulticlass()
        {
            Mock<ILogger> Logger = new Mock<ILogger>();
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            Mock<IDatabase> Db = new Mock<IDatabase>();

            TrainInput trainingInput = new TrainInput()
            {
                Columns = new List<ColumnInformation>()
                {
                    new ColumnInformation()
                    {
                        ColumnIndex = 0,
                        ColumnName = "Id",
                        Type = 1 //ignore
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 1,
                        ColumnName = "SepalLengthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 2,
                        ColumnName = "SepalWidthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 3,
                        ColumnName = "PetalLengthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 4,
                        ColumnName = "PetalWidthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 5,
                        ColumnName = "Species",
                        Type = (int)DataKind.String
                    }
                },
                Data = File.ReadAllText("Iris.csv"),
                HasHeaders = true,
                Description = "test",
                LabelColumn = "Species",
                ModelType = TrainInput.ModelTypes.Multiclass,
                Separator = ',',
                Title = "Iris Test"
            };

            var dataString = JsonConvert.SerializeObject(trainingInput);
            MemoryStream dataStream = new MemoryStream(Encoding.ASCII.GetBytes(dataString));

            request.Setup(x => x.Body).Returns(dataStream);

            Train.db = Db.Object;

            var results = Train.Run(request.Object, Logger.Object);
            Assert.IsTrue(results.Success);
        }

        [TestMethod()]
        public void RunTestBad()
        {
            Mock<ILogger> Logger = new Mock<ILogger>();
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            Mock<IDatabase> Db = new Mock<IDatabase>();

            TrainInput trainingInput = new TrainInput()
            {
                Columns = new List<ColumnInformation>()
                {
                    new ColumnInformation()
                    {
                        ColumnIndex = 0,
                        ColumnName = "Id",
                        Type = 1 //ignore
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 1,
                        ColumnName = "SepalLengthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 2,
                        ColumnName = "SepalWidthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 3,
                        ColumnName = "PetalLengthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 4,
                        ColumnName = "PetalWidthCm",
                        Type = (int)DataKind.Single
                    },
                    new ColumnInformation()
                    {
                        ColumnIndex = 5,
                        ColumnName = "Species",
                        Type = (int)DataKind.Single // Wrong data type
                    }
                },
                Data = File.ReadAllText("Iris.csv"),
                HasHeaders = true,
                Description = "test",
                LabelColumn = "Species",
                ModelType = TrainInput.ModelTypes.Multiclass,
                Separator = ',',
                Title = "Iris Test"
            };

            var dataString = JsonConvert.SerializeObject(trainingInput);
            MemoryStream dataStream = new MemoryStream(Encoding.ASCII.GetBytes(dataString));

            request.Setup(x => x.Body).Returns(dataStream);

            Train.db = Db.Object;

            var results = Train.Run(request.Object, Logger.Object);
            Assert.IsFalse(results.Success);
        }
    }
}