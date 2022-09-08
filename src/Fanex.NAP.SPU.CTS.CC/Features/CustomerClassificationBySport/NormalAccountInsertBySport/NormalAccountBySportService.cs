namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;   

    public interface INormalAccountBySportService
    {
        IEnumerable<CustomerClassificationBySportEntity> GetNewClassification();

        InsertNormalAccountResultEntity InsertNormalAccountBySport(IEnumerable<CustomerClassificationBySportEntity> listClassification, string serviceName, bool isWriteLog);
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

        public IEnumerable<CustomerClassificationBySportEntity> GetNewClassification()
        {
            return normalAccountBySportDataAccess.GetNewClassificationBySport();
        }

        public InsertNormalAccountResultEntity InsertNormalAccountBySport(IEnumerable<CustomerClassificationBySportEntity> listClassification, string serviceName, bool isWriteLog)
        {
            var result = new InsertNormalAccountResultEntity();
            var custClassJsonData = JsonConvert.SerializeObject(listClassification);
            

            if (!string.IsNullOrEmpty(custClassJsonData))
            {
                result = normalAccountBySportDataAccess.InsertCustomerClassification(custClassJsonData);

                Utilities.LogSentryInfo(serviceName, "InsertNormalAccountBySport done", custClassJsonData, isWriteLog, sentryClient);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertNormalAccountBySport throws Exception", custClassJsonData, new Exception(result.ErrorMessage), sentryClient);
                }
            }

            return result;
        }
    }
}
