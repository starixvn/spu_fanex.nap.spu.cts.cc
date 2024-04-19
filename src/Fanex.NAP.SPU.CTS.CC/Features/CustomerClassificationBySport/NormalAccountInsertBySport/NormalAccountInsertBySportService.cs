namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;

    public interface INormalAccountInsertBySportService
    {
        GetNormalAccountBySportModel GetNormalAccount();

        InsertNormalAccountBySportModel InsertNormalAccount(IEnumerable<NormalAccountBySportEntity> custs, string serviceName);

        bool CompleteNormalAccountBySport(string customersXML, long scannedMaxId, bool isSuccess);

    }

    public class NormalAccountInsertBySportService : INormalAccountInsertBySportService
    {
        private readonly INormalAccountInsertBySportDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public NormalAccountInsertBySportService(INormalAccountInsertBySportDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public GetNormalAccountBySportModel GetNormalAccount()
        {
            return dataAccess.GetNormalAccount();
        }

        public InsertNormalAccountBySportModel InsertNormalAccount(IEnumerable<NormalAccountBySportEntity> custInfos, string serviceName)
        {
            var result = new InsertNormalAccountBySportModel();

            var custInfosJson = JsonConvert.SerializeObject(custInfos);

            if (!string.IsNullOrEmpty(custInfosJson))
            {
                result = dataAccess.InsertNormalAccount(custInfosJson);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertNormalAccount By Sport Renovate throws Exception", custInfosJson, new Exception(result.ErrorMessage), sentryClient);
                }
            }

            return result;
        }

        public bool CompleteNormalAccountBySport(string customersXML, long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.CompleteNormalAccountBySport(customersXML, scannedMaxId);
            }

            return true;
        }
    }

}
