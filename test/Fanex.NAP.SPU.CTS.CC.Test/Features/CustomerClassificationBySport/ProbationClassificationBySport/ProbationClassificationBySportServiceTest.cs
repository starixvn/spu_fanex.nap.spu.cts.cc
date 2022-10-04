namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ProbationClassificationBySportServiceTest
    {
        private readonly IProbationClassificationBySportDataAccess probationClassificationDataAccess;
        private readonly IProbationClassificationBySportService probationClassificationServices;

        public ProbationClassificationBySportServiceTest()
        {
            probationClassificationDataAccess = Substitute.For<IProbationClassificationBySportDataAccess>();
            probationClassificationServices = new ProbationClassificationBySportService(probationClassificationDataAccess);
        }

        [Fact]
        public void ScanProbationClassificationBySport_WhenSuccess_ReturnNotException()
        {
            var probationList = new List<ProbationClassificationBySportEntity>
            {
               new ProbationClassificationBySportEntity
               {
                    CustID = 1,
                    SportID = 1,
                    IsProbationLastDay = 1
               },
               new ProbationClassificationBySportEntity
               {
                    CustID= 2,
                    SportID= 2,
                    IsProbationLastDay = 1
               }
            };

            var scanProbationList = new List<ScanProbationClassificationBySportEntity>
            {
               new ScanProbationClassificationBySportEntity
               {
                    CustId = 1,
                    SportId = 1,
                    IsProbationLastDay = 1
               },
               new ScanProbationClassificationBySportEntity
               {
                    CustId= 2,
                    SportId= 2,
                    IsProbationLastDay = 1
               }
            };

            probationClassificationDataAccess.GetProbationCustClassificationBySport(1).Returns(probationList);
            probationClassificationDataAccess.ScanProbationClassificationBySport(scanProbationList);

            probationClassificationServices.GetProbationCustClassificationBySport(1);
            Action action = () => probationClassificationServices.ScanProbationClassificationBySport();

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateScanDateProbationClassificationBySport_WhenSuccess_ReturnNotException()
        {
            var probationList = new List<ProbationClassificationBySportEntity>
            {
               new ProbationClassificationBySportEntity
               {
                    CustID = 1,
                    SportID = 1,
                    IsProbationLastDay = 1
               },
               new ProbationClassificationBySportEntity
               {
                    CustID= 2,
                    SportID= 2,
                    IsProbationLastDay = 1
               }
            };

            var dataJson = JsonConvert.SerializeObject(probationList);

            probationClassificationDataAccess.GetProbationCustClassificationBySport(1).Returns(probationList);
            probationClassificationDataAccess.UpdateScanDateProbationClassificationBySport(dataJson);

            probationClassificationServices.GetProbationCustClassificationBySport(1);
            Action action = () => probationClassificationServices.UpdateScanDateProbationClassificationBySport();

            action.Should().NotThrow<Exception>();
        }
    }
}
