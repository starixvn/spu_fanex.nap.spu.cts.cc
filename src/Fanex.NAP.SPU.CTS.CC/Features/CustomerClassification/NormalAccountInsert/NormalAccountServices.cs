namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountServices
    {
        GeneralCustomerClassificationEnity GetNewClassification(string serviceName, bool IsWriteLog);

        IEnumerable<TaggedCustomerEntity> GetCustomerAssociationProblemAccount(IEnumerable<CustomerClassificationEntity> listClassification);

        bool UpdateAppliedStatusClassification(IEnumerable<CustomerClassificationEntity> custClassifications, bool isSuccess);

        InsertCategoryResultEntity InsertMainCustomerClassification(IEnumerable<CustomerClassificationEntity> listClassification, string serviceName, bool isWriteLog);

        List<CustomerClassificationEntity> MergeWithTaggedCust(IEnumerable<CustomerClassificationEntity> custClassification, IEnumerable<TaggedCustomerEntity> custAssociationPA);

        IEnumerable<TaggedCustomerEntity> GetGroupBettingAndRejectCustomer(IEnumerable<CustomerClassificationEntity> listClassification);
    }

    public class NormalAccountServices : INormalAccountServices
    {
        private readonly INormalAccountDataAccess normalAccountDataAccess;
        private readonly ISentryClient sentryClient;

        public NormalAccountServices(INormalAccountDataAccess normalAccountDataAccess, ISentryClient sentryClient)
        {
            this.normalAccountDataAccess = normalAccountDataAccess;
            this.sentryClient = sentryClient;
        }

        public GeneralCustomerClassificationEnity GetNewClassification(string serviceName, bool IsWriteLog)
        {
            var newClassifications = new GeneralCustomerClassificationEnity
            {
                GeneralCustClassifications = normalAccountDataAccess.GetNewClassification(),
                IsContinute = false
            };

            if (newClassifications.GeneralCustClassifications != null && newClassifications.GeneralCustClassifications.Any())
            {
                newClassifications.IsContinute = true;
                newClassifications.GeneralCustClassifications = newClassifications.GeneralCustClassifications.Where(c => c.IsGeneralCategory);
            }

            Utilities.LogSentryInfo(serviceName, "Get ProblemAccount", newClassifications.GeneralCustClassifications, IsWriteLog, sentryClient);
            return newClassifications;
        }

        public IEnumerable<TaggedCustomerEntity> GetCustomerAssociationProblemAccount(IEnumerable<CustomerClassificationEntity> listClassification)
        {
            if (listClassification != null && listClassification.Any())
            {
                var custIds = listClassification.Select(x => x.CustId).Distinct();

                return normalAccountDataAccess.GetCustsAssociationProblemAccount(string.Join(",", custIds));
            }

            return new List<TaggedCustomerEntity>();
        }
        
        public IEnumerable<TaggedCustomerEntity> GetGroupBettingAndRejectCustomer(IEnumerable<CustomerClassificationEntity> listClassification)
        {
            if (listClassification != null && listClassification.Any())
            {
                var custIds = listClassification.Select(x => x.CustId).Distinct();

                return normalAccountDataAccess.CheckGroupBettingAndRejectCustomer(string.Join(",", custIds));
            }

            return new List<TaggedCustomerEntity>();
        }

        public InsertCategoryResultEntity InsertMainCustomerClassification(IEnumerable<CustomerClassificationEntity> listClassification, string serviceName, bool isWriteLog)
        {
            var custClassJsonData = JsonConvert.SerializeObject(listClassification);
            var result = new InsertCategoryResultEntity();

            if (!string.IsNullOrEmpty(custClassJsonData))
            {
                result = normalAccountDataAccess.InsertMainCustomerClassification(custClassJsonData);
                Utilities.LogSentryInfo(serviceName, "InsertMainCustomerClassification done", custClassJsonData, isWriteLog, sentryClient);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccount throws Exception", custClassJsonData, new Exception(result.ErrorMessage), sentryClient);
                }
            }

            return result;
        }

        public bool UpdateAppliedStatusClassification(IEnumerable<CustomerClassificationEntity> custClassifications, bool isSuccess)
        {
            if (isSuccess && custClassifications != null && custClassifications.Any())
            {
                var custIds = custClassifications.Select(x => x.CustId).Distinct();

                normalAccountDataAccess.UpdateAppliedStatusClassification(string.Join(",", custIds), true);
            }

            return true;
        }

        public List<CustomerClassificationEntity> MergeWithTaggedCust(IEnumerable<CustomerClassificationEntity> custClassification, IEnumerable<TaggedCustomerEntity> taggedCustomers)
        {
            if (custClassification == null || !custClassification.Any())
            {
                if (taggedCustomers == null || !taggedCustomers.Any())
                {
                    return custClassification.ToList();
                }

                return new List<CustomerClassificationEntity>();
            }

            var result = custClassification.GroupJoin(taggedCustomers,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new CustomerClassificationEntity
                                    {
                                        CustId = group.custClass.CustId,
                                        SportGroupId = group.custClass.SportGroupId,
                                        CategoryId = group.custClass.CategoryId,
                                        CreatedTime = group.custClass.CreatedTime,
                                        TurnoverRM = group.custClass.TurnoverRM,
                                        WinlossRM = group.custClass.WinlossRM,
                                        BetCount = group.custClass.BetCount,
                                        ActiveDays = group.custClass.ActiveDays,
                                        LastXDaysMargin = group.custClass.LastXDaysMargin,
                                        ProbationPeriodWinloss = group.custClass.ProbationPeriodWinloss,
                                        IsGeneralCategory = group.custClass.IsGeneralCategory,
                                        TaggingID = (temp != null) ? temp.TaggingID : group.custClass.TaggingID,
                                        TaggingType = (temp != null) ? temp.TaggingType : group.custClass.TaggingType,
                                        TWBetCount = (temp != null) ? temp.TWBetCount : 0,
                                        TWDesktopUsageRate = (temp != null) ? temp.TWDesktopUsageRate : 0,
                                        TWGroupBettingRate = (temp != null) ? temp.TWGroupBettingRate : 0,
                                        TWTicketRejectRate = (temp != null) ? temp.TWTicketRejectRate: 0
                                    });

            return result.ToList();
        }
    }
}