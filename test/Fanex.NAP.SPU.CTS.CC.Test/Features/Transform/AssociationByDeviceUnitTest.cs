namespace Fanex.NAP.SPU.CTS.Tests.Features.Transform
{
    using Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice;
    using Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice.Entities;
    using NSubstitute;
    using Sentry;
    using System.Collections.Generic;
    using Xunit;

    public class AssociationByDeviceUnitTest
    {
        private readonly IAssociationByDeviceService associationByDeviceService;
        private readonly IAssociationByDeviceDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public AssociationByDeviceUnitTest()
        {
            dataAccess = Substitute.For<IAssociationByDeviceDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            associationByDeviceService = new AssociationByDeviceService(dataAccess, sentryClient);
        }

        [Fact]
        public void GetRawAssociation_Returns_Empty()
        {
            IEnumerable<RawAssociationModel> data = new List<RawAssociationModel>();
            dataAccess.GetRawAssociation(10, 1).Returns(data);
            associationByDeviceService.TransformAssociationByDevice(10, 1, false);
        }
    }
}