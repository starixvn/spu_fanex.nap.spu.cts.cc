namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class RealtimeClassificationBySportServicesTest
    {
        private readonly IRealtimeClassificationBySportDataAccess realtimeClassificationDataAccess;
        private readonly IRealtimeClassificationBySportServices realtimeClassificationServices;

        public RealtimeClassificationBySportServicesTest()
        {
            realtimeClassificationDataAccess = Substitute.For<IRealtimeClassificationBySportDataAccess>();
            realtimeClassificationServices = new RealtimeClassificationBySportServices(realtimeClassificationDataAccess, null);
        }

        [Fact]
        public void ScanningRealtimeClassification_WhenSuccess_ReturnNotException()
        {
            var classificationList = new List<CustClassificationRealtimeBySportEntity>()
            {
                new CustClassificationRealtimeBySportEntity
                {
                     CustId = 1,
                     CategoryId = 201,
                     SportId = 1
                },
                new CustClassificationRealtimeBySportEntity
                {
                     CustId = 2,
                     CategoryId = 202,
                     SportId = 2
                }
            };

            realtimeClassificationDataAccess.ClassifyCustomerBySportRealtime(classificationList);

            Action action = () => realtimeClassificationServices.ScanningRealtimeClassification(1, 1);

            action.Should().NotThrow<Exception>();
        }

    }
}
