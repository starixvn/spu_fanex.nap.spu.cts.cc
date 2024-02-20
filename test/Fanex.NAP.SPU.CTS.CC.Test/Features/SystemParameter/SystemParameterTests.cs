namespace Fanex.NAP.SPU.CTS.Tests.Features.SystemParameter
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class SystemParameterTests
    {
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private readonly ISystemParameterServices systemParameterServices;

        public SystemParameterTests()
        {
            systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();

            systemParameterServices = new SystemParameterServices(systemParameterDataAccess);
        }

        [Theory]
        [InlineData(CTS_DC_SystemParameters.MatchMonitorLastScannedTrans)]
        public void GetLastScannedId_WhenSuccess_ReturnLastScannedId(CTS_DC_SystemParameters paraID)
        {
            var output = new SystemParameterEntity
            {
                ParameterDataType = "BIGINT UNSIGNED",
                ParameterValue = "212121"
            };

            systemParameterDataAccess.GetPreviousTimingExecution(paraID).Returns(output);

            var result = systemParameterServices.GetLastScannedId(paraID);

            result.Should().Be(Int64.Parse(output.ParameterValue));
            result.Should().NotBe(null);
        }

        [Theory]
        [InlineData(CTS_DC_SystemParameters.DailyNormalClassificationLastCategoryID)]
        [InlineData(CTS_DC_SystemParameters.RobotAccountLastScannedTime)]
        public void GetLastScannedTime_WhenSuccess_ReturnLastScannedTime(CTS_DC_SystemParameters paraID)
        {
            var output = new SystemParameterEntity
            {
                ParameterDataType = "DATETIME",
                ParameterValue = "2021-07-05 22:50:55"
            };

            systemParameterDataAccess.GetPreviousTimingExecution(paraID).Returns(output);

            var result = systemParameterServices.GetLastScannedDateTime(paraID);

            result.Should().Be(DateTime.Parse(output?.ParameterValue));
        }

        [Theory]
        [InlineData(CTS_DC_SystemParameters.DailyNormalClassificationLastCategoryID, "2021-07-05 22:50:53")]
        public void UpdateTimingExecution_WhenSuccess(CTS_DC_SystemParameters paraId, string lastScannedTime)
        {
            systemParameterDataAccess.UpdateTimingExecution(lastScannedTime, paraId);

            bool isSuccess = systemParameterServices.UpdateTimingExecution(lastScannedTime, paraId);

            isSuccess.Should().BeTrue();
        }
    }
}