namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountServices
    {
        GeneralCustomerClassificationEnity GetNewClassification(string serviceName, bool IsWriteLog);

        IEnumerable<CustAssociationProblemAccountEntity> GetCustomerAssociationProblemAccount(IEnumerable<CustomerClassificationEntity> listClassification);

        bool UpdateAppliedStatusClassification(IEnumerable<CustomerClassificationEntity> custClassifications, bool isSuccess);

        InsertCategoryResultEntity InsertMainCustomerClassification(IEnumerable<CustomerClassificationEntity> custClassifications
                                                        , IEnumerable<CustAssociationProblemAccountEntity> custAssociationPA
                                                        , string serviceName, bool isWriteLog);

        string MergeCustomerClassificationInfo(IEnumerable<CustomerClassificationEntity> custClassification, IEnumerable<CustAssociationProblemAccountEntity> custAssociationPA);
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

        public IEnumerable<CustAssociationProblemAccountEntity> GetCustomerAssociationProblemAccount(IEnumerable<CustomerClassificationEntity> listClassification)
        {
            if (listClassification != null && listClassification.Any())
            {
                var custIds = listClassification.Select(x => x.CustId).Distinct();

                return normalAccountDataAccess.GetCustsAssociationProblemAccount(string.Join(",", custIds));
            }

            return new List<CustAssociationProblemAccountEntity>();
        }

        public InsertCategoryResultEntity InsertMainCustomerClassification(IEnumerable<CustomerClassificationEntity> custClassifications
                                                                , IEnumerable<CustAssociationProblemAccountEntity> custAssociationPA
                                                                , string serviceName
                                                                , bool isWriteLog)
        {
            var result = new InsertCategoryResultEntity();

            string jsonClassification = MergeCustomerClassificationInfo(custClassifications, custAssociationPA);

            if (!string.IsNullOrEmpty(jsonClassification))
            {
                result = normalAccountDataAccess.InsertMainCustomerClassification(jsonClassification);
                Utilities.LogSentryInfo(serviceName, "InsertMainCustomerClassification done", jsonClassification, isWriteLog, sentryClient);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertProblemAccount throws Exception", jsonClassification, new Exception(result.ErrorMessage), sentryClient);
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

        public string MergeCustomerClassificationInfo(IEnumerable<CustomerClassificationEntity> custClassification, IEnumerable<CustAssociationProblemAccountEntity> custAssociationPA)
        {
            if (custAssociationPA == null || !custAssociationPA.Any()
                || custClassification == null || !custClassification.Any())
            {
                return string.Empty;
            }

            var result = custClassification.GroupJoin(custAssociationPA,
                               custClass => custClass.CustId,
                               custAss => custAss.CustID,
                               (custClass, custPA) => new { custClass, custPA })
                               .SelectMany(
                                    group => group.custPA.DefaultIfEmpty(),
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
                                        TaggingID = (temp != null) ? temp.TaggingID : 0,
                                        TaggingType = (temp != null) ? temp.TaggingType : 0
                                    });

            return JsonConvert.SerializeObject(result);
        }
    }
}