namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RealtimeTWInitialSmartService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class TWInitialSmartServiceTest
    {
        private readonly ITWInitialSmartDataAccess twInitialSmartDataAccess;
        private readonly ITWInitialSmartService twInitialSmartService;

        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;

        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        
        public TWInitialSmartServiceTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>(); 
            problemAccountServices = new ProblemAccountServices(Substitute.For<IBaseCustomerClassificationDataAccess>(), problemAccountDataAccess, null);

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, null);

            twInitialSmartDataAccess = Substitute.For<ITWInitialSmartDataAccess>();
            twInitialSmartService = new TWInitialSmartService(problemAccountServices, custClassToMainDBService, twInitialSmartDataAccess);
        }

        [Fact]
        public void GetCustInitialSmart_WhenSuccess_ReturnValue()
        {
            // Arrange
            var expectedResult = new TWInitialSmartModel() { CustInfos = new List<TWInitialSmartEntity>() };

            // Act
            twInitialSmartDataAccess.GetCustInitialSmart().Returns(expectedResult);

            var result = twInitialSmartService.GetCustInitialSmart();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void InsertTWInitialSmartClassification_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var twInitialSmartCusts = new List<TWInitialSmartEntity>()
            {
                 new TWInitialSmartEntity
                 {
                     CustID = 1, CTSCustID = 1, CategoryID = 1, CategoryGroup = 1, DetectedDate = DateTime.Now,
                     IsLicensee = true, RoleID = 1, Probability = new decimal(0.8971), SubscriberID = 6
                 }
            };
           
            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = 500,              
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "Realtime TW Initial Smart JobService"
            };

            // Act
            var result = twInitialSmartService.InsertTWInitialSmartClassification(twInitialSmartCusts, paras);

            // Assert
            result.Should().BeTrue();           
        }
    }
}
