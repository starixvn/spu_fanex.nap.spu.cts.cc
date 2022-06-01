namespace Fanex.NAP.SPU.CTS.Tests.Features.PushDataToExternal
{
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class CustClassificationToMainDBTests
    {
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IFeedSoon88DataAccess feedSoon88DataAccess;
        private readonly IFeedSoon88Services feedSoon88Services;

        public CustClassificationToMainDBTests()
        {
            feedSoon88DataAccess = Substitute.For<IFeedSoon88DataAccess>();
            feedSoon88Services = new FeedSoon88Services(feedSoon88DataAccess, null);

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, feedSoon88Services, null);
        }

        //[Fact]
        //public void GetLastestCustomerClassification_WhenSuccess_ReturnLastestCC()
        //{
        //    var lastestCustomerClassification = new List<LatestClassificationEntity> {
        //        new LatestClassificationEntity
        //        {
        //            CustId = 1,
        //            Category = 1,
        //            CustomerClass = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new LatestClassificationEntity {
        //            CustId = 2,
        //            Category = 2,
        //            CustomerClass = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //         }
        //    };

        //    var custIds = new List<long> { 1, 2, 3, 4 };

        //    custClassToMainDBDataAccess.GetLatestClassificationDangerLevel("1,2,3,4").Returns(lastestCustomerClassification);

        //    var result = custClassToMainDBService.GetLatestClassification(custIds, "Push CC To MainDB - Get Lastest CC", false);

        //    result.Should().BeEquivalentTo(lastestCustomerClassification);
        //}

        //[Fact]
        //public void UpdateAppliedCustClassification_WhenSuccess_ReturnTrue()
        //{
        //    var lastestCustomerClassification = new List<LatestClassificationEntity> {
        //        new LatestClassificationEntity
        //        {
        //            CustId = 1,
        //            Category = 1,
        //            CustomerClass = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new LatestClassificationEntity {
        //            CustId = 2,
        //            Category = 2,
        //            CustomerClass = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //         }
        //    };

        //    var dataResult = new DBResult
        //    {
        //        ErrorMsg = string.Empty,
        //        ErrorCode = 3
        //    };

        //    var jsonData = JsonConvert.SerializeObject(lastestCustomerClassification);
        //    custClassToMainDBDataAccess.UpdateAppliedCustomerClassification(jsonData).Returns(dataResult);

        //    var result = custClassToMainDBService.UpdateAppliedCustClassification(lastestCustomerClassification, "Push CC To MainDB - IsApplied Lastest CC");

        //    result.Should().BeTrue();
        //}

        //[Fact]
        //public void RetryPushCustClassification_WhenSuccess_ReturnTrue()
        //{
        //    var lastestCustomerClassification = new List<LatestClassificationEntity> {
        //        new LatestClassificationEntity
        //        {
        //            CustId = 1,
        //            Category = 1,
        //            CustomerClass = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new LatestClassificationEntity {
        //            CustId = 2,
        //            Category = 2,
        //            CustomerClass = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //         }
        //    };

        //    var dataResult = new DBResult
        //    {
        //        ErrorMsg = string.Empty,
        //        ErrorCode = 0
        //    };

        //    custClassToMainDBDataAccess.UpdateCustomerGeneralClassificationtoMainDB(lastestCustomerClassification).Returns(dataResult);

        //    var result = custClassToMainDBService.RetryPushCCToMainDB(lastestCustomerClassification, 1, "Push CC To MainDB - IsApplied Lastest CC");

        //    result.Should().BeTrue();
        //}

        //[Fact]
        //public void RetryUpdateDangerLevelToMainDB_WhenSuccess_ReturnTrue()
        //{
        //    var dangerLevelEnity = new List<DangerLevelEntity> {
        //        new DangerLevelEntity
        //        {
        //            CustId = 1,
        //            DangerLevel = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new DangerLevelEntity {
        //            CustId = 2,
        //            DangerLevel = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:51:55")
        //         }
        //    };

        //    var dataResult = new DBResult
        //    {
        //        ErrorMsg = string.Empty,
        //        ErrorCode = 0
        //    };

        //    custClassToMainDBDataAccess.UpdateDangerLevelToMainDB(dangerLevelEnity, DangerLevel.Original).Returns(dataResult);

        //    var result = custClassToMainDBService.RetryUpdateDangerLevelToMainDB(dangerLevelEnity, DangerLevel.Original, 1, "Push CC To MainDB - IsApplied Lastest CC");

        //    result.Should().BeTrue();
        //}

        //[Fact]
        //public void PushCustClassificationToMainDB_WhenSuccess_ReturnTrue()
        //{
        //    var lastestCustomerClassification = new List<LatestClassificationEntity> {
        //        new LatestClassificationEntity
        //        {
        //            CustId = 1,
        //            Category = 1,
        //            CustomerClass = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new LatestClassificationEntity {
        //            CustId = 2,
        //            Category = 2,
        //            CustomerClass = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //         }
        //    };

        //    var dataResult = new DBResult
        //    {
        //        ErrorMsg = string.Empty,
        //        ErrorCode = 0
        //    };

        //    custClassToMainDBDataAccess.UpdateCustomerGeneralClassificationtoMainDB(lastestCustomerClassification).Returns(dataResult);

        //    var jsonData = JsonConvert.SerializeObject(lastestCustomerClassification);
        //    custClassToMainDBDataAccess.UpdateAppliedCustomerClassification(jsonData).Returns(dataResult);

        //    var result = custClassToMainDBService.PushCustClassificationToMainDB(lastestCustomerClassification, "Push CC To MainDB", false);

        //    result.Should().BeTrue();
        //}

        //[Fact]
        //public void UpdateDangerLevelToMainDB_WhenSuccess_ReturnTrue()
        //{
        //    var lastestCustomerClassification = new List<LatestClassificationEntity> {
        //        new LatestClassificationEntity
        //        {
        //            CustId = 1,
        //            Category = 1,
        //            CustomerClass = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new LatestClassificationEntity {
        //            CustId = 2,
        //            Category = 2,
        //            CustomerClass = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //         }
        //    };

        //    var dangerLevelEnity = new List<DangerLevelEntity> {
        //        new DangerLevelEntity
        //        {
        //            CustId = 1,
        //            DangerLevel = 1,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:50:55")
        //        },
        //         new DangerLevelEntity {
        //            CustId = 2,
        //            DangerLevel = 2,
        //            ScannedTime = DateTime.Parse("2021-07-05 22:51:55")
        //         }
        //    };

        //    var dataResult = new DBResult
        //    {
        //        ErrorMsg = string.Empty,
        //        ErrorCode = 0
        //    };

        //    var custIds = string.Join(",", lastestCustomerClassification.Select(c => c.CustId));
        //    custClassToMainDBDataAccess.GetDangerLevel(custIds, DangerLevel.Original).Returns(dangerLevelEnity);

        //    custClassToMainDBDataAccess.UpdateDangerLevelToMainDB(dangerLevelEnity, DangerLevel.Original).Returns(dataResult);

        //    var jsonUpdateApplyCCs = JsonConvert.SerializeObject(dangerLevelEnity);
        //    custClassToMainDBDataAccess.UpdateAppliedDangerLevel(jsonUpdateApplyCCs, DangerLevel.Original);

        //    var result = custClassToMainDBService.UpdateDangerLevelToMainDB(lastestCustomerClassification, DangerLevel.Original, "Update DangerLevel To MainDB", false);

        //    result.Should().BeTrue();
        //}
    }
}