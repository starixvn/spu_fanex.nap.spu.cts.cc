namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class NormalAccountInsertTest
    {
        private readonly INormalAccountDataAccess normalAccountDataAccess;
        private readonly INormalAccountServices normalAccountServices;

        public NormalAccountInsertTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountDataAccess>();

            normalAccountServices = new NormalAccountServices(normalAccountDataAccess, null);
        }

        [Fact]
        public void GetNewClassification_WhenSuccess_ReturnNewClassificationList()
        {
            var normalAccounts = new GeneralCustomerClassificationEnity
            {
                GeneralCustClassifications = new List<CustomerClassificationEntity>
                {
                    new CustomerClassificationEntity
                    {
                        CustId = 1,
                        SportGroupId = 1,
                        CategoryId = 1,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 1,
                        BetCount = 1,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 1,
                        ProbationPeriodWinloss = 1,
                        TaggingID = 1,
                        TaggingType = 1,
                        TurnoverRM = 1,
                        WinlossRM = 1
                    },
                    new CustomerClassificationEntity
                    {
                        CustId = 2,
                        SportGroupId = 2,
                        CategoryId = 2,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 2,
                        BetCount = 2,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 2,
                        ProbationPeriodWinloss = 2,
                        TaggingID = 2,
                        TaggingType = 2,
                        TurnoverRM = 2,
                        WinlossRM = 2
                    },
                },
                IsContinute = true
            };

            normalAccountDataAccess.GetNewClassification().Returns(normalAccounts.GeneralCustClassifications);

            var result = normalAccountServices.GetNewClassification("NormalAccountInsert", false);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(normalAccounts);
        }

        [Fact]
        public void GetCustAssociationPA_WhenSuccess_ReturnCustInfoList()
        {
            var listClassification = new List<CustomerClassificationEntity>
                {
                    new CustomerClassificationEntity
                    {
                        CustId = 1,
                        SportGroupId = 1,
                        CategoryId = 1,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 1,
                        BetCount = 1,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 1,
                        ProbationPeriodWinloss = 1,
                        TaggingID = 1,
                        TaggingType = 1,
                        TurnoverRM = 1,
                        WinlossRM = 1
                    },
                    new CustomerClassificationEntity
                    {
                        CustId = 2,
                        SportGroupId = 2,
                        CategoryId = 2,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 2,
                        BetCount = 2,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 2,
                        ProbationPeriodWinloss = 2,
                        TaggingID = 2,
                        TaggingType = 2,
                        TurnoverRM = 2,
                        WinlossRM = 2
                    },
                };

            var custAssociationWithPA = new List<TaggedCustomerEntity>
            {
                new TaggedCustomerEntity
                {
                    CustID = 1,
                    TaggingID = 1,
                    TaggingType = 1,
                },
                 new TaggedCustomerEntity
                {
                    CustID = 2,
                    TaggingID = 2,
                    TaggingType = 2,
                }
            };

            var custIds = listClassification.Select(x => x.CustId).Distinct();
            normalAccountDataAccess.GetCustsAssociationProblemAccount(string.Join(",", custIds)).Returns(custAssociationWithPA);

            var result = normalAccountServices.GetCustomerAssociationProblemAccount(listClassification);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(custAssociationWithPA);
        }

        [Fact]
        public void UpdateAppliedStatusClassification_WhenSuccess_ReturnTrue()
        {
            var listClassification = new List<CustomerClassificationEntity>
                {
                    new CustomerClassificationEntity
                    {
                        CustId = 1,
                        SportGroupId = 1,
                        CategoryId = 1,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 1,
                        BetCount = 1,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 1,
                        ProbationPeriodWinloss = 1,
                        TaggingID = 1,
                        TaggingType = 1,
                        TurnoverRM = 1,
                        WinlossRM = 1
                    },
                    new CustomerClassificationEntity
                    {
                        CustId = 2,
                        SportGroupId = 2,
                        CategoryId = 2,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 2,
                        BetCount = 2,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 2,
                        ProbationPeriodWinloss = 2,
                        TaggingID = 2,
                        TaggingType = 2,
                        TurnoverRM = 2,
                        WinlossRM = 2
                    },
                };

            var custIds = listClassification.Select(x => x.CustId).Distinct();
            normalAccountDataAccess.UpdateAppliedStatusClassification(string.Join(",", custIds));

            var result = normalAccountServices.UpdateAppliedStatusClassification(listClassification, true);

            result.Should().BeTrue();
        }

        [Fact]
        public void InsertMainCustomerClassification_WhenSuccess_ReturnCustIds()
        {
            var listClassification = new List<CustomerClassificationEntity>
                {
                    new CustomerClassificationEntity
                    {
                        CustId = 1,
                        SportGroupId = 1,
                        CategoryId = 1,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 1,
                        BetCount = 1,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 1,
                        ProbationPeriodWinloss = 1,
                        TaggingID = 1,
                        TaggingType = 1,
                        TurnoverRM = 1,
                        WinlossRM = 1
                    },
                    new CustomerClassificationEntity
                    {
                        CustId = 2,
                        SportGroupId = 2,
                        CategoryId = 2,
                        CreatedTime = DateTime.Parse("2021-07-05 22:50:55"),
                        ActiveDays = 2,
                        BetCount = 2,
                        IsGeneralCategory = true,
                        LastXDaysMargin = 2,
                        ProbationPeriodWinloss = 2,
                        TaggingID = 2,
                        TaggingType = 2,
                        TurnoverRM = 2,
                        WinlossRM = 2
                    },
                };

            var custAssociationWithPA = new List<TaggedCustomerEntity>
            {
                new TaggedCustomerEntity
                {
                    CustID = 1,
                    TaggingID = 1,
                    TaggingType = 1,
                },
                 new TaggedCustomerEntity
                {
                    CustID = 2,
                    TaggingID = 2,
                    TaggingType = 2,
                }
            };

            var ouput = new InsertCategoryResultEntity
            {
                CustIds = new List<long> { 1, 2, 3 },
                ErrorMessage = string.Empty
            };

            var custClassificationAfterCheckPA = normalAccountServices.MergeWithTaggedCust(listClassification, custAssociationWithPA);
            var jsonClassification = JsonConvert.SerializeObject(custClassificationAfterCheckPA);
            normalAccountDataAccess.InsertMainCustomerClassification(jsonClassification).Returns(ouput);

            var custIds = listClassification.Select(x => x.CustId).Distinct();
            normalAccountDataAccess.UpdateAppliedStatusClassification(string.Join(",", custIds));

            var result = normalAccountServices.InsertMainCustomerClassification(custClassificationAfterCheckPA, "NormalAccountInsert", false);

            result.Should().BeEquivalentTo(ouput);
        }
    }
}