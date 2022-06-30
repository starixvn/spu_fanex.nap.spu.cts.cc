namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMatchMonitorClassificationDataAccess
    {
        ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor();

        void ClassifyGBMatchMonitorCompleted(string mmDetailIDs);
    }

    public class MatchMonitorClassificationDataAccess : IMatchMonitorClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public MatchMonitorClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ClassifyGBMatchMonitorModel ClassifyGBMatchMonitor()
        {
            var criteria = new ClassifyGBMatchMonitorCriteria();

            var (custInfos, mmDetailIDs) = repository.FetchMultiple<CustInfosEntity, int>(criteria);

            return new ClassifyGBMatchMonitorModel
            {
                CustInfos = custInfos,
                MMDetailIDs = mmDetailIDs
            };
        }

        public void ClassifyGBMatchMonitorCompleted(string mmDetailIDs)
        {
            var criteria = new ClassifyGBMatchMonitorCompletedCriteria
            {
                MMDetailsJson = mmDetailIDs
            };

            repository.Execute(criteria);
        }
    }
}
