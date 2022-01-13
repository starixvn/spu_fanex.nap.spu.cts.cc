namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAntiFraudCenterServices
    {
        void InsertAFCAccounts(IEnumerable<long> custIds, string insertURL, string updateURL, bool isSyncAFCData, string internalKey);

        void DeleteAFCAccounts(IEnumerable<long> custIds, string url, bool isSyncAFCData, string internalKey);
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

        #region Public methods

        public void InsertAFCAccounts(IEnumerable<long> custIds, string insertURL, string updateURL, bool isSyncAFCData, string internalKey)
        {
            try
            {
                if (custIds != null && custIds.Any() && isSyncAFCData)
                {
                    var accountInfo = GetAccountForInsert(string.Join(",", custIds));

                    if (accountInfo != null && accountInfo.Any())
                    {
                        var notExistedAccount = CheckAccountExist(accountInfo, internalKey);

                        SyncAccountToAFCDBAsync(accountInfo, notExistedAccount, insertURL, updateURL, internalKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, "CTS.CC Service - Insert Accounts fail", custIds, ex, sentryClient);

                antiFraudCenterDataAccess.InsertErrorLog(null, ex.Message, JsonConvert.SerializeObject(custIds));
            }
        }

        public async void DeleteAFCAccounts(IEnumerable<long> custIds, string url, bool isSyncAFCData, string internalKey)
        {
            try
            {
                if (custIds != null && custIds.Any() && isSyncAFCData)
                {
                    var b2cCustIds = GetAccountForDelete(string.Join(",", custIds));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var para = new DeleteAccountParasViewEntity
                        {
                            Key = internalKey,
                            Data = b2cCustIds
                        };

                        var result = await ApiHelper.Post<AccountDBResult>(url, para);

                        if (result.StatusCode != (int)HttpStatus.OK && IsLogError(result.ErrorMessage))
                        {
                            Utilities.LogSentryError(ServiceName, $"CTS.CC Service - Delete Accounts fail", custIds, new Exception(result.ErrorMessage), sentryClient);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, "CTS.CC Service - Delete Accounts fail", custIds, ex, sentryClient);

                antiFraudCenterDataAccess.InsertErrorLog(null, ex.Message, JsonConvert.SerializeObject(custIds));
            }
        }

        #endregion Public methods

        #region Private methods

        private IEnumerable<GetB2CEntity> GetAccountForDelete(string custIds)
        {
            var filterB2CLicensee = antiFraudCenterDataAccess.FilterLicenseeCust(custIds);

            if (filterB2CLicensee != null && filterB2CLicensee.Any())
            {
                var custIdsToDelete = filterB2CLicensee.Where(c => c.FraudIDList == null || !c.FraudIDList.Any())?.Select(c => c.CustID)?.Distinct();

                if (custIdsToDelete != null && custIdsToDelete.Any())
                {
                    return antiFraudCenterDataAccess.GetB2CCustIds(string.Join(",", custIdsToDelete));
                }
            }

            return Enumerable.Empty<GetB2CEntity>();
        }

        private IEnumerable<GetAccountViewEntity> GetAccountForInsert(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var filterB2CLicensee = antiFraudCenterDataAccess.FilterLicenseeCust(custIds);

                if (filterB2CLicensee != null && filterB2CLicensee.Any())
                {
                    var probAccouts = filterB2CLicensee.Where(c => c.FraudIDList != null && c.FraudIDList.Any()).Select(c => c.CustID).Distinct();

                    var b2cCustIds = antiFraudCenterDataAccess.GetB2CCustIds(string.Join(",", probAccouts));

                    if (b2cCustIds != null && b2cCustIds.Any())
                    {
                        var b2cCustJson = JsonConvert.SerializeObject(b2cCustIds.Select(c => c.B2CCustId));

                        var accountInfo = antiFraudCenterDataAccess.GetB2CInfo(b2cCustJson);

                        return MappingFrauIDs(accountInfo, filterB2CLicensee, b2cCustIds);
                    }
                }
            }

            return Enumerable.Empty<GetAccountViewEntity>();
        }

        private static IEnumerable<GetAccountViewEntity> MappingFrauIDs(IEnumerable<GetAccountEntity> accountInfo, IEnumerable<FilterCustToSyncAFCKEntity> fraudIDs, IEnumerable<GetB2CEntity> b2cCustIDs)
        {
            if (IsValid(accountInfo, fraudIDs, b2cCustIDs))
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
                                        Email = group.a.Email,
                                        FullName = group.a.FullName
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
                                       CountryCode = new RegionInfo(group.a.CountryCode)?.ThreeLetterISORegionName ?? string.Empty,
                                       PhoneNo = group.a.PhoneNo,
                                       AccountName = group.a.AccountName,
                                       AccountNum = group.a.AccountNum,
                                       Email = group.a.Email,
                                       FullName = group.a.FullName,
                                       FraudID = temp?.FraudIDList
                                   });

                return accInfo ?? Enumerable.Empty<GetAccountViewEntity>();
            }

            return Enumerable.Empty<GetAccountViewEntity>();
        }

        private IEnumerable<string> CheckAccountExist(IEnumerable<GetAccountViewEntity> accInfo, string internalKey)
        {
            if (accInfo != null && accInfo.Any())
            {
                var participants = antiFraudCenterDataAccess.GetParticipant();

                if (participants != null && participants.Any())
                {
                    var internalInfo = participants.SingleOrDefault(c => string.Equals(c.SecretKey, internalKey));

                    if (internalInfo == null)
                    {
                        throw new Exception("AFC - Internal Key is Invalid");
                    }

                    var result = antiFraudCenterDataAccess.CheckAccountValidation(internalInfo.ParticipantID, accInfo.Select(c => new CheckAccountExistEntity { AccountID = c.B2CustId }));

                    if (result != null && result.Any())
                    {
                        var notExistedAcc = result.Where(c => string.Equals(c.Status, "Not Exist")).Select(c => c.AccountID);

                        return notExistedAcc ?? Enumerable.Empty<string>();
                    }
                }
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Insert or Update AFC account
        /// </summary>
        /// <param name="accInfo"></param>
        /// <param name="isInsertAction"></param>
        private async Task<AccountDBResult> SyncAFCData(IEnumerable<GetAccountViewEntity> accInfo, string url, string internalKey)
        {
            if (accInfo != null && accInfo.Any())
            {
                var para = new InsertAccountParasViewEntity
                {
                    Key = internalKey,
                    Data = accInfo
                };

                return await ApiHelper.Post<AccountDBResult>(url, para);
            }

            return new AccountDBResult { StatusCode = (int)HttpStatus.OK, ErrorMessage = string.Empty };
        }

        private void SyncAccountToAFCDBAsync(IEnumerable<GetAccountViewEntity> accountInfo, IEnumerable<string> notExistedAccount, string insertURL, string updateURL, string internalKey)
        {
            if (accountInfo == null || !accountInfo.Any())
            {
                return;
            }    

            var accToInsert = notExistedAccount.Any() ? accountInfo.Where(c => notExistedAccount.Contains(c.B2CustId)) : null;
            var accToUpdate = notExistedAccount.Any() ? accountInfo.Where(c => !notExistedAccount.Contains(c.B2CustId)) : accountInfo;

            Parallel.Invoke(async () =>
            {
                try
                {
                    var result = await SyncAFCData(accToInsert, insertURL, internalKey);

                    if (result.StatusCode == (int)HttpStatus.OK || result.StatusCode == (int)HttpStatus.Created)
                    {
                        return;
                    }

                    Utilities.LogSentryError(ServiceName, $"CTS.CC Service - Insert Accounts fail", accToInsert, new Exception(result.ErrorMessage), sentryClient);
                    antiFraudCenterDataAccess.InsertErrorLog(null, result.ErrorMessage, JsonConvert.SerializeObject(accToInsert));
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(ServiceName, "CTS.CC Service - Insert Accounts fail", accToInsert, ex, sentryClient);
                    antiFraudCenterDataAccess.InsertErrorLog(null, ex.Message, JsonConvert.SerializeObject(accToInsert));
                }
            },
            async () =>
            {
                try
                {
                    var result = await SyncAFCData(accToUpdate, updateURL, internalKey);

                    if (result.StatusCode != (int)HttpStatus.OK)
                    {
                        Utilities.LogSentryError(ServiceName, $"CTS.CC Service - Update Accounts fail", accToUpdate, new Exception(result.ErrorMessage), sentryClient);
                        antiFraudCenterDataAccess.InsertErrorLog(null, result.ErrorMessage, JsonConvert.SerializeObject(accToUpdate));
                    }
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(ServiceName, "CTS.CC Service - Update Accounts fail", accToUpdate, ex, sentryClient);
                    antiFraudCenterDataAccess.InsertErrorLog(null, ex.Message, JsonConvert.SerializeObject(accToUpdate));
                }
            });
        }

        private static bool IsValid(IEnumerable<GetAccountEntity> accountInfo, IEnumerable<FilterCustToSyncAFCKEntity> fraudIDs, IEnumerable<GetB2CEntity> b2cCustIDs)
        {
            if (accountInfo != null && fraudIDs != null && b2cCustIDs != null)
            {
                return accountInfo.Any() && fraudIDs.Any() && b2cCustIDs.Any();
            }

            return false;
        }

        private static bool IsLogError(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return !errorMessage.Contains("Not Exist");
            }

            return true;
        }

        #endregion Private methods
    }
}