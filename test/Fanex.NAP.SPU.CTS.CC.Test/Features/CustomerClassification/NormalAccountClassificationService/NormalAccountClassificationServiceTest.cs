namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalAccountClassificationService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class NormalAccountClassificationServiceTest
    {
        private readonly INormalAccountClassificationDataAccess normalAccountDataAccess;
        private readonly INormalAccountClassificationService normalAccountService;

        public NormalAccountClassificationServiceTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountClassificationDataAccess>();
            normalAccountService = new NormalAccountClassificationService(null, normalAccountDataAccess);
        }

        [Fact]
        public void GetNormalPoolCustomers_WhenSuccess()
        {
            // Arrange

            // Act
            normalAccountService.GetNormalPoolCustomers();

            // Assert
            normalAccountDataAccess.Received(1).GetNormalPoolCustomers();
        }

        [Fact]
        public void GetCurrentCustomerCategory_WhenSuccess()
        {
            // Arrange
            var custIDList = "1,2";

            // Act
            normalAccountService.GetCurrentCustomerCategory(custIDList);

            // Assert
            normalAccountDataAccess.Received(1).GetCurrentCustomerCategory(custIDList);
        }

        [Fact]
        public void ClassifyNormalPoolCustomers_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custInfos = new List<GetCurrentCustomerCategoryModel>() {
                new GetCurrentCustomerCategoryModel {
                     CustId = 1, CategoryId = 1, IsCheckBetCount = true, IsNewCreated = false
                }
            };
           
            var custInfoXml = custInfos.ToXmlString();

            // Act
            normalAccountService.ClassifyNormalPoolCustomers(custInfoXml);

            // Assert
            normalAccountDataAccess.Received(1).ClassifyNormalPoolCustomers(custInfoXml);
        }

        [Fact]
        public void ClearNormalPoolCustomers_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custIDList = "1,2";
            var scannedMaxId = 10;

            // Act
            normalAccountService.ClearNormalPoolCustomers(custIDList, scannedMaxId);

            // Assert
            normalAccountDataAccess.Received(1).ClearNormalPoolCustomers(custIDList, scannedMaxId);
        }
    }
}
