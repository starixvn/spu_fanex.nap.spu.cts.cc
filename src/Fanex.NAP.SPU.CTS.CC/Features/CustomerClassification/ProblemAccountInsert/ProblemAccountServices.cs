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
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountServices
    {
        IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(IEnumerable<long> custIds);

        InsertPAResultModel InsertProblemAccount(IEnumerable<ProblemAccountCategoryEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing, string serviceName);

        IEnumerable<CustInfoEntity> InsertRobotAccount(IEnumerable<InsertRobotEntity> robotAccounts);

        void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName);
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

        public IEnumerable<ExistedProblemAccountEntity> GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var custIdsToString = string.Join(",", custIds);

                return problemAccountDataAccess.GetExistedProblemAccounts(custIdsToString);
            }

            return Enumerable.Empty<ExistedProblemAccountEntity>();
        }

        public InsertPAResultModel InsertProblemAccount(IEnumerable<ProblemAccountCategoryEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing, string serviceName)
        {
            var result = new InsertPAResultModel();

            if (problemAccounts != null && problemAccounts.Any())
            {
                var probAccounts = MergePALosingClassification(problemAccounts, paLosing);

                result = InsertProblemAccountByCategory(probAccounts, serviceName);

                if (result.ErrorCode == 0)
                {
                    InsertPALogics(result.customerInfo, serviceName);
                }
            }

            return result;
        }

        public IEnumerable<CustInfoEntity> InsertRobotAccount(IEnumerable<InsertRobotEntity> robotAccounts)
        {
            if (robotAccounts != null && robotAccounts.Any())
            {
                var customerInfo = problemAccountDataAccess.InsertRobotAccount(JsonConvert.SerializeObject(robotAccounts));

                CheckNewPAAssociationCustomerForRollback(customerInfo);

                return customerInfo;
            }

            return Enumerable.Empty<CustInfoEntity>();
        }

        public void InsertPALogics(IEnumerable<CustInfoEntity> customerInfo, string serviceName)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                CheckNewPAAssociationCustomerForRollback(customerInfo);

                var custIds = customerInfo.Select(x => x.CustID).Distinct();

                UpdateBetLimitOfFlaggedCustomer(custIds, serviceName);
            }
        }

        #region Private Methods

        private void CheckNewPAAssociationCustomerForRollback(IEnumerable<CustInfoEntity> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
            }
        }

        private InsertPAResultModel InsertProblemAccountByCategory(IEnumerable<InsertProblemAccountByCategoryEntity> problemAccounts, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountDataAccess.InsertProblemAccount(JsonConvert.SerializeObject(problemAccounts), nameof(InsertPA_ByType.Category));

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccountByCategory", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
                }

                return result;
            }

            return new InsertPAResultModel();
        }

        private InsertPAResultModel InsertProblemAccountByReason(IEnumerable<InsertProblemAccountByReasonEntity> problemAccounts, string serviceName)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var result = problemAccountDataAccess.InsertProblemAccount(JsonConvert.SerializeObject(problemAccounts), nameof(InsertPA_ByType.Reason));

                if (result.ErrorCode != 0)
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccountByReason", problemAccounts, new Exception(result.ErrorMessage), sentryClient);
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

        private IEnumerable<InsertProblemAccountByCategoryEntity> MergePALosingClassification(IEnumerable<ProblemAccountCategoryEntity> problemAccounts, IEnumerable<ProblemAccountLosingClassificationEntity> paLosing)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                return problemAccounts.GroupJoin(paLosing ?? Enumerable.Empty<ProblemAccountLosingClassificationEntity>(),
                                                 pa => pa.CustID,
                                                 pl => pl.CustId,
                                                (pa, pl) => new { pa, pl })
                                                .SelectMany(
                                                        group => group.pl.DefaultIfEmpty(),
                                                        (group, temp) => new InsertProblemAccountByCategoryEntity
                                                        {
                                                            CTSCustID = group.pa.CTSCustID,
                                                            CustID = group.pa.CustID,
                                                            CategoryID = group.pa.CategoryID,
                                                            CategoryGroup = group.pa.CategoryGroup,
                                                            IsDirectPA = group.pa.IsDirectPA,
                                                            IsLicensee = group.pa.IsLicensee,
                                                            Remark = group.pa.Remark,
                                                            RoleID = group.pa.RoleID,
                                                            SubscriberID = group.pa.SubscriberID,
                                                            CreatedBy = StarixITId,
                                                            BetCount = temp?.BetCount,
                                                            ActiveDays = temp?.ActiveDays,
                                                            TurnoverRM = temp?.TurnoverRM,
                                                            WinlossRM = temp?.WinlossRM,
                                                            WinlossStatus = temp?.WinlossStatus
                                                        });
            }

            return Enumerable.Empty<InsertProblemAccountByCategoryEntity>();
        }

        #endregion
    }
}