namespace Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface ITWRobotImpervaDataAccess
    {
        TWRobotImpervaGetModel GetTWRobotImperva(long lastLogID);

        void InsertTWRobotImperva(string robotInfos);
    }

    public class TWRobotImpervaDataAccess : ITWRobotImpervaDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWRobotImpervaDataAccess()
        {
            repository = new DynamicRepository();
        }        

        public TWRobotImpervaGetModel GetTWRobotImperva(long lastLogID)
        {
            var result = new TWRobotImpervaGetModel();

            var criteria = new TWRobotImpervaGetCriteria
            {
                LastLogID = lastLogID
            };

            result.RobotInfos = repository.Fetch<TWRobotImpervaGetEntity>(criteria);
            result.MaxLogID = criteria.MaxLogID;

            return result;
        }

        public void InsertTWRobotImperva(string robotInfos)
        {
            if (robotInfos.Any())
            {
                var criteria = new TWRobotImpervaInsertCriteria
                {
                    RobotInfo = JsonConvert.SerializeObject(robotInfos)
                };

                repository.Execute(criteria);
            }
        }

    }
}
