namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class DetectUnauthorizedLoginCustomerServiceTest
    {
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private readonly IDetectUnauthorizedLoginCustomerDataAccess unAuthorizedLogindataAccess;        
        private readonly IDetectUnauthorizedLoginCustomerService unAuthorizedLoginService;

        public DetectUnauthorizedLoginCustomerServiceTest()
        {
            systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();
            unAuthorizedLogindataAccess = Substitute.For<IDetectUnauthorizedLoginCustomerDataAccess>();
            unAuthorizedLoginService = new DetectUnauthorizedLoginCustomerService(unAuthorizedLogindataAccess, systemParameterDataAccess);
        }

        [Theory]
        [InlineData(CTS_SystemParameters.UnauthorizedLogin_LastScanCustID_Credit)]
        [InlineData(CTS_SystemParameters.UnauthorizedLogin_LastScanCustID_Licensee)]
        public void GetLastScannedCustID_WhenSuccess_ReturnLastScannedID(CTS_SystemParameters paraID)
        {
            // Arrange
            var output = new SystemParameterEntity
            {
                ParameterDataType = "BIGINT UNSIGNED",
                ParameterValue = "119"
            };

            // Act
            systemParameterDataAccess.GetPreviousTimingExecution(paraID).Returns(output);

            var result = unAuthorizedLoginService.GetLastScannedCustID(paraID);

            // Assert
            result.Should().Be(int.Parse(output.ParameterValue));
            result.Should().NotBe(null);
        }

        [Theory]
        [InlineData(CTS_SystemParameters.UnauthorizedLogin_LastScanDate_Credit)]
        [InlineData(CTS_SystemParameters.UnauthorizedLogin_LastScanDate_Licensee)]
        public void GetLastScannedDate_WhenSuccess_ReturnLastScannedDate(CTS_SystemParameters paraID)
        {
            // Arrange
            var output = new SystemParameterEntity
            {
                ParameterDataType = "BIGINT UNSIGNED",
                ParameterValue = "2023-12-29 10:50:53"
            };

            // Act
            systemParameterDataAccess.GetPreviousTimingExecution(paraID).Returns(output);

            var result = unAuthorizedLoginService.GetLastScannedDate(paraID);

            // Assert
            result.Should().Be(DateTime.Parse(output?.ParameterValue));
        }

        [Theory]
        [InlineData(CTS_SystemParameters.UnauthorizedLogin_LastScanDate_Credit, "2023-12-29 10:50:53")]
        public void UpdateLastScannedID_WhenSuccess(CTS_SystemParameters paraID, string lastScannedTime)
        {
            // Arrange

            // Act
            systemParameterDataAccess.UpdateTimingExecution(lastScannedTime, paraID);

            bool isSuccess = unAuthorizedLoginService.UpdateLastScannedID(paraID, lastScannedTime);

            // Assert
            isSuccess.Should().BeTrue();
        }

        [Fact]
        public void GetCustWithFirstBetDate_WhenSuccess_ReturnData()
        {
            // Arrange
            var param = new GetCustWithFirstBetDateParam
            {
                FromDate = DateTime.Now.AddDays(-3),
                ToDate = DateTime.Now.AddDays(-1),
                BatchSize = 20000,
                IsLicensee = false,
                LastCustID = 1
            };

            var custParam = new DetectUnauthorizedLoginCustParam
            {
                FirstBetFromDay = 1,
                FirstBetToDay = 3,
                IsLicensee = false,
                LastScanCustID = CTS_SystemParameters.UnauthorizedLogin_LastScanCustID_Credit,
                LastScanDate = CTS_SystemParameters.UnauthorizedLogin_LastScanDate_Credit
            };

            var expectedCustWithFirstBetDate = new List<CustFirstBetDateEntity>() { };

            // Act
            unAuthorizedLogindataAccess.GetCustWithFirstBetDate(param).Returns(expectedCustWithFirstBetDate);

            var result = unAuthorizedLoginService.GetCustWithFirstBetDate(custParam, DateTime.Now);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCustWithFirstBetDate);
        }

        [Fact]
        public void DetectUnauthorizedLogin_WhenSuccess_ReturnData()
        {
            // Arrange
            var custJson = "";
            var expectedResult = new List<DetectUnauthorizedLoginCustEntity>()
            {
                new DetectUnauthorizedLoginCustEntity()
            };
            var dateTime = DateTime.Now;

            // Act
            unAuthorizedLogindataAccess.DetectUnauthorizedLogin(custJson, dateTime).Returns(expectedResult);

            var result = unAuthorizedLoginService.DetectUnauthorizedLogin(custJson, dateTime);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

    }
}
