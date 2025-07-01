namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAntiFraudCenterServices
    {
        void InsertAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow, bool isSyncAFCData, string url);

        void DeleteAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow, bool isSyncAFCData, string url);
    }

    public class AntiFraudCenterServices : IAntiFraudCenterServices
    {
        private const string ServiceName = "AntiFraudCenter";

        #region Public methods

        public async void InsertAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow, bool isSyncAFCData, string url)
        {
            try
            {
                if (custIds != null && custIds.Any() && isSyncAFCData)
                {
                    await CallAPI(custIds, fromFlow, url);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, $" {fromFlow} - CTS.CC Service - Insert Accounts fail", custIds, ex, "CustIds");
            }
        }

        public async void DeleteAFCAccounts(IEnumerable<long> custIds, FromFlow fromFlow, bool isSyncAFCData, string url)
        {
            try
            {
                if (custIds != null && custIds.Any() && isSyncAFCData)
                {
                    await CallAPI(custIds, fromFlow, url);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, $" {fromFlow} - CTS.CC Service - Delete Accounts fail", custIds, ex, "CustIds");
            }
        }

        #endregion Public methods

        #region Private methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private static async Task<bool> CallAPI(IEnumerable<long> custIds, FromFlow fromFlow, string url)
        {
            if (custIds != null && custIds.Any())
            {
                var para = new
                {
                    CustIds = custIds,
                    FromFlow = fromFlow
                };

                return await ApiHelper.Post<bool>(url, para, 300);
            }

            return true;
        }

        #endregion Private methods
    }
}