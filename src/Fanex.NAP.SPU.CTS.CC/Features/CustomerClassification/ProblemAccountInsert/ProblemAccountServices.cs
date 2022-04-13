namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
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

        IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime);

        InsertCategoryResultEntity InsertProblemAccount(IEnumerable<ProblemAccountsEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> probAccountLosing, bool isRescanFlow, string serviceName);

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

            var problemAccounts = problemAccountDataAccess.GetProblemAccounts_IBC(custIDs, dateTime);

            return problemAccounts ?? new List<ProblemAccountsEntity>();
        }

        public IEnumerable<ProblemAccountsEntity> GetProblemAccounts_IBC(string custIDs, DateTime? dateTime)
        {
            return problemAccountDataAccess.GetProblemAccounts_IBC(custIDs, dateTime);
        }

        public InsertCategoryResultEntity InsertProblemAccount(IEnumerable<ProblemAccountsEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> probAccountLosing, bool isRescanFlow, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var proAccountClassification = MergeProblemAccountClassification(problemAccounts, probAccountLosing);

                if (proAccountClassification.Any())
                {
                    var result = InsertProblemAccount(proAccountClassification, isRescanFlow, serviceName);

                    if (result.ErrorCode == 0)
                    {
                        CheckNewPAAssociationCustomerForRollback(result.NewPAAssociationCustomerForRollback);
                    }

                    return result;
                }
            }

            return new InsertCategoryResultEntity { CustIds = new List<long>(), ErrorCode = 0, ErrorMessage = string.Empty };
        }

        public void CheckNewPAAssociationCustomerForRollback(IEnumerable<CheckPACustomerAssociationForRollbackEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }
        }

        #region Private methods

        private static IEnumerable<InsertProblemAccountEntity> MergeProblemAccountClassification(IEnumerable<ProblemAccountsEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> probAccountLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(probAccountLosing,
                                            pa => pa.CustID,
                                            paLosing => paLosing.CustId,
                                            (pa, paLosing) => new { pa, paLosing })
                                            .SelectMany(
                                                 group => group.paLosing.DefaultIfEmpty(),
                                                 (group, temp) => new InsertProblemAccountEntity
                                                 {
                                                     CustID = group.pa.CustID,
                                                     ReasonID = group.pa.ReasonID,
                                                     RobotCounter = group.pa.RobotCounter,
                                                     BetCount = (temp != null) ? temp.BetCount : null,
                                                     ActiveDays = (temp != null) ? temp.ActiveDays : null,
                                                     TurnoverRM = (temp != null) ? temp.TurnoverRM : null,
                                                     WinlossRM = (temp != null) ? temp.WinlossRM : null,
                                                     WinlossStatus = (temp != null) ? temp.WinlossStatus : null
                                                 });
            }

            return Enumerable.Empty<InsertProblemAccountEntity>();
        }

        private InsertCategoryResultEntity InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> problemAccounts, bool isRescanFlow, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var problemAccountJson = JsonConvert.SerializeObject(problemAccounts);

                var result = problemAccountDataAccess.InsertProblemAccount(problemAccountJson, isRescanFlow);

                if (result.ErrorCode == -1)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccountToCTS throws Exception", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }

                return result;
            }

            return new InsertCategoryResultEntity { CustIds = new List<long>(), ErrorCode = 0, ErrorMessage = string.Empty };
        }

        #endregion Private methods
    }
}