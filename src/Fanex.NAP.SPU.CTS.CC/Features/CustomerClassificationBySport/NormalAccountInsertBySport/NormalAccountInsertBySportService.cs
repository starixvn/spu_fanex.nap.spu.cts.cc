namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalAccountInsertBySportService
    {
        GetNormalAccountBySportModel GetNormalClassificationBySport();

        ResultInsertClassificationBySportModel InsertNormalClassificationBySport(IEnumerable<NormalAccountBySportEntity> custs, string serviceName);

        void CompleteNormalAccountBySport(long scannedMaxId, bool isSuccess);
    }

    public class NormalAccountInsertBySportService : BaseCustomerClassificationBySportService, INormalAccountInsertBySportService
    {
        private readonly INormalAccountInsertBySportDataAccess dataAccess;

        public NormalAccountInsertBySportService(IBaseCustomerClassificationBySportDataAccess baseDataAccess, INormalAccountInsertBySportDataAccess dataAccess, ISentryClient sentryClient) : base(baseDataAccess, sentryClient)
        {
            this.dataAccess = dataAccess;
        }

        public GetNormalAccountBySportModel GetNormalClassificationBySport()
        {
            return dataAccess.GetNormalClasificationAccount();
        }

        public ResultInsertClassificationBySportModel InsertNormalClassificationBySport(IEnumerable<NormalAccountBySportEntity> custs, string serviceName)
        {
            var result = new ResultInsertClassificationBySportModel();

            if (custs != null && custs.Any())
            {
                var modelInsert = new InsertCustomerClassificationBySportModel()
                {
                    CustClassificationJson = JsonConvert.SerializeObject(custs)
                };

                result = base.InsertCustomerClassificationBySport(CCInputFlow.BySportInsertNormal, modelInsert, serviceName);
            }

            return result;
        }

        public void CompleteNormalAccountBySport(long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.CompleteNormalAccountBySport(scannedMaxId);
            }
        }
    }
}