namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemAccountServices
    {
        void CheckNewPAAssociationCustomerForRollback(IEnumerable<CheckPACustomerAssociationForRollbackEntity> assCustForRollbacks);
    }

    public class ProblemAccountServices : IProblemAccountServices
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ISentryClient sentryClient;

        public ProblemAccountServices(IProblemAccountDataAccess problemAccountDataAccess, ISentryClient sentryClient)
        {
            this.problemAccountDataAccess = problemAccountDataAccess;
            this.sentryClient = sentryClient;
        }

        public void CheckNewPAAssociationCustomerForRollback(IEnumerable<CheckPACustomerAssociationForRollbackEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }
        }

        //public void UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custIds)
        //{
        //    if (custIds != null && custIds.Any())
        //    {
        //        var custIdsToString = string.Join(",", custIds);

        //        var dbResult = problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(custIdsToString);

        //        if (dbResult.ErrorCode != 0)
        //        {
        //            LogService.Error("CTS.CC Update Bet Limit Of Flagged Customer throws exception: " + dbResult.ErrorMsg);
        //        }
        //    }
        //}
    }
}