namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemAccountServices
    {
        IEnumerable<ProblemAccountsEntity> GetProblemAccounts(DateTime? dateTime, string serviceName, bool isWriteLog);

        InsertCategoryResultEntity InsertProblemAccountToCTS(IEnumerable<ProblemAccountsEntity> problemAccounts, bool isRescan, string serviceName, bool isWriteLog);

        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime);

        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime);

        void CheckNewPAAssociationCustomerForRollback(IEnumerable<NewPAAssociationCustomerForRollbackEntity> assCustForRollbacks, string serviceName);
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

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts(DateTime? dateTime, string serviceName, bool isWriteLog)
        {
            var problemAccounts = new List<ProblemAccountsEntity>();

            string custIDs = (dateTime == null) ? Constants.InitialProblemAccounts : Constants.ProblemAccountByDateTime;

            problemAccounts.AddRange(GetProblemAccounts_IBC(custIDs, dateTime));

            problemAccounts.AddRange(GetProblemAccounts_CN88(custIDs, dateTime));

            Utilities.LogSentryInfo(serviceName, "Get ProblemAccount", problemAccounts, isWriteLog, sentryClient);
            return problemAccounts;
        }

        public InsertCategoryResultEntity InsertProblemAccountToCTS(IEnumerable<ProblemAccountsEntity> problemAccounts, bool isRescan, string serviceName, bool isWriteLog)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var problemAccountJson = JsonConvert.SerializeObject(problemAccounts);

                var result = problemAccountDataAccess.InsertProblemAccount(problemAccountJson, isRescan);

                Utilities.LogSentryInfo(serviceName, "InsertProblemAccountToCTS", result.CustIds, isWriteLog, sentryClient);

                if (result.ErrorCode == -1)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccountToCTS throws Exception", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }
                else
                {
                    CheckNewPAAssociationCustomerForRollback(result.NewPAAssociationCustomerForRollback, serviceName);
                }

                return result;
            }

            return new InsertCategoryResultEntity { CustIds = new List<long>(), ErrorCode = 0, ErrorMessage = string.Empty };
        }

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime)
        {
            return problemAccountDataAccess.GetProblemAccounts_IBC(custIDs, dateTime);
        }

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts_CN88(string custIDs, DateTime? dateTime)
        {
            return problemAccountDataAccess.GetProblemAccounts_CN88(custIDs, dateTime);
        }

        public void CheckNewPAAssociationCustomerForRollback(IEnumerable<NewPAAssociationCustomerForRollbackEntity> assCustForRollbacks, string serviceName)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                try
                {
                    var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                    var updateResult = problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);

                    Utilities.LogSentryInfo(serviceName, "Check new PA association customer for rollback", custInfoJson, true, sentryClient);
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "CheckNewPAAssociationCustomerForRollback throws exception", assCustForRollbacks, ex, sentryClient);
                }
            }
        }

    }
}