namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotAccountInsertTest
    {
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private readonly ISystemParameterServices systemParameterServices;

        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;

        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;

        public RobotAccountInsertTest()
        {
            systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();
            systemParameterServices = new SystemParameterServices(systemParameterDataAccess);

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, null);

            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            problemAccountServices = new ProblemAccountServices(problemAccountDataAccess, null);

            robotAccountInsertDataAccess = Substitute.For<IRobotAccountInsertDataAccess>();
            robotAccountInsertServices = new RobotAccountInsertServices(robotAccountInsertDataAccess,
                                                                        systemParameterServices,
                                                                        custClassToMainDBService,
                                                                        problemAccountServices);
        }

        [Fact]
        public void GetRobotUsers_WhenSuccess_ReturnRobotAccounts()
        {
            var robotAccounts = new RobotUserViewEntity
            {
                RobotUsers = new List<RobotUserEntity>
                {
                    new RobotUserEntity
                    {
                        CustId =1,
                        Counter = 1
                    },
                    new RobotUserEntity
                    {
                        CustId = 2,
                        Counter = 2
                    }
                },
                MaxTimestamp = 2
            };

            var output = new SystemParameterEntity
            {
                ParameterDataType = "BIGINT UNSIGNED",
                ParameterValue = "1"
            };

            long lastTimestamp = 1;

            robotAccountInsertDataAccess.GetRobotUsersFromMainDb(lastTimestamp).Returns(robotAccounts);

            systemParameterDataAccess.GetPreviousTimingExecution(SystemParameters.RobotAccountLastScannedTime).Returns(output);

            robotAccountInsertServices.Start().GetLastTimespan().GetRobotUser();

            robotAccountInsertServices.RobotUserInfo.Should().BeEquivalentTo(robotAccounts);
        }

        [Fact]
        public void MappingRobotCategory_WhenSuccess_ReturnProblemAccounts()
        {
            var robotAccounts = new List<RobotUserEntity>
            {
                new RobotUserEntity
                {
                    CustId =1,
                    Counter = 1
                },
                new RobotUserEntity
                {
                    CustId = 2,
                    Counter = 2
                }
            };

            var problemAccounts = new List<RobotInsetViewEntity>
            {
                new RobotInsetViewEntity
                {
                    CustID=1,
                    ReasonID=(int)Category.Robot,
                    RobotCounter = 1
                },
                  new RobotInsetViewEntity
                {
                    CustID=2,
                    ReasonID=(int)Category.Robot,
                    RobotCounter = 2
                }
            };

            var result = robotAccountInsertServices.MappingRobotCategory(robotAccounts);

            result.Should().BeEquivalentTo(problemAccounts);
        }

        [Fact]
        public void MappingRobotCategory_WhenFail_ReturnEmptyLists()
        {
            var problemAccounts = new List<RobotInsetViewEntity>
            {
               new RobotInsetViewEntity
                {
                    CustID=1,
                    ReasonID=(int)Category.Robot,
                    RobotCounter = 1
                },
                  new RobotInsetViewEntity
                {
                    CustID=2,
                    ReasonID=(int)Category.Robot,
                    RobotCounter = 2
                }
            };

            var result = robotAccountInsertServices.MappingRobotCategory(null);

            result.Should().NotBeEquivalentTo(problemAccounts);
        }
    }
}