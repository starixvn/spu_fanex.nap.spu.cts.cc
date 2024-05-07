namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyProblemClassificationServices
    {
        IDailyProblemClassificationServices Start();

        IDailyProblemClassificationServices Reset();

        IDailyProblemClassificationServices GetPADailyScanFromQueue();

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(FunctionId funcId);

        IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification);

        bool PushCCToExternal(PushDataToExternalEntity paras);

        bool UpdateComplete();

        IDailyProblemClassificationServices InsertPALogic();
    }

    public class DailyProblemClassificationServices : IDailyProblemClassificationServices
    {
        private readonly IDailyProblemClassificationDataAccess dailyClassificationDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private IEnumerable<long> scanningCustIDs;
        private IEnumerable<InsertPAProbationResult> problemAccounts;
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly string serviceName = "ProblemAccountProbationService";

        public DailyProblemClassificationServices(
            IDailyProblemClassificationDataAccess dailyClassificationDataAccess,
            ISystemParameterServices systemParameterService,
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService,
            IRescanCustClassificationDataAccess rescanCustClassificationDataAccess)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
            this.systemParameterService = systemParameterService;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.rescanCustClassificationDataAccess = rescanCustClassificationDataAccess;
        }

        public IDailyProblemClassificationServices Start()
        {
            scanningCustIDs = Enumerable.Empty<long>();

            return this;
        }

        public IDailyProblemClassificationServices Reset()
        {
            problemAccounts = Enumerable.Empty<InsertPAProbationResult>();

            return this;
        }

        public IDailyProblemClassificationServices GetPADailyScanFromQueue()
        {
            scanningCustIDs = dailyClassificationDataAccess.GetPADailyScanFromQueue();

            return this;
        }

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(FunctionId funcId)
        {
            if (scanningCustIDs != null && scanningCustIDs.Any())
            {
                return problemAccountServices.GetProblemAccountLosingClassification(scanningCustIDs, funcId);
            }

            return Enumerable.Empty<ProblemAccountLosingClassificationEntity>();
        }

        public IDailyProblemClassificationServices InsertProblemAccountProbationClassification(int batchSize, IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification)
        {
            if (problemClassification != null && problemClassification.Any())
            {
                var jsonDailPas = GetTWInitialGBs(problemClassification);

                problemAccounts = dailyClassificationDataAccess.InsertProblemProbationClassification(jsonDailPas);
            }

            return this;
        }

        private string GetTWInitialGBs(IEnumerable<ProblemAccountLosingClassificationEntity> problemClassification)
        {
            if (problemClassification != null && problemClassification.Any())
            {
                var dicTWInitialGBs = rescanCustClassificationDataAccess.GetInitialTWGroupBetting(string.Join(",", scanningCustIDs)).ToDictionary(x => x.CustID);

                var dailyPas = problemClassification.Select(x =>
                {
                    var pa = new ProblemAccountLosingClassificationModel(x);

                    var twGBCust = default(RealtimeTWInitialGroupBetting.TWInitialGBGetChangesEntity);

                    dicTWInitialGBs.TryGetValue(x.CustId, out twGBCust);

                    if (twGBCust != null)
                    {
                        pa.Remark = $"Detection Date: { twGBCust.DetectedDate.ToString(Constants.CONVERT_TO_DATETIME_MMMddyyyy)} - GB Bet Count: {twGBCust.GBTicketCount} - Sport: { twGBCust.SportName }";
                    }
                    
                    return pa;
                });

                return JsonConvert.SerializeObject(dailyPas);
            }

            return string.Empty;
        }

        public IDailyProblemClassificationServices InsertPALogic()
        {
            if (problemAccounts != null & problemAccounts.Any())
            {
                var probAccountToInsertLogic = problemAccounts.Where(c => !c.IsRobot).Select(c => new CustInfoEntity { CTSCustID = c.CTSCustID, CustID = c.CustID });
                problemAccountServices.InsertPALogics(probAccountToInsertLogic, serviceName);

                var robotToInsertLogic = problemAccounts.Where(c => c.IsRobot).Select(c => new CustInfoEntity { CTSCustID = c.CTSCustID, CustID = c.CustID });
                problemAccountServices.InsertRobotLogics(robotToInsertLogic);
            }

            return this;
        }

        public bool PushCCToExternal(PushDataToExternalEntity paras)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var custIds = problemAccounts.Select(c => c.CustID).Distinct();

                return custClassToMainDBService.PushDataToExternal(custIds, paras);
            }

            return true;
        }

        public bool UpdateComplete()
        {
            string custIdToString = string.Empty;

            if (scanningCustIDs != null && scanningCustIDs.Any())
            {
                custIdToString = string.Join(",", scanningCustIDs.Distinct());
            }

            dailyClassificationDataAccess.CompletePADailyScan(custIdToString);

            return (scanningCustIDs != null && scanningCustIDs.Any());
        }
    }
}