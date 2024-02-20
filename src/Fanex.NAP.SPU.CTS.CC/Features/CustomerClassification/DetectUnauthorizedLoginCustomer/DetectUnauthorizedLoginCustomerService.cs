namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDetectUnauthorizedLoginCustomerService
    {
        int GetLastScannedCustID(CTS_DC_SystemParameters paraID);

        DateTime GetLastScannedDate(CTS_DC_SystemParameters paraID);

        DateTime GetProcessDate(CTS_DC_SystemParameters lastScanDate);

        bool UpdateLastScannedID(CTS_DC_SystemParameters paraID, string value);

        IEnumerable<CustFirstBetDateEntity> GetCustWithFirstBetDate(DetectUnauthorizedLoginCustParam param, DateTime processDate);

        IEnumerable<DetectUnauthorizedLoginCustEntity> DetectUnauthorizedLogin(string custJson, DateTime processDate);
    }

    public class DetectUnauthorizedLoginCustomerService : IDetectUnauthorizedLoginCustomerService
    {
        private readonly IDetectUnauthorizedLoginCustomerDataAccess detectUnauthorizedDataAccess;
        private readonly ISystemParameterDataAccess systemDataAccess;

        public DetectUnauthorizedLoginCustomerService(IDetectUnauthorizedLoginCustomerDataAccess detectUnauthorizedDataAccess, ISystemParameterDataAccess systemDataAccess)
        {
            this.detectUnauthorizedDataAccess = detectUnauthorizedDataAccess;
            this.systemDataAccess = systemDataAccess;
        }
       
        public int GetLastScannedCustID(CTS_DC_SystemParameters paraID)
        {
            var lastValue = systemDataAccess.GetPreviousTimingExecution(paraID);

            return lastValue != null ? int.Parse(lastValue?.ParameterValue) : 0;
        }

        public DateTime GetLastScannedDate(CTS_DC_SystemParameters paraID)
        {
            var lastValue = systemDataAccess.GetPreviousTimingExecution(paraID);

            return (lastValue != null && !string.IsNullOrEmpty(lastValue.ParameterValue)) ? DateTime.Parse(lastValue?.ParameterValue) : DateTime.MinValue;
        }

        public bool UpdateLastScannedID(CTS_DC_SystemParameters para, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                systemDataAccess.UpdateTimingExecution(value, para);
            }
            
            return true;
        }

        public DateTime GetProcessDate(CTS_DC_SystemParameters lastScanDate)
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
     

    }
}
