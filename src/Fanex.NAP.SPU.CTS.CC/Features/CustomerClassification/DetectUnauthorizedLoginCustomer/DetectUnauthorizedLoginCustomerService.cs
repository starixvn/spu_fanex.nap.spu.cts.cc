namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDetectUnauthorizedLoginCustomerService
    {
        int GetLastScannedCustID(CTS_SystemParameters paraID);

        DateTime GetLastScannedDate(CTS_SystemParameters paraID);

        DateTime GetProcessDate(CTS_SystemParameters lastScanDate);

        bool UpdateLastScannedID(CTS_SystemParameters paraID, string value);

        IEnumerable<CustFirstBetDateEntity> GetCustWithFirstBetDate(DetectUnauthorizedLoginCustParam param, DateTime processDate);

        IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson, DateTime processDate);

        bool InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras);
    }

    public class DetectUnauthorizedLoginCustomerService : ProblemAccountServices, IDetectUnauthorizedLoginCustomerService
    {
        private readonly IDetectUnauthorizedLoginCustomerDataAccess detectUnauthorizedDataAccess;
        private readonly ISystemParameterDataAccess systemDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly string serviceName;

        public DetectUnauthorizedLoginCustomerService(IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ISentryClient sentryClient,
            ICustClassToMainDBService custClassToMainDBService,
            IDetectUnauthorizedLoginCustomerDataAccess detectUnauthorizedDataAccess, 
            ISystemParameterDataAccess systemDataAccess) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.detectUnauthorizedDataAccess = detectUnauthorizedDataAccess;
            this.systemDataAccess = systemDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.serviceName = "Detect Unauthorized Login Customer";
        }
       
        public int GetLastScannedCustID(CTS_SystemParameters paraID)
        {
            var lastValue = systemDataAccess.GetPreviousTimingExecution(paraID);

            return lastValue != null ? int.Parse(lastValue?.ParameterValue) : 0;
        }

        public DateTime GetLastScannedDate(CTS_SystemParameters paraID)
        {
            var lastValue = systemDataAccess.GetPreviousTimingExecution(paraID);

            return (lastValue != null && !string.IsNullOrEmpty(lastValue.ParameterValue)) ? DateTime.Parse(lastValue?.ParameterValue) : DateTime.MinValue;
        }

        public bool UpdateLastScannedID(CTS_SystemParameters para, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                systemDataAccess.UpdateTimingExecution(value, para);
            }
            
            return true;
        }

        public DateTime GetProcessDate(CTS_SystemParameters lastScanDate)
        {
            var now = DateTime.Now;
            var lastScannedDate = this.GetLastScannedDate(lastScanDate);
            
            return lastScannedDate != DateTime.MinValue
                ? new DateTime(lastScannedDate.Year, lastScannedDate.Month, lastScannedDate.Day).AddDays(1)
                : new DateTime(now.Year, now.Month, now.Day);
        }

        public IEnumerable<CustFirstBetDateEntity> GetCustWithFirstBetDate(DetectUnauthorizedLoginCustParam param, DateTime processDate)
        {
            var lastScannedCustID = this.GetLastScannedCustID(param.LastScanCustID);
            
            var firstBetDateParam = new GetCustWithFirstBetDateParam
            {
                FromDate = processDate.AddDays(param.FirstBetFromDay),
                ToDate = processDate.AddDays(param.FirstBetToDay).AddDays(1).AddTicks(-20000),
                LastCustID = lastScannedCustID,
                IsLicensee = param.IsLicensee,
                BatchSize = 20000
            };

            return detectUnauthorizedDataAccess.GetCustWithFirstBetDate(firstBetDateParam);
        }

        public IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson, DateTime processDate)
        {
            return detectUnauthorizedDataAccess.DetectUnauthorizedLogin(custJson, processDate);
        }
     
        public bool InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras)
        {
            var paCustIDs = InsertProblemAccountPackage(pas, FunctionId.DetectUnauthorizedLoginCust, CCInputFlow.InsertPACategory, false, serviceName);

            custClassToMainDBService.PushExternalPackageByCustId(paCustIDs, paras);

            return true;
        }
    }
}
