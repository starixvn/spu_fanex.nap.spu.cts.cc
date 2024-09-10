namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
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
            probationClassificationServices = new ProbationClassificationServices(probationClassificationDataAccess, Substitute.For<ISystemParameterDataAccess>());
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
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    SportGroupId=2
               },
            };

            var probationModel = new ProbationModel()
            {
                CustProbations = probationList
            };

            probationClassificationDataAccess.GetProbationCustForScanning(batchSize).Returns(probationModel);

            probationClassificationServices.Start().GetProbationCustForScanning(batchSize);

            var result = probationClassificationServices.Customers;

            result.Should().BeEquivalentTo(probationList);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetProbationCustForScanning_WhenFail_ReturnEmptyList(int batchSize)
        {
            probationClassificationDataAccess.GetProbationCustForScanning(batchSize).Returns(new ProbationModel());

            probationClassificationServices.Start().GetProbationCustForScanning(batchSize);

            var result = probationClassificationServices.Customers;

            result.Should().BeEquivalentTo(new List<ProbationEntity>());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ClassifyCategoryForProbationCust_WhenSuccess_ReturnNotException(int batchSize)
        {
            var probationList = new List<ProbationEntity>
            {
               new ProbationEntity
               {
                    CustId=1,
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    SportGroupId=2
               },
            };

            var custToString = "1,2";

            probationClassificationDataAccess.ClassifyProbation(custToString);

            probationClassificationDataAccess.GetProbationCustForScanning(batchSize).Returns(new ProbationModel());

            probationClassificationServices.Start().GetProbationCustForScanning(batchSize);

            Action action = () => probationClassificationServices.ClassifyProbation();

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
                    SportGroupId=1
               },
               new ProbationEntity
               {
                    CustId=2,
                    SportGroupId=2
               },
            };

            var probationModel = new ProbationModel()
            {
                CustProbations = probationList
            };

            var dataJson = JsonConvert.SerializeObject(probationList);

            probationClassificationDataAccess.GetProbationCustForScanning(1).Returns(probationModel);

            probationClassificationServices.GetProbationCustForScanning(1);
            Action action = () => probationClassificationServices.UpdateLastScanDate();

            action.Should().NotThrow<Exception>();
        }
    }
}