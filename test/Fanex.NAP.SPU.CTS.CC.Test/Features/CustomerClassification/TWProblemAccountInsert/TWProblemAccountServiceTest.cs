namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class TWProblemAccountServiceTest
    {
        private readonly ITWProblemAccountDataAccess twProblemAccountDataAccess;
        private readonly ITWProblemAccountService twProblemAccountService;

        public TWProblemAccountServiceTest()
        {
            twProblemAccountDataAccess = Substitute.For<ITWProblemAccountDataAccess>();
            twProblemAccountService = new TWProblemAccountService(twProblemAccountDataAccess);
        }

        [Fact]
        public void ScanSystemFormulaBetMonitor_WhenSuccess()
        {
            uint lastCustID;
            var expectedResult = new List<RescanTWProblemAccountModel>();            

            twProblemAccountDataAccess.ScanSystemFormulaBetMonitor(out lastCustID).Returns(expectedResult);

            var result = twProblemAccountService.ScanSystemFormulaBetMonitor(out lastCustID);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCustomerPerformance_WhenSuccess()
        {
            var custIdList = "3673223,3679916,3679918";
            var expectedResult = new List<RescanTWProblemAccountModel>();

            twProblemAccountDataAccess.GetCustomerPerformance(custIdList).Returns(expectedResult);

            var result = twProblemAccountService.GetCustomerPerformance(custIdList);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void RemoveSystemFormulaBetCustomer_WhenSuccess_ReturnNotException()
        {
            var custIdList = "3673223,3679916,3679918";         

            Action action = () => twProblemAccountService.RemoveSystemFormulaBetCustomer(custIdList);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void ScanSystemFormulaBetMonitorComplete_WhenSuccess_ReturnNotException()
        {
            uint lastCustID = 0;

            Action action = () => twProblemAccountService.ScanSystemFormulaBetMonitorComplete(lastCustID);            

            action.Should().NotThrow<Exception>();
        }
    }
}
