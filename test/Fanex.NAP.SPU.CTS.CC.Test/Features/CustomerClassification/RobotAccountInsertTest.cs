namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RobotAccountInsert;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotAccountInsertTest
    {
        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;

        public RobotAccountInsertTest()
        {
            robotAccountInsertDataAccess = Substitute.For<IRobotAccountInsertDataAccess>();
            robotAccountInsertServices = new RobotAccountInsertServices(robotAccountInsertDataAccess);
        }

        [Fact]
        public void GetRobotUsers_WhenSuccess_ReturnRobotAccounts()
        {
            var fromDate = DateTime.Now;

            var robotAccounts = new List<RobotUserEntity>
            {
                new RobotUserEntity
                {
                    CustId =1,
                    RobotDate = DateTime.Parse("2021-07-05 22:50:55")
                },
                new RobotUserEntity
                {
                    CustId = 2,
                    RobotDate = DateTime.Parse("2021-07-07 22:50:55")
                }
            };

            robotAccountInsertDataAccess.GetRobotUsers(fromDate, "1234").Returns(robotAccounts);

            var result = robotAccountInsertServices.GetRobotUser(fromDate, "1234");

            result.Should().BeEquivalentTo(robotAccounts);
        }

        [Fact]
        public void GetRobotUsers_WhenSuccess_ReturnEmptyList()
        {
            var result = robotAccountInsertServices.GetRobotUser(null, "1234");

            result.Should().BeEquivalentTo(new List<RobotUserEntity>());
        }

        [Fact]
        public void MappingRobotCategory_WhenSuccess_ReturnProblemAccounts()
        {
            var robotAccounts = new List<RobotUserEntity>
            {
                new RobotUserEntity
                {
                    CustId =1,
                    RobotDate = DateTime.Parse("2021-07-05 22:50:55")
                },
                new RobotUserEntity
                {
                    CustId = 2,
                    RobotDate = DateTime.Parse("2021-07-07 22:50:55")
                }
            };

            var problemAccounts = new List<ProblemAccountsEntity>
            {
                new ProblemAccountsEntity
                {
                    CustID=1,
                    ReasonID=(int)Category.Robot
                },
                  new ProblemAccountsEntity
                {
                    CustID=2,
                    ReasonID=(int)Category.Robot
                }
            };

            var result = robotAccountInsertServices.MappingRobotCategory(robotAccounts);

            result.Should().BeEquivalentTo(problemAccounts);
        }

        [Fact]
        public void MappingRobotCategory_WhenFail_ReturnEmptyLists()
        {
            var problemAccounts = new List<ProblemAccountsEntity>
            {
                new ProblemAccountsEntity
                {
                    CustID=1,
                    ReasonID=(int)Category.Robot
                },
                  new ProblemAccountsEntity
                {
                    CustID=2,
                    ReasonID=(int)Category.Robot
                }
            };

            var result = robotAccountInsertServices.MappingRobotCategory(null);

            result.Should().NotBeEquivalentTo(problemAccounts);
        }
    }
}