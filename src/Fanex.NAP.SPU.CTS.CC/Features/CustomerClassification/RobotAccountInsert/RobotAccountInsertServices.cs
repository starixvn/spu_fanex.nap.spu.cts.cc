namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAccountInsertServices
    {
        IEnumerable<RobotUser> GetRobotUser(DateTime? fromDate, string creditSiteList);

        IEnumerable<ProblemAccountsEntity> MappingRobotCategory(IEnumerable<RobotUser> robotUsers);
    }

    public class RobotAccountInsertServices : IRobotAccountInsertServices
    {
        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;

        public RobotAccountInsertServices(IRobotAccountInsertDataAccess robotAccountInsertDataAccess)
        {
            this.robotAccountInsertDataAccess = robotAccountInsertDataAccess;
        }

        public IEnumerable<RobotUser> GetRobotUser(DateTime? fromDate, string creditSiteList)
        {
            if (fromDate.HasValue)
            {
                return robotAccountInsertDataAccess.GetRobotUsers(fromDate.Value, creditSiteList);
            }

            return new List<RobotUser>();
        }

        public IEnumerable<ProblemAccountsEntity> MappingRobotCategory(IEnumerable<RobotUser> robotUsers)
        {
            var custCategory = new List<ProblemAccountsEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new ProblemAccountsEntity
                {
                    CustID = c.CustId,
                    ReasonID = (int)Category.Robot
                }).ToList();
            }

            return custCategory;
        }
    }
}