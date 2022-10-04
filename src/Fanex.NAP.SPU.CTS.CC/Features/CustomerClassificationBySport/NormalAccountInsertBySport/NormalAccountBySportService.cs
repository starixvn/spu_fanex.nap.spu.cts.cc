namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountBySportService
    {
        IEnumerable<CustomerClassificationBySportEntity> GetNewClassificationBySport();

        InsertNormalAccountResultEntity InsertCustomerClassificationBySport(IEnumerable<CustomerClassificationBySportEntity> listClassification, string serviceName, bool isWriteLog);

        bool UpdateAppliedStatusClassificationBySport(IEnumerable<CustomerClassificationBySportEntity> custClassifications, bool isSuccess);
    }

    public class NormalAccountBySportService : INormalAccountBySportService
    {
        private readonly INormalAccountBySportDataAccess normalAccountBySportDataAccess;
        private readonly ISentryClient sentryClient;

        public NormalAccountBySportService(INormalAccountBySportDataAccess normalAccountBySportDataAccess, ISentryClient sentryClient)
        {
            this.normalAccountBySportDataAccess = normalAccountBySportDataAccess;
            this.sentryClient = sentryClient;
        }

        public IEnumerable<CustomerClassificationBySportEntity> GetNewClassificationBySport()
        {
            return normalAccountBySportDataAccess.GetNewClassificationBySport();
        }

        public InsertNormalAccountResultEntity InsertCustomerClassificationBySport(IEnumerable<CustomerClassificationBySportEntity> listClassification, string serviceName, bool isWriteLog)
        {
            var result = new InsertNormalAccountResultEntity();
            var custClassJsonData = JsonConvert.SerializeObject(listClassification);
            
            if (!string.IsNullOrEmpty(custClassJsonData))
            {
                result = normalAccountBySportDataAccess.InsertCustomerClassificationBySport(custClassJsonData);

                Utilities.LogSentryInfo(serviceName, "InsertCustomerClassificationBySport done", custClassJsonData, isWriteLog, sentryClient);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertCustomerClassificationBySport throws Exception", custClassJsonData, new Exception(result.ErrorMessage), sentryClient);
                }
            }

            return result;
        }

        public bool UpdateAppliedStatusClassificationBySport(IEnumerable<CustomerClassificationBySportEntity> custClassifications, bool isSuccess)
        {
            if (isSuccess && custClassifications != null && custClassifications.Any())
            {
                var custs = custClassifications.Select(x => new UpdateApplyStatusCustEntity
                {
                    CustId = x.CustId,
                    SportId = x.SportId
                }).Distinct();
               
                normalAccountBySportDataAccess.UpdateAppliedStatusClassificationBySport(custs.ToXmlString());
            }

            return true;
        }
    }
}
