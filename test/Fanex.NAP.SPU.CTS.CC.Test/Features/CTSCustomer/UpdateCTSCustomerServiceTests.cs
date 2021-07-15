namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class UpdateCTSCustomerServiceTests
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ISentryClient sentryClient;

        public UpdateCTSCustomerServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService);
        }

        //[Fact]
        //public void TransformCustomerFromMainDB_WhenDataHasPropertyNull1_RunSuccessfully()
        //{
        //    var expectedResult = new List<InsertCustomerModel>
        //    {
        //        new InsertCustomerModel
        //        {
        //            CTSCustID = 1231,
        //            CustID = 123,
        //            CustSubID = 0,
        //            IsExisting = false,
        //            RegisterName = "ahaha",
        //            SubscriberID = 2,
        //            UserName = "super123"
        //        }
        //    };

        //    var inputData = new List<CustomerMainDBModel> { CreateSampleCustomerMainDBModel() };
        //    dataAccess.TransformCustomerFromMainDB(string.Empty).Returns(expectedResult);
        //    var result = ctsCustomerService.GetLastUpdatedTime();

        //    result.Should().BeEquivalentTo(expectedResult);
        //}

        
    }
}