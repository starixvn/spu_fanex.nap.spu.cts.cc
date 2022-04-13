namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyProblemClassificationDataAccess
    {
        ProblemClassificationDailyEntity GetCustomerForProblemDailyScanning();

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(string custIds, FunctionId funcId);

        IEnumerable<CheckPACustomerAssociationForRollbackEntity> InsertProblemProbationClassification(string problemAccountJson);
    }

    public class DailyProblemClassificationDataAccess : IDailyProblemClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyProblemClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ProblemClassificationDailyEntity GetCustomerForProblemDailyScanning()
        {
            var criteria = new ProblemClassificationForDailyScanningGetCriteria();

            var result = repository.Fetch<Int64>(criteria);

            return new ProblemClassificationDailyEntity
            {
                CustID = result ?? Enumerable.Empty<Int64>(),
                op_LastCategoryID = criteria.LastCategoryID,
                op_LastCustID = criteria.LastCustID
            };
        }

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(string custIds, FunctionId funcId)
        {
            var criteria = new ProblemAccountLosingClassificationGetCriteria()
            {
                ListCustIds = custIds,
                FunctionId = (int)funcId
            };

            return repository.Fetch<ProblemAccountLosingClassificationEntity>(criteria);
        }

        public IEnumerable<CheckPACustomerAssociationForRollbackEntity> InsertProblemProbationClassification(string problemAccountJson)
        {
            var criteria = new ProblemAccountProbationInsertCriteria
            {
                ProblemAccountJson = problemAccountJson
            };

            return repository.Fetch<CheckPACustomerAssociationForRollbackEntity>(criteria);
        }
    }
}