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
        Task InsertAFCAccountsAsync(IEnumerable<long> custIds, string insertURL, string updateURL);

        Task DeleteAFCAccountsAsync(IEnumerable<long> custIds, string url);
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

        public async Task InsertAFCAccountsAsync(IEnumerable<long> custIds, string insertURL, string updateURL)
        {
            try
            {
                if (custIds != null && custIds.Any())
                {
                    var accountInfo = GetAccountInfo(string.Join(",", custIds));

                    if (accountInfo != null && accountInfo.Any())
                    {
                        var notExistedAccount = CheckAccountExist(accountInfo);

                        await SyncAccountToAFCDBAsync(accountInfo, notExistedAccount, insertURL, updateURL);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, "Service - Insert Accounts fail", custIds, ex, sentryClient);
            }
        }

        public async Task DeleteAFCAccountsAsync(IEnumerable<long> custIds, string url)
        {
            try
            {
                if (custIds != null && custIds.Any())
                {
                    var b2cCustIds = antiFraudCenterDataAccess.GetB2CCustIds(string.Join(",", custIds));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var para = new DeleteAccountParasViewEntity
                        {
                            Key = AFCInternalKey,
                            Data = b2cCustIds
                        };

                        await ApiHelper.Post<bool>(url, para);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, "Service - Delete Accounts fail", custIds, ex, sentryClient);
            }
        }

        private IEnumerable<GetAccountViewEntity> GetAccountInfo(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var filterB2CLicensee = antiFraudCenterDataAccess.FilterLicenseeCust(custIds);

                if (filterB2CLicensee != null && filterB2CLicensee.Any())
                {
                    var b2cCustIds = antiFraudCenterDataAccess.GetB2CCustIds(string.Join(",", filterB2CLicensee));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var b2cCustJson = JsonConvert.SerializeObject(b2cCustIds.Select(c => c.B2CCustId));

                        var accountInfo = antiFraudCenterDataAccess.GetAccountInfo(b2cCustJson);

                        return MappingFrauIDs(accountInfo, filterB2CLicensee, b2cCustIds);
                    }
                }
            }

            return new List<GetAccountViewEntity>();
        }

        private IEnumerable<GetAccountViewEntity> MappingFrauIDs(IEnumerable<GetAFCAccountModel> accountInfo
                                                      , IEnumerable<FilterCustToSyncAFCKEntity> fraudIDs
                                                      , IEnumerable<GetB2CEntity> b2cCustIDs)
        {
            if (accountInfo != null && fraudIDs != null && b2cCustIDs != null &&
                accountInfo.Any() && fraudIDs.Any() && b2cCustIDs.Any())
            {
                var accInfo = accountInfo.GroupJoin(b2cCustIDs,
                                   a => a.CustId,
                                   b => b.B2CCustId,
                                   (a, b) => new { a, b })
                               .SelectMany(
                                    group => group.b.DefaultIfEmpty(),
                                    (group, temp) => new GetAccountViewEntity
                                    {
                                        CustId = temp?.CustId,
                                        B2CustId = group.a.CustId,
                                        CountryCode = group.a.CountryCode,
                                        PhoneNo = group.a.PhoneNo,
                                        AccountName = group.a.AccountName,
                                        AccountNum = group.a.AccountNum,
                                        Email = group.a.Email
                                    })
                                .GroupJoin(fraudIDs,
                                    a => a.CustId,
                                    f => f.CustID,
                                    (a, f) => new { a, f })
                                .SelectMany(
                                   group => group.f.DefaultIfEmpty(),
                                   (group, temp) => new GetAccountViewEntity
                                   {
                                       CustId = group.a.CustId,
                                       B2CustId = group.a.B2CustId,
                                       CountryCode = group.a.CountryCode,
                                       PhoneNo = group.a.PhoneNo,
                                       AccountName = group.a.AccountName,
                                       AccountNum = group.a.AccountNum,
                                       Email = group.a.Email,
                                       FraudID = temp?.FraudIDList
                                   });

                return accInfo ?? new List<GetAccountViewEntity>();
            }

            return new List<GetAccountViewEntity>();
        }

        private IEnumerable<string> CheckAccountExist(IEnumerable<GetAccountViewEntity> accInfo)
        {
            if (accInfo != null && accInfo.Any())
            {
                var participants = antiFraudCenterDataAccess.GetParticipant();

                if (participants != null && participants.Any())
                {
                    var internalInfo = participants.SingleOrDefault(c => string.Equals(c.SecretKey, AFCInternalKey));

                    var result = antiFraudCenterDataAccess.CheckAccountValidation(internalInfo.ParticipantID, accInfo.Select(c => new CheckAccountExistEntity { AccountID = c.B2CustId }));

                    if (result != null && result.Any())
                    {
                        var notExistedAcc = result.Where(c => string.Equals(c.Status, "Not Exist")).Select(c => c.AccountID);

                        return notExistedAcc ?? new List<string>();
                    }
                }
            }

            return new List<string>();
        }


        /// <summary>
        /// Insert or Update AFC account
        /// </summary>
        /// <param name="accInfo"></param>
        /// <param name="isInsertAction"></param>
        private async Task CallAPIAsync(IEnumerable<GetAccountViewEntity> accInfo, string url)
        {
            if (accInfo != null && accInfo.Any())
            {
                var para = new InsertAccountParasViewEntity
                {
                    Key = AFCInternalKey,
                    Data = accInfo
                };

                await ApiHelper.Post<bool>(url, para);
            }
        }

        private async Task SyncAccountToAFCDBAsync(IEnumerable<GetAccountViewEntity> accountInfo, IEnumerable<string> notExistedAccount, string insertURL, string updateURL)
        {
            if(accountInfo != null && accountInfo.Any())
            {
                var tasks = new List<Task>();

                if (notExistedAccount.Any())
                {
                    var accToInsert = accountInfo.Where(c => notExistedAccount.Contains(c.B2CustId));

                    accountInfo.ToList().RemoveAll(x => accToInsert.Contains(x));

                    tasks.Add(Task.Run( async () =>
                    {
                        await CallAPIAsync(accToInsert, insertURL);
                    }));
                }

                if (accountInfo.Any())
                {
                    tasks.Add(Task.Run(async  () =>
                    {
                        await CallAPIAsync(accountInfo, updateURL);
                    }));
                }

                await Task.WhenAll(tasks.ToArray());
            } 
        }
    }
}