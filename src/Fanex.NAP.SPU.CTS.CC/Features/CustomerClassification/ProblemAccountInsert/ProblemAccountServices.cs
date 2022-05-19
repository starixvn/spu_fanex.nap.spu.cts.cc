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
        IEnumerable<ProblemAccountEntity> GetExistedProblemAccounts(IEnumerable<long> custIds);

        InsertPAResultModel InsertProblemAccount(IEnumerable<ProblemAccountEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing, string serviceName);

        void InsertPALogics(IEnumerable<AssCustomerForCheckingRollbackEntity> customerInfo, string serviceName);
    }

    public class ProblemAccountServices : IProblemAccountServices
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ISentryClient sentryClient;

        public ProblemAccountServices(
            IProblemAccountDataAccess problemAccountDataAccess,
            ISentryClient sentryClient)
        {
            this.problemAccountDataAccess = problemAccountDataAccess;
            this.sentryClient = sentryClient;
        }

        public IEnumerable<ProblemAccountEntity> GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var custIdsToString = string.Join(",", custIds);

                return problemAccountDataAccess.GetExistedProblemAccounts(custIdsToString);
            }

            return Enumerable.Empty<ProblemAccountEntity>();
        }

        public InsertPAResultModel InsertProblemAccount(IEnumerable<ProblemAccountEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing, string serviceName)
        {
            var result = new InsertPAResultModel();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var probAccounts = MergePALosingClassification(problemAccounts, paLosing);

                result = InsertProblemAccountByCategory(probAccounts, serviceName);

                if (result.ErrorCode != 0)
                {
                    return result;
                }

                InsertPALogics(result.customerInfo, serviceName);
            }

            return result;
        }

        public void InsertPALogics (IEnumerable<AssCustomerForCheckingRollbackEntity> customerInfo, string serviceName)
        {
            if(customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(customerInfo);

                var custIds = customerInfo.Select(x => x.CustID).Distinct();

                UpdateBetLimitOfFlaggedCustomer(custIds, serviceName);
            }
        }
            
        #region Private Methods

        private bool CheckNewPAAssociationCustomerForRollback(IEnumerable<AssCustomerForCheckingRollbackEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }

            return true;
        }

        private InsertPAResultModel InsertProblemAccountByCategory(IEnumerable<ProblemAccountViewEntity> problemAccounts, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountDataAccess.InsertProblemAccountByCategory(JsonConvert.SerializeObject(problemAccounts));

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccountByCategory", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }

                return result;
            }

            return new InsertPAResultModel();
        }

        private bool UpdateBetLimitOfFlaggedCustomer(IEnumerable<long> custId, string serviceName)
        {
            if (custId != null && custId.Any())
            {
                var custIdsToString = string.Join(",", custId);

                var result = problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(custIdsToString);

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "UpdateBetLimitOfFlaggedCustomer", custId, new Exception(result.ErrorMsg), sentryClient);

                    return false;
                }
            }

            return true;
        }

        private bool UpdateBetLimitOfUnflaggedCustomer(IEnumerable<long> custId, string serviceName)
        {
            if (custId != null && custId.Any())
            {
                var custIdsToString = string.Join(",", custId);

                var result = problemAccountDataAccess.UpdateBetLimitOfUnflaggedCustomer(custIdsToString);

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "UpdateBetLimitOfUnflaggedCustomer", custId, new Exception(result.ErrorMsg), sentryClient);

                    return false;
                }
            }

            return true;
        }

        private IEnumerable<ProblemAccountViewEntity> MergePALosingClassification(IEnumerable<ProblemAccountEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(paLosing ?? Enumerable.Empty<ProblemAccountLosingClassificationEntity>(),
                                                 pa => pa.CustID,
                                                 pl => pl.CustId,
                                                (pa, pl) => new { pa, pl })
                                                .SelectMany(
                                                        group => group.pl.DefaultIfEmpty(),
                                                        (group, temp) => new ProblemAccountViewEntity
                                                        {
                                                            CustID = group.pa.CustID,
                                                            CategoryID = group.pa.CategoryID,
                                                            CreatedBy = group.pa.CreatedBy,
                                                            BetCount = temp?.BetCount,
                                                            ActiveDays = temp?.ActiveDays,
                                                            TurnoverRM = temp?.TurnoverRM,
                                                            WinlossRM = temp?.WinlossRM,
                                                            WinlossStatus = temp?.WinlossStatus
                                                        });
            }

            return Enumerable.Empty<ProblemAccountViewEntity>();
        }

        #endregion
    }
}