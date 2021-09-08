namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public interface IRobotAccountInsertDataAccess
    {
        IEnumerable<RobotUserEntity> GetRobotUsers(DateTime fromDate, string creditSiteList);
    }

    public class RobotAccountInsertDataAccess : IRobotAccountInsertDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotAccountInsertDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RobotUserEntity> GetRobotUsers(DateTime fromDate, string creditSiteList)
        {
            var criteria = new RobotUserGetCriteria
            {
                FromDate = fromDate,
                CreditSite = creditSiteList
            };

            return repository.Fetch<RobotUserEntity>(criteria);
        }
    }
}