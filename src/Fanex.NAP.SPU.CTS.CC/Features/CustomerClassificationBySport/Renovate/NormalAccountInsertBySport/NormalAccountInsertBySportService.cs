namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;

    public interface INormalAccountInsertBySportService
    {
        GetNormalAccountBySportModel GetNormalAccount();

        InsertNormalAccountBySportModel InsertNormalAccount(IEnumerable<InsertNormalAccountBySportModel> custs, string serviceName);

        bool CompleteNormalAccountBySport(string custIdList, long scannedMaxId, bool isSuccess);
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

        public InsertNormalAccountBySportModel InsertNormalAccount(IEnumerable<InsertNormalAccountBySportModel> custs, string serviceName)
        {
            var result = new InsertNormalAccountBySportModel();

            var custsJson = JsonConvert.SerializeObject(custs);

            if (!string.IsNullOrEmpty(custsJson))
            {
                result = dataAccess.InsertNormalAccount(custsJson);               

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertNormalAccount By Sport throws Exception", custsJson, new Exception(result.ErrorMessage), sentryClient);
                }
            }

            return result;
        }

        public bool CompleteNormalAccountBySport(string custIdList, long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.CompleteNormalAccountBySport(custIdList, scannedMaxId);
            }

            return true;
        }
    }
}
