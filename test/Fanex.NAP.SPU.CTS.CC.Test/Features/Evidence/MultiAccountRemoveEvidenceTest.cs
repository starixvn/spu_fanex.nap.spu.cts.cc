namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class MultiAccountRemoveEvidenceTest
    {
        private readonly IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess;
        private readonly IRemoveAccountEvidenceServices removeAccountEvidenceServices;

        public MultiAccountRemoveEvidenceTest()
        {
            removeAccountEvidenceDataAccess = Substitute.For<IRemoveAccountEvidenceDataAccess>();
            removeAccountEvidenceServices = new RemoveAccountEvidenceServices(removeAccountEvidenceDataAccess, null);
        }

        [Fact]
        public void GetCustomerEvidenceQueue_WhenSuccess_ReturnCustomerEvidence()
        {
            var cusEvidences = new List<CustomerEvidenceQueueEntity>
            {
                new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 1,
                    CustId = 1,
                    CreatedBy = 1,
                    ActionId = 1,
                    EvidenceId = 1,
                    Id = 1,
                    SubscriberId = 1,
                    Remark = string.Empty
                },
                  new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 2,
                    CustId = 2,
                    CreatedBy = 2,
                    ActionId = 2,
                    EvidenceId = 2,
                    Id = 2,
                    SubscriberId = 2,
                    Remark = string.Empty
                }
            };

            removeAccountEvidenceDataAccess.GetCustomerEvidenceQueue().Returns(cusEvidences);

            var result = removeAccountEvidenceServices.GetCustomerEvidenceQueue();

            result.Should().BeEquivalentTo(cusEvidences);
        }

        [Fact]
        public void ModifyCustomerEvidence_WhenSuccess_ReturnDBResult()
        {
            var cusEvidences = new List<CustomerEvidenceQueueEntity>
            {
                new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 1,
                    CustId = 1,
                    CreatedBy = 1,
                    ActionId = 1,
                    EvidenceId = 1,
                    Id = 1,
                    SubscriberId = 1,
                    Remark = string.Empty
                },
                  new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 2,
                    CustId = 2,
                    CreatedBy = 2,
                    ActionId = 2,
                    EvidenceId = 2,
                    Id = 2,
                    SubscriberId = 2,
                    Remark = string.Empty
                }
            };

            var result = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
                OutParam = new Hashtable {
                    { "custIds", "1,2,3" }
                }
            };

            var customerEvidenceJson = JsonConvert.SerializeObject(cusEvidences);

            removeAccountEvidenceDataAccess.RemoveCustEvidenceFromQueue(customerEvidenceJson, Constants.RemoveEvidences_ClearActionId).Returns(result);

            Action action = () => removeAccountEvidenceServices.RemoveCustEvidenceFromQueue(cusEvidences, Constants.RemoveEvidences_ClearActionId, "MultiAccountRemoveEvidences");

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateCustomerEvidence_WhenSuccess_ReturnDBResult()
        {
            var cusEvidences = new List<CustomerEvidenceQueueEntity>
            {
                new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 1,
                    CustId = 1,
                    CreatedBy = 1,
                    ActionId = 1,
                    EvidenceId = 1,
                    Id = 1,
                    SubscriberId = 1,
                    Remark = string.Empty
                },
                  new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 2,
                    CustId = 2,
                    CreatedBy = 2,
                    ActionId = 2,
                    EvidenceId = 2,
                    Id = 2,
                    SubscriberId = 2,
                    Remark = string.Empty
                }
            };

            var result = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty
            };

            var processedIdList = cusEvidences.Select(x => x.Id);

            removeAccountEvidenceDataAccess.UpdateCustomerEvidenceComplete(string.Join(",", processedIdList)).Returns(result);

            Action action = () => removeAccountEvidenceServices.InitialData().UpdateCustomerEvidenceComplete(cusEvidences, "MultiAccountRemoveEvidences");

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void GetCustRescanClassification_WhenSuccess_ReturnCustIds()
        {
            var cusEvidences = new List<CustomerEvidenceQueueEntity>
            {
                new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 1,
                    CustId = 1,
                    CreatedBy = 1,
                    ActionId = 1,
                    EvidenceId = 1,
                    Id = 1,
                    SubscriberId = 1,
                    Remark = string.Empty
                },
                  new CustomerEvidenceQueueEntity
                {
                    CTSCustId = 2,
                    CustId = 2,
                    CreatedBy = 2,
                    ActionId = 2,
                    EvidenceId = 2,
                    Id = 2,
                    SubscriberId = 2,
                    Remark = string.Empty
                }
            };

            var dbResult = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
                OutParam = new Hashtable {
                    { "custIds", "1,2,3" }
                }
            };

            var custIdsToRescan = dbResult.OutParam["custIds"]?.ToString().Split(',').Select(long.Parse);

            var customerEvidenceJson = JsonConvert.SerializeObject(cusEvidences);
            var processedIdList = cusEvidences.Select(x => x.Id);

            removeAccountEvidenceDataAccess.RemoveCustEvidenceFromQueue(customerEvidenceJson, Constants.RemoveEvidences_ClearActionId).Returns(dbResult);
            removeAccountEvidenceServices.RemoveCustEvidenceFromQueue(cusEvidences, Constants.RemoveEvidences_ClearActionId, "MultiAccountRemoveEvidences");

            removeAccountEvidenceDataAccess.UpdateCustomerEvidenceComplete(string.Join(",", processedIdList)).Returns(dbResult);
            removeAccountEvidenceServices.UpdateCustomerEvidenceComplete(cusEvidences, "MultiAccountRemoveEvidences");

            var result = removeAccountEvidenceServices.GetCustomerToUpdateCC();
            result.Should().BeEquivalentTo(custIdsToRescan);
        }
    }
}