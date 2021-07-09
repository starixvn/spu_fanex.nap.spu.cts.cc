namespace Fanex.NAP.SPU.CTS.Tests.Features.Transform
{
    using Fanex.NAP.SPU.CTS.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.JobService.Services.Transform;
    using NSubstitute;
    using Sentry;

    public class CTSCustomerUnitTest
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ICTSCustomerJobService ctsCustomerJobService;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ISentryClient sentryClient;

        public CTSCustomerUnitTest()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService);
            //ctsCustomerJobService = new CTSCustomerJobService(sentryClient,ctsCustomerService)
        }

        public void Test()
        {
        }
    }
}