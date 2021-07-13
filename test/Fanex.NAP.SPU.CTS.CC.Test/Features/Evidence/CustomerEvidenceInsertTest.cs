namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class CustomerEvidenceInsertTest
    {
        private readonly ICustomerEvidenceInsertDataAccess customerEvidenceInsertDataAccess;
        private readonly ICustomerEvidenceInsertServices customerEvidenceInsertServices;

        public CustomerEvidenceInsertTest()
        {
            customerEvidenceInsertDataAccess = Substitute.For<ICustomerEvidenceInsertDataAccess>();
            customerEvidenceInsertServices = new CustomerEvidenceInsertServices(customerEvidenceInsertDataAccess);
        }

        [Fact]
        public void GetCustomerEvidenceFromFile_WhenSuccess_ReturnCustomerEvidence()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            customerEvidenceInsertDataAccess.GetCustomerEvidenceFromFile().Returns(cusEvidences);

            var result = customerEvidenceInsertServices.GetCustomerEvidenceFromFile();

            result.Should().BeEquivalentTo(cusEvidences);
        }

        [Fact]
        public void RemoveOldEvidence_WhenSuccess_ReturnTrue()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var rs = new ResultRemoveCustomerEvidence
            {
                CustEviRemovedList = new List<CustomerEvidenceRemovedEntity>
                 {
                     new CustomerEvidenceRemovedEntity
                     {
                         CTSCustId = 1,
                         CustId = 1,
                         ActionType = 1,
                         EvidenceId = 1,
                         SubscriberId = 1
                     },
                     new CustomerEvidenceRemovedEntity
                     {
                         CTSCustId = 2,
                         CustId = 2,
                         ActionType = 2,
                         EvidenceId = 2,
                         SubscriberId = 2
                     },
                 },
                DBResult = new DBResult
                {
                    ErrorCode = 0,
                    ErrorMsg = string.Empty,
                }
            };

            var ctsCustIdList = cusEvidences.Select(x => new { x.CTSCustID }).Distinct();

            var jsonData = JsonConvert.SerializeObject(ctsCustIdList);

            customerEvidenceInsertDataAccess.RemoveCustomerEvidenceByCTSCustIds(jsonData, false, false).Returns(rs);

            var result = customerEvidenceInsertServices.RemoveOldEvidence(cusEvidences, out IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList);

            result.Should().BeTrue();
        }

        [Fact]
        public void InsertNewCustomerEvidence_WhenSuccess_ReturnTrue()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var rs = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
            };

            var objects = cusEvidences.Select(x => new
            {
                x.CTSCustID,
                x.EvidenceID,
                x.Remark,
                x.CreatedBy
            });

            var jsonData = JsonConvert.SerializeObject(objects);

            customerEvidenceInsertDataAccess.InsertCustomerEvidenceMuitiple(jsonData, true).Returns(rs);

            var result = customerEvidenceInsertServices.InsertNewCustomerEvidence(cusEvidences, true, true);

            result.Should().BeTrue();
        }

        [Fact]
        public void OverwriteCustomerEvidenceToCategory_WhenSuccess_ReturnTrue()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var custEviRemovedList = new List<CustomerEvidenceRemovedEntity>
                 {
                     new CustomerEvidenceRemovedEntity
                     {
                         CTSCustId = 1,
                         CustId = 1,
                         ActionType = (int)EvidenceActionType.Remove,
                         EvidenceId = 1,
                         SubscriberId = 1
                     },
                     new CustomerEvidenceRemovedEntity
                     {
                         CTSCustId = 2,
                         CustId = 2,
                         ActionType = (int)EvidenceActionType.Remove,
                         EvidenceId = 2,
                         SubscriberId = 2
                     },
                 };

            var rs = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
            };

            var objects = cusEvidences.Select(x => new
            {
                CustId = x.CustID,
                CTSCustId = x.CTSCustID,
                SubscriberId = x.SubscriberID,
                EvidenceId = x.EvidenceID,
                ActionType = (int)EvidenceActionType.Remove
            }).ToList();

            var removeds = custEviRemovedList.Select(x => new
            {
                x.CustId,
                x.CTSCustId,
                x.SubscriberId,
                x.EvidenceId,
                x.ActionType
            });

            objects.AddRange(removeds);

            var jsonData = JsonConvert.SerializeObject(objects);

            customerEvidenceInsertDataAccess.SyncCustomerEvidenceToCategory(jsonData, false).Returns(rs);

            var result = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(cusEvidences, EvidenceActionType.Remove, custEviRemovedList, true);

            result.Should().BeTrue();
        }

        [Fact]
        public void SyncCustomerEvidenceToCategory_WhenSuccess_ReturnTrue()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var rs = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
            };

            var objects = cusEvidences.Select(x => new
            {
                CustId = x.CustID,
                CTSCustId = x.CTSCustID,
                SubscriberId = x.SubscriberID,
                EvidenceId = x.EvidenceID,
                ActionType = (int)EvidenceActionType.Insert
            }).ToList();

            var jsonData = JsonConvert.SerializeObject(objects);

            customerEvidenceInsertDataAccess.SyncCustomerEvidenceToCategory(jsonData, false).Returns(rs);

            var result = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(cusEvidences, EvidenceActionType.Insert, true);

            result.Should().BeTrue();
        }

        [Fact]
        public void UpdateBetLimitOfFlaggedCustomer_WhenSuccess_ReturnNotException()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var rs = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
            };

            var associatedCustIds = new List<int> { 1, 2, 3 };

            var ctsCustIds = string.Join(",", cusEvidences.Select(x => x.CTSCustID).Distinct());
            customerEvidenceInsertDataAccess.GetAssociatedAccountByCTSCustIds(ctsCustIds).Returns(associatedCustIds);

            var associatedCustIdsToString = string.Join(",", associatedCustIds.Distinct());
            customerEvidenceInsertDataAccess.UpdateBetLimitOfFlaggedCustomer(associatedCustIdsToString).Returns(rs);

            Action action = () => customerEvidenceInsertServices.UpdateBetLimitOfFlaggedCustomer(cusEvidences);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateProcessedCustEvidence_WhenSuccess_ReturnNotException()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 2,
                    CTSCustID = 2,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var rs = new DBResult
            {
                ErrorCode = 0,
                ErrorMsg = string.Empty,
            };

            var idList = string.Join(",", cusEvidences.Select(x => x.ID));
            customerEvidenceInsertDataAccess.UpdateProcessedCustEvidence(idList, true).Returns(rs);

            Action action = () => customerEvidenceInsertServices.UpdateProcessedCustEvidence(cusEvidences, true);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void GetFailCustomerEvidence_WhenSuccess_ReturnCustEvidenceList()
        {
            var result = customerEvidenceInsertServices.GetLogError();

            result.Should().NotBeNull();
        }

        [Fact]
        public void ChunkSize_WhenSuccess_ReturnCustEvidenceList()
        {
            var cusEvidences = new List<CustomerEvidenceFromFileEntity>
            {
                new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 1,
                    EvidenceID = 1,
                    ID = 1,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 1,
                    CTSCustID = 1,
                    CreatedBy = 2,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 2,
                    EvidenceID = 2,
                    ID = 2,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
                 new CustomerEvidenceFromFileEntity
                {
                    CustID = 3,
                    CTSCustID = 3,
                    CreatedBy = 3,
                    CreatedDate = DateTime.Parse("2021-07-07 22:50:55"),
                    SubscriberID = 3,
                    EvidenceID = 3,
                    ID = 3,
                    FileKey = string.Empty,
                    IsOverwrite = false,
                    Remark = string.Empty
                },
            };

            var result = customerEvidenceInsertServices.ChunkSize(cusEvidences, 1);

            result.Should().HaveCount(2);
        }
    }
}