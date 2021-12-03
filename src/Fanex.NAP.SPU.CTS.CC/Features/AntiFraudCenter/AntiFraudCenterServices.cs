namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAntiFraudCenterServices
    {
        void InsertAFCAccounts(IEnumerable<long> custIds, string url);

        void DeleteAFCAccounts(IEnumerable<long> custIds, string url);
    }

    public class AntiFraudCenterServices : IAntiFraudCenterServices
    {
        private readonly IAntiFraudCenterDataAccess antiFraudCenterDataAccess;
        private readonly ISentryClient sentryClient;
        private const string ServiceName = "AntiFraudCenter";

        public AntiFraudCenterServices(
            IAntiFraudCenterDataAccess antiFraudCenterDataAccess,
            ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;
            this.antiFraudCenterDataAccess = antiFraudCenterDataAccess;
        }

        public void InsertAFCAccounts(IEnumerable<long> custIds, string url)
        {
            try
            {
                if (custIds != null && custIds.Any())
                {
                    var afcAccountInfo = GetAccountInfo(string.Join(",", custIds));

                    if (afcAccountInfo != null && afcAccountInfo.Any())
                    {
                        var inputData = new List<InsertAccountParasEntity>();

                        afcAccountInfo.ToList().ForEach(c =>
                        {
                            inputData.Add(new InsertAccountParasEntity
                            {
                                AccountID = c.CustId,
                                CountryCode = c.CountryCode,
                                Phone = c.PhoneNo,
                                Email = c.Email,
                                BankAccountID = c.AccountNum,
                                BankName = c.AccountName
                            });
                        });

                        var para = new InsertAccountParasViewEntity
                        {
                            Key = AFCInternalKey,
                            Data = inputData
                        };

                        var result = ApiHelper.Post<bool>(url, para);

                        Task.WaitAll(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, "Service - Insert Accounts fail", custIds, ex, sentryClient);
            }
        }

        public void DeleteAFCAccounts(IEnumerable<long> custIds, string url)
        {
            try
            {
                if (custIds != null && custIds.Any())
                {
                    var b2cCustIds = antiFraudCenterDataAccess.GetB2CCustIds(string.Join(",", custIds));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var inputData = new List<DeleteAccountParasEntity>();

                        b2cCustIds.ToList().ForEach(c =>
                        {
                            inputData.Add(new DeleteAccountParasEntity
                            {
                                AccountID = c.B2CCustId
                            });
                        });

                        var para = new DeleteAccountParasViewEntity
                        {
                            Key = AFCInternalKey,
                            Data = inputData
                        };

                        var result = ApiHelper.Post<bool>(url, para);

                        Task.WaitAll(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, "Service - Delete Accounts fail", custIds, ex, sentryClient);
            }
        }

        private IEnumerable<GetAFCAccountEntity> GetAccountInfo(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var filterB2CLicensee = antiFraudCenterDataAccess.FilterCustIdB2CLicensee(custIds);

                if (filterB2CLicensee != null && filterB2CLicensee.Any())
                {
                    var b2cCustIds = antiFraudCenterDataAccess.GetB2CCustIds(string.Join(",", filterB2CLicensee));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var b2cCustJson = JsonConvert.SerializeObject(b2cCustIds.Select(c => c.B2CCustId));

                        return antiFraudCenterDataAccess.GetAFCAccountInfo(b2cCustJson) ?? new List<GetAFCAccountEntity>();
                    }
                }
            }

            return new List<GetAFCAccountEntity>();
        }
    }
}