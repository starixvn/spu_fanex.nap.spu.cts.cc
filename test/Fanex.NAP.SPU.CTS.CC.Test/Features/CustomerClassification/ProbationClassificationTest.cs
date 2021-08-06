namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProbationClassification;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ProbationClassificationTest
    {
        private readonly IProbationClassificationDataAccess probationClassificationDataAccess;
        private readonly IProbationClassificationServices probationClassificationServices;

        public ProbationClassificationTest()
        {
            probationClassificationDataAccess = Substitute.For<IProbationClassificationDataAccess>();
            probationClassificationServices = new ProbationClassificationServices(probationClassificationDataAccess);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetProbationCustForScanning_WhenSuccess_ReturnProbationList(int batchSize)
        {
            var probationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=1,
                    CategoryId=1,
                    IsPassed=true,
                    IsProbationLastDay=1,
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            probationClassificationDataAccess.GetProbationCustForScanning(batchSize).Returns(probationList);

            probationClassificationServices.Start().GetProbationCustForScanning(batchSize);
            var result = probationClassificationServices.CustomerClassification;

            result.Should().BeEquivalentTo(probationList);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetProbationCustForScanning_WhenFail_ReturnEmptyList(int batchSize)
        {
            probationClassificationDataAccess.GetProbationCustForScanning(batchSize).Returns(new List<ProbationEntity>());

            probationClassificationServices.Start().GetProbationCustForScanning(batchSize);
            var result = probationClassificationServices.CustomerClassification;

            result.Should().BeEquivalentTo(new List<ProbationEntity>());
        }

        [Fact]
        public void ClassifyCategoryForProbationCust_WhenSuccess_ReturnProbationList()
        {
            var probationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=1,
                    CategoryId=1,
                    IsPassed=true,
                    IsProbationLastDay=1,
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            probationClassificationDataAccess.ClassifyCategoryForProbationCust(probationList).Returns(probationList);

            var result = probationClassificationServices.ClassifyCategoryForProbationCust(probationList);

            result.Should().BeEquivalentTo(probationList);
        }

        [Fact]
        public void InsertProbationNotification_WhenSuccess_ReturnNotException()
        {
            var probationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=1,
                    CategoryId=1,
                    IsPassed=true,
                    IsProbationLastDay=1,
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            var failProbationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            probationClassificationDataAccess.InsertProbationNotification(probationList, failProbationList);

            Action action = () => probationClassificationServices.InsertProbationNotification(probationList, failProbationList);
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void ScanningProbationClassification_WhenSuccess_ReturnNotException()
        {
            var probationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=1,
                    CategoryId=1,
                    IsPassed=true,
                    IsProbationLastDay=1,
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            var failProbationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            probationClassificationDataAccess.GetProbationCustForScanning(1).Returns(probationList);
            probationClassificationDataAccess.ClassifyCategoryForProbationCust(probationList).Returns(probationList);
            probationClassificationDataAccess.InsertProbationNotification(probationList, failProbationList);

            probationClassificationServices.GetProbationCustForScanning(1);
            Action action = () => probationClassificationServices.ScanningProbationClassification();
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateLastScanDateForProbationCust_WhenSuccess_ReturnNotException()
        {
            var probationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=1,
                    CategoryId=1,
                    IsPassed=true,
                    IsProbationLastDay=1,
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    CategoryId=2,
                    IsPassed=false,
                    IsProbationLastDay=2,
                    SportGroupId=2
               },
            };

            var dataJson = JsonConvert.SerializeObject(probationList);

            probationClassificationDataAccess.GetProbationCustForScanning(1).Returns(probationList);
            probationClassificationDataAccess.UpdateLastScanDateForProbationCust(dataJson);

            probationClassificationServices.GetProbationCustForScanning(1);
            Action action = () => probationClassificationServices.UpdateLastScanDateForProbationCust();

            action.Should().NotThrow<Exception>();
        }
    }
}