using Fanex.NAP.SPU.CTS.Features.Transform.Accounts;
using Fanex.NAP.SPU.CTS.Features.Transform.Accounts.Entities;
using Fanex.NAP.SPU.CTS.Shares;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fanex.NAP.SPU.CTS.Tests.Features.Accounts
{
    public class AccountTests
    {
        private readonly IAccountDataAccess accountDataAccess;
        private readonly IAccountServices accountServices;

        public AccountTests()
        {
            accountDataAccess = Substitute.For<IAccountDataAccess>();

            accountServices = new AccountServices(accountDataAccess);
        }

        [Fact]
        public void GetRawAccount_WhenSuccess_ReturnAccountList()
        {
            var accountEntity = new List<AccountEntity> {
                new AccountEntity
                {
                   AccountId = 1,
                   BatchId = 1,
                   LastLoginTime = DateTime.Now,
                   LoginName = "abc",
                   SubscriberId = 1
                },
                 new AccountEntity
                {
                   AccountId = 2,
                   BatchId = 1,
                   LastLoginTime = DateTime.Now,
                   LoginName = "abczzz",
                   SubscriberId = 1
                },
                  new AccountEntity
                {
                   AccountId = 3,
                   BatchId = 1,
                   LastLoginTime = DateTime.Now,
                   LoginName = "abcxxx",
                   SubscriberId = 1
                },
                new AccountEntity
                {
                   AccountId = 4,
                   BatchId = 2,
                   LastLoginTime = DateTime.Now,
                   LoginName = "xxx",
                   SubscriberId = 1
                },
                 new AccountEntity
                {
                   AccountId = 5,
                   BatchId = 2,
                   LastLoginTime = DateTime.Now,
                   LoginName = "xxxabc",
                   SubscriberId = 1
                },
                  new AccountEntity
                {
                   AccountId = 6,
                   BatchId = 2,
                   LastLoginTime = DateTime.Now,
                   LoginName = "xxxzzz",
                   SubscriberId = 1
                },
            };

            accountDataAccess.GetRawAccount(100, 2).Returns(accountEntity);

            var result = accountServices.GetRawAccount(100, 2);

            result.Should().BeEquivalentTo(accountEntity);
        }

        [Fact]
        public void TransformAccount_WhenSuccess_ReturnDataResult()
        {
            var accountEntity = new List<AccountEntity> {
                new AccountEntity
                {
                   AccountId = 1,
                   BatchId = 1,
                   LastLoginTime = DateTime.Now,
                   LoginName = "abc",
                   SubscriberId = 1
                },
                 new AccountEntity
                {
                   AccountId = 2,
                   BatchId = 1,
                   LastLoginTime = DateTime.Now,
                   LoginName = "abczzz",
                   SubscriberId = 1
                }
            };

            var jsonResult = JsonConvert.SerializeObject(accountEntity);

            var expectedResult = new DataResult { 
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            accountDataAccess.TransformDCSAccount(jsonResult).Returns(expectedResult);

            var result = accountServices.TransformAccount(accountEntity);

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}