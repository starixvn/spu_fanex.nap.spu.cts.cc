namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class NormalAccountClassificationBySportServiceTest
    {
        private readonly INormalAccountClassificationBySportDataAccess normalAccountClassifyDataAccess;
        private readonly INormalAccountClassificationBySportService normalAccountClassifyService;

        public NormalAccountClassificationBySportServiceTest()
        {
            normalAccountClassifyDataAccess = Substitute.For<INormalAccountClassificationBySportDataAccess>();
            normalAccountClassifyService = new NormalAccountClassificationBySportService(null, normalAccountClassifyDataAccess);
        }

        [Fact]
        public void GetNormalPoolCustomers_WhenSuccess_ReturnData()
        {
            // Arrange

            // Act
            normalAccountClassifyService.GetNormalPoolCustomers();

            // Assert
            normalAccountClassifyDataAccess.Received(1).GetNormalPoolCustomers();
        }

        [Fact]
        public void GetCurrentCustomerCategory_WhenSuccess_ReturnData()
        {
            // Arrange
            var custSportList = new List<CustSportModel>()
            {
                new CustSportModel { CustId = 1, SportGroup = 1 },
                new CustSportModel { CustId = 1, SportGroup = 2 }
            };

            var custSportJson = JsonConvert.SerializeObject(custSportList);

            // Act
            normalAccountClassifyService.GetCurrentCustomerCategory(custSportJson);

            // Assert
            normalAccountClassifyDataAccess.Received(1).GetCurrentCustomerCategory(custSportJson);
        }

        [Fact]
        public void ClassifyNormalPoolCustomers_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custInfos = new List<GetCurrentCustomerCategoryBySportModel>() 
            {
                new GetCurrentCustomerCategoryBySportModel 
                {
                     CustID = 1, SportGroup = 1, CategoryID =1, IsCheckBetCount = false, IsProbationLastDay = false
                }
            };
            var custInfoXml = custInfos.ToXmlString();
           
            // Act
            normalAccountClassifyService.ClassifyNormalPoolCustomers(custInfoXml);

            // Assert
            normalAccountClassifyDataAccess.Received(1).ClassifyNormalPoolCustomers(custInfoXml);
        }

        [Fact]
        public void ClearNormalPoolCustomers_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custInfos = new List<GetCurrentCustomerCategoryBySportModel>()
            {
                new GetCurrentCustomerCategoryBySportModel
                {
                     CustID = 1, SportGroup = 1, CategoryID =1, IsCheckBetCount = false, IsProbationLastDay = false
                }
            };
            var custInfoXml = custInfos.ToXmlString();
            var scannedMaxId = 7;

            // Act
            normalAccountClassifyService.ClearNormalPoolCustomers(custInfoXml, scannedMaxId);

            // Assert
            normalAccountClassifyDataAccess.Received(1).ClearNormalPoolCustomers(custInfoXml, scannedMaxId);
        }

    }
}
