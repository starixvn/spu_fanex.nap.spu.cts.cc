namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;

    public class NormalAccountBySportServiceTest
    {
        private readonly INormalAccountBySportDataAccess normalAccountDataAccess;
        private readonly INormalAccountBySportService normalAccountServices;

        public NormalAccountBySportServiceTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountBySportDataAccess>();

            normalAccountServices = new NormalAccountBySportService(normalAccountDataAccess, null);
        }

        [Fact]
        public void GetNewClassification_WhenSuccess_ReturnNewClassificationList()
        {
            var normalAccounts = new List<CustomerClassificationBySportEntity>()
            {
                new CustomerClassificationBySportEntity
                {
                    CustId = 1,
                    CategoryId = 201,
                    SportId = 1,
                    ActiveDays = 1,
                    BetCount = 1,
                    TurnoverRM = 1,
                    WinlossRM = 1
                },
                new CustomerClassificationBySportEntity
                {
                    CustId = 2,
                    CategoryId = 202,
                    SportId = 2,
                    ActiveDays = 1,
                    BetCount = 1,
                    TurnoverRM = 1,
                    WinlossRM = 1
                }
            };
            
            normalAccountDataAccess.GetNewClassificationBySport().Returns(normalAccounts);

            var result = normalAccountServices.GetNewClassificationBySport();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(normalAccounts);
        }

        [Fact]
        public void InsertCustomerClassificationBySport_WhenSuccess_ReturnCustInfos()
        {
            var listClassification = new List<CustomerClassificationBySportEntity>
                {
                    new CustomerClassificationBySportEntity
                    {
                        CustId = 1,
                        CategoryId = 201,
                        SportId = 1,
                        ActiveDays = 1,
                        BetCount = 1,
                        TurnoverRM = 1,
                        WinlossRM = 1
                    },
                    new CustomerClassificationBySportEntity
                    {
                        CustId = 2,
                        CategoryId = 202,
                        SportId = 2,
                        ActiveDays = 2,
                        BetCount = 2,
                        TurnoverRM = 2,
                        WinlossRM = 2
                    }
                };

            var dataAccessResult = new InsertNormalAccountResultEntity
            {
                CustInfos = new List<InsertNormalAccountCustInfoEntity>(),
                ErrorMessage = string.Empty
            };

            var custClassJsonData = JsonConvert.SerializeObject(listClassification);
            normalAccountDataAccess.InsertCustomerClassificationBySport(custClassJsonData).Returns(dataAccessResult);
            
            var result = normalAccountServices.InsertCustomerClassificationBySport(listClassification, "NormalAccountInsert", false);

            result.Should().BeEquivalentTo(dataAccessResult);
        }

        [Fact]
        public void UpdateAppliedStatusClassificationBySport_WhenSuccess_ReturnTrue()
        {
            var listClassification = new List<CustomerClassificationBySportEntity>
                {
                    new CustomerClassificationBySportEntity
                    {
                        CustId = 1,
                        CategoryId = 201,
                        SportId = 1,
                        ActiveDays = 1,
                        BetCount = 1,
                        TurnoverRM = 1,
                        WinlossRM = 1
                    },
                    new CustomerClassificationBySportEntity
                    {
                        CustId = 2,
                        CategoryId = 202,
                        SportId = 2,
                        ActiveDays = 2,
                        BetCount = 2,
                        TurnoverRM = 2,
                        WinlossRM = 2
                    }
                };
            
            normalAccountDataAccess.UpdateAppliedStatusClassificationBySport(listClassification.ToXmlString());

            var result = normalAccountServices.UpdateAppliedStatusClassificationBySport(listClassification, true);

            result.Should().BeTrue();
        }
    }
}
