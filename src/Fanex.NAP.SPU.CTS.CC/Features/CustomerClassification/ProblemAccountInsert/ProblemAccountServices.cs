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

        InsertCategoryResultEntity InsertProblemAccount(IEnumerable<ProblemAccountsEntity> problemAccounts, bool isRescanFlow, string serviceName, bool isWriteLog);

        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime);

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

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts(DateTime? dateTime, string serviceName, bool isWriteLog)
        {
            string custIDs = (dateTime == null) ? Constants.InitialProblemAccounts : Constants.ProblemAccountByDateTime;

            var problemAccounts = GetProblemAccounts_IBC(custIDs, dateTime);

            return problemAccounts ?? new List<ProblemAccountsEntity>();
        }

        public InsertCategoryResultEntity InsertProblemAccount(IEnumerable<ProblemAccountsEntity> problemAccounts, bool isRescanFlow, string serviceName, bool isWriteLog)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var problemAccountJson = JsonConvert.SerializeObject(problemAccounts);

                var result = problemAccountDataAccess.InsertProblemAccount(problemAccountJson, isRescanFlow);

                if (result.ErrorCode == -1)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccountToCTS throws Exception", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }
                else
                {
                    CheckNewPAAssociationCustomerForRollback(result.NewPAAssociationCustomerForRollback);
                }

                return result;
            }

            return new InsertCategoryResultEntity { CustIds = new List<long>(), ErrorCode = 0, ErrorMessage = string.Empty };
        }

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime)
        {
            return problemAccountDataAccess.GetProblemAccounts_IBC(custIDs, dateTime);
        }

        public void CheckNewPAAssociationCustomerForRollback(IEnumerable<CheckPACustomerAssociationForRollbackEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }
        }
    }
}