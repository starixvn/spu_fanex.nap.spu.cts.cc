namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWInitialSmartBySportService
    {
        public ITWInitialSmartBySportService Start();

        public ITWInitialSmartBySportService GetCustInitialSmartBySport(int internalBatchSize);

        public ITWInitialSmartBySportService InsertTWInitialSmartClassification(PushDataToExternalEntity paras);

        public bool UpdateLastProcessed();
    }

    public class TWInitialSmartBySportService : ProblemAccountServices, ITWInitialSmartBySportService
    {
        private readonly ITWInitialSmartBySportDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly ISystemParameterServices _systemParameter;
        private const string ServiceName = "Realtime TW Initial Smart JobService";

        private TWInitialSmartBySportModel TWSmartCustData;

        public TWInitialSmartBySportService(
            IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ITWInitialSmartBySportDataAccess dataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            ISystemParameterServices systemParameter)
            : base(baseDataAccess, problemAccountDataAccess)
        {
            _dataAccess = dataAccess;
            _custClassToMainDBService = custClassToMainDBService;
            _systemParameter = systemParameter;
        }

        public ITWInitialSmartBySportService Start()
        {
            TWSmartCustData = new TWInitialSmartBySportModel();

            return this;
        }

        public ITWInitialSmartBySportService GetCustInitialSmartBySport(int internalBatchSize)
        {
            TWSmartCustData = _dataAccess.GetCustInitialSmartBySport(internalBatchSize);

            return this;
        }

        public ITWInitialSmartBySportService InsertTWInitialSmartClassification(PushDataToExternalEntity paras)
        {
            if (TWSmartCustData.CustInfos.IsNotNullOrEmpty())
            {
                var pas = TWSmartCustData.CustInfos.Select(x => new InsertProblemAccountEntity(x));

                var pasChunks = pas.Chunk(paras.ExternalBatchSize);

                foreach (var pasChunk in pasChunks)
                {
                    var paCustIDs = InsertProblemAccountPackage(pasChunk, FunctionId.TWInitialSmartClassification, CCInputFlow.Potential, false, ServiceName);

                    _custClassToMainDBService.PushExternalPackageByCustId(paCustIDs, paras);
                }
            }

            return this;
        }

        public bool UpdateLastProcessed()
        {
            if (TWSmartCustData.CustInfos.IsNotNullOrEmpty())
            {
                // need update systemid
                _systemParameter.UpdateTimingExecution(TWSmartCustData.NextScannedCustID.ToString(), CTS_SystemParameters.TWInitialSmartBySportClassification_LastCustID);
                _systemParameter.UpdateTimingExecution(TWSmartCustData.NextScannedDateTime.ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.TWInitialSmartBySportClassification_LastScanDate);

                return true;
            }

            return false;
        }
    }
}
