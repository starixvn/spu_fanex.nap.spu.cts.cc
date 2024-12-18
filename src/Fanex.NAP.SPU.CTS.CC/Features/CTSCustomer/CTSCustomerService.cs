namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerService
    {
        LastScannedCustModel GetLastInsertCust();

        InsertCustomerViewModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBModel> GetCustSubFromMainDBByLastCustSub(int numOfAccount, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBModel> GetCustSubFromMainDBByCustIDs(string custIds);

        bool InsertNewCustomer(bool isNewCustomer, InsertCustomerViewModel data, InsertNewCustRescanEntity paras);

        LastUpdateValueModel GetLastUpdatedTime();

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromProductStatusHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        bool IsValidData(TranformCustModel data);

        void UpdateCustomer(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog);

        IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);

        IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3874:\"out\" and \"ref\" parameters should not be used", Justification = "<Pending>")]
        string GetCustInfoToUpdate(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog, out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks);


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S103:Lines should not be too long", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3874:\"out\" and \"ref\" parameters should not be used", Justification = "<Pending>")]
        string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB, IEnumerable<CustomerMainDBModel> custInfoFromCTS, UpdateCustomer sourceType, out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks);

        #region Insert Missing Customers

        IEnumerable<MissingCustomerModel> GetMissingCustomers();

        public InsertCustomerViewModel GetMissingCustInfo(IEnumerable<long> CustIds);

        void CompleteMissingCustomer(string resolvedIDs);

        #endregion

        #region  Update UserName2

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3874:\"out\" and \"ref\" parameters should not be used", Justification = "<Pending>")]
        bool UpdateUserName2Package(ref UInt64 lastCTSCustId, bool isCustSub, int rowSize);

        ScannedCustModel GetCustWithNullUserName2(int rowSize, UInt64 lastCTSCustId, bool isCustSub);

        IEnumerable<CustNullUserName2Model> GetCustWithHavingUserName2(IEnumerable<CustNullUserName2Model> custs, bool isCustSub);

        void UpdateUsername2(IEnumerable<CustNullUserName2Model> customerList);

        #endregion

        IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime? lastScannedDate, DateTime? nextScannedDate);

        IEnumerable<long> CheckExistCTSArchiveCustomer(IEnumerable<ReactivatedCustModel> reactivedCusts);

        InactivateNewCustomer GetInactivateNewCustomers();

        void InsertReactivedCustomerLog(DateTime? lastScanTime, DateTime? requestTime, DateTime? maxLogTime, DateTime? custInfoTime);
    }

    public class CTSCustomerService : ICTSCustomerService
    {
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly string serviceName = "CTSCustomerService";

        public CTSCustomerService(ICTSCustomerDataAccess dataAccess,
            ISentryClient sentryClient,
            ICustClassToMainDBService custClassToMainDBService)
        {
            this.dataAccess = dataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
            this.sentryClient = sentryClient;
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            return dataAccess.GetLastUpdatedTime();
        }

        #region Insert New Customers

        public LastScannedCustModel GetLastInsertCust()
        {
            return dataAccess.GetLastInsertCust();
        }

        public InsertCustomerViewModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataAccess.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubFromMainDBByLastCustSub(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataAccess.GetCustSubFromMainDBByLastCustSub(numOfAccount, lastScannedCustID);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S4462:Calls to \"async\" methods should not be blocking", Justification = "<Pending>")]
        public bool InsertNewCustomer(bool isNewCustomer, InsertCustomerViewModel data, InsertNewCustRescanEntity paras)
        {
            if (data == null && data.CustomerInfos.IsNullOrEmpty())
            {
                return true;
            }

            var chunkCusts = data.CustomerInfos.Distinct().Chunk(paras.InternalBatchSize);

            foreach (var batchCust in chunkCusts)
            {
                var customers = dataAccess.InsertNewCTSCustomer(batchCust.ToList(), isNewCustomer);

                if (customers.IsNotNullOrEmpty())
                {
                    var isUpdateCompleteAndRetranCustomer =
                            PushExternalPackageByCustId(customers, RoleGroupType.Agency, paras)
                        .FlatMap(() =>
                            PushExternalPackageByCustId(customers, RoleGroupType.Member, paras))
                        .FlatMap(() =>
                            UpdateAssociationCustomerForArchive(customers))
                        .FlatMap(() =>
                            InsertVIP_BAClassificationAsync(batchCust, paras.InsertVIPURL, paras.InsertBAURL, paras.serviceName).Result)
                        .FlatMap(() =>
                            UpdateComleteAndRetranCustomer(customers, data.LastInsertCustID, paras.IsInsertMissingCust));

                    if (!isUpdateCompleteAndRetranCustomer)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool PushExternalPackageByCustId(IEnumerable<InsertNewCustomerResultModel> customers, RoleGroupType roleGroup, InsertNewCustRescanEntity paras)
        {
            var custIds = customers
                            .WhereIf(roleGroup == RoleGroupType.Agency, customer => customer.RoleID > (int)CustomerRole.Member)
                            .WhereIf(roleGroup == RoleGroupType.Member, customer => customer.RoleID == (int)CustomerRole.Member)
                            .Select(customer => customer.CustID)
                            .Distinct();

            var result = custClassToMainDBService.PushExternalPackageByCustId(
                            custIds,
                            new PushDataToExternalEntity
                            {
                                ExternalBatchSize = paras.ExternalBatchSize,
                                IsUpdateDangerToMainDB = false,
                                IsWriteLog = false,
                                ServiceName = serviceName,
                                RoleGroup = roleGroup
                            });

            return result;
        }

        private bool UpdateComleteAndRetranCustomer(IEnumerable<InsertNewCustomerResultModel> custIdList, long? lastCustID, bool isInsertMissingCust)
        {
            if (custIdList != null && custIdList.Any())
            {
                dataAccess.UpdateCompleteAndRetranCustomer(JsonConvert.SerializeObject(custIdList), lastCustID, isInsertMissingCust);
            }

            return true;
        }

        private bool UpdateAssociationCustomerForArchive(IEnumerable<InsertNewCustomerResultModel> customerList)
        {
            if (customerList != null && customerList.Any())
            {
                var custList = customerList.Select(x => new UpdateAssociationCustomerForArchiveModel
                {
                    CustID = x.CustID,
                    CTSCustID = x.CTSCustID,
                    CustSubID = x.CustSubID,
                    RoleID = x.RoleID
                });

                var custInfoJson = JsonConvert.SerializeObject(custList);

                dataAccess.UpdateAssociationCustomerForArchive(custInfoJson);
            }

            return true;
        }

        private async Task<bool> InsertVIP_BAClassificationAsync(IEnumerable<CustomerMainDBModel> customer, string insertVIPUrl, string insertBAUrl, string serviceName)
        {
            if (customer != null && customer.Any())
            {
                var custIdsToInsertVIP = customer.Where(c => c.IsLicenseeVIP).Select(c => c.CustID);
                var custIdsToInsertBA = customer.Where(c => c.IsLicenseeBA).Select(c => c.CustID);

                try
                {
                    if (custIdsToInsertVIP.IsNotNullOrEmpty())
                    {
                        await CallAPIInsertVIPBA(custIdsToInsertVIP, insertVIPUrl);
                    }

                    if (custIdsToInsertBA.IsNotNullOrEmpty())
                    {
                        await CallAPIInsertVIPBA(custIdsToInsertBA, insertBAUrl);
                    }
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, $"Insert Lic VIP/BA", new { custIdsToInsertVIP, custIdsToInsertBA }, ex, sentryClient);

                    return false;
                }
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private static async Task<string> CallAPIInsertVIPBA(IEnumerable<int> custIds, string url)
        {
            if (custIds != null && custIds.Any())
            {
                var para = new
                {
                    CustIDs = string.Join(",", custIds.Distinct()),
                    CreatedBy = StarixITId,
                    IsFromExternal = false
                };

                return await ApiHelper.Post<string>(url, para, 300);
            }

            return string.Empty;
        }

        #endregion Insert New Customers

        #region Insert Missing Customers

        public IEnumerable<MissingCustomerModel> GetMissingCustomers()
        {
            return dataAccess.GetMissingCustomers();
        }

        public void CompleteMissingCustomer(string resolvedIDs)
        {
            dataAccess.CompleteMissingCustomer(resolvedIDs);
        }

        public InsertCustomerViewModel GetMissingCustInfo(IEnumerable<long> CustIds)
        {
            var custInfo = new InsertCustomerViewModel { CustomerInfos = new List<CustomerMainDBModel>(), LastInsertCustID = null };

            if (CustIds != null && CustIds.Any())
            {
                var custToString = string.Join(",", CustIds.Select(c => c).Distinct());
                var mainCusts = GetCustomerFromMainDBForUpdate(custToString, Constants.UpdateCustomer.ForNewCustomer);
                var subCusts = GetCustSubFromMainDBByCustIDs(custToString);

                custInfo.CustomerInfos.AddRange(mainCusts);
                custInfo.CustomerInfos.AddRange(subCusts);
            }

            return custInfo;
        }

        #endregion

        #region Update Customers Info

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public void UpdateCustomer(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog)
        {
            try
            {
                if (custs != null && custs.LastCustId != null && custs.LastUpdatedTime != null)
                {
                    var assCustForRollbacks = Enumerable.Empty<UpdateAssociationCustomerForRollbackModel>();

                    var custInfoToUpdate = GetCustInfoToUpdate(custs, sourceType, isWriteLog, out assCustForRollbacks);

                    UpdateAssociationCustomerForRollback(assCustForRollbacks);

                    UpdateCustomerComplete(custInfoToUpdate, custs.LastUpdatedTime, custs.LastCustId, sourceType);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "UpdateCustomer has an error", custs, ex, sentryClient);
            }
        }

        public string GetCustInfoToUpdate(TranformCustModel custs
            , UpdateCustomer sourceType
            , bool isWriteLog
            , out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            var custInfoToUpdate = JsonConvert.SerializeObject(string.Empty);
            assCustForRollbacks = new List<UpdateAssociationCustomerForRollbackModel>();

            if (!string.IsNullOrWhiteSpace(custs.CustIdList))
            {
                var custInfoFromMainDB = GetCustomerFromMainDBForUpdate(custs.CustIdList, sourceType);
                var custInfoFromCTS = GetCustomerFromCTSForUpdate(custs.CustIdList, sourceType);

                Utilities.LogSentryInfo(serviceName, $"Healing {sourceType} - Get info from MainDB", custInfoFromMainDB, isWriteLog, sentryClient);
                Utilities.LogSentryInfo(serviceName, $"Healing {sourceType} - Get info from CTS", custInfoFromCTS, isWriteLog, sentryClient);

                if (custInfoFromMainDB != null && custInfoFromCTS != null)
                {
                    custInfoToUpdate = CompareCustomerInfo(custInfoFromMainDB, custInfoFromCTS, sourceType, out assCustForRollbacks);
                }
            }

            Utilities.LogSentryInfo(serviceName, $"Healing {sourceType} - Cust to update data:", custInfoToUpdate, isWriteLog, sentryClient);

            return custInfoToUpdate;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3257:Declarations and initializations should be as concise as possible", Justification = "<Pending>")]
        public string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB
            , IEnumerable<CustomerMainDBModel> custInfoFromCTS
            , UpdateCustomer sourceType
            , out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            assCustForRollbacks = new List<UpdateAssociationCustomerForRollbackModel>();

            switch (sourceType)
            {
                case Constants.UpdateCustomer.FromCustInfo:
                    {
                        var customerInfo = custInfoFromMainDB
                            .Join(custInfoFromCTS, custMain => custMain.CustID,
                                                   custCTS => custCTS.CustID,
                                                   (custMain, custCTS) => custMain)
                            .Where(c => !custInfoFromCTS.Contains(c, new CustomerInfoEqualityComparer()))
                            .Select(c => new CustInfoModel
                            {
                                CustID = c.CustID,
                                CustStatusID = c.CustStatusID,
                                Danger1 = c.Danger1,
                                Danger2 = c.Danger2,
                                Danger3 = c.Danger3,
                                Danger4 = c.Danger4,
                                Danger5 = c.Danger5,
                                DangerSabaSc = c.DangerSabaSc,
                                DangerSabaBkb = c.DangerSabaBkb
                            }).ToList();

                        assCustForRollbacks = customerInfo.Select(x => new UpdateAssociationCustomerForRollbackModel
                        {
                            CustID = x.CustID,
                            CustStatusID = x.CustStatusID
                        });

                        return JsonConvert.SerializeObject(customerInfo);
                    }
                case Constants.UpdateCustomer.FromCustomer:
                    {
                        var customerInfo = custInfoFromMainDB
                            .Join(custInfoFromCTS, custMain => custMain.CustID,
                                                   custCTS => custCTS.CustID,
                                                   (custMain, custCTS) => custMain)
                            .Where(c => !custInfoFromCTS.Contains(c, new CustomerEqualityComparer()))
                            .Select(c => new CustomerInfoModel
                            {
                                CustID = c.CustID,
                                UserName = c.UserName,
                                UserName2 = c.UserName2,
                                SiteID = c.SiteID,
                                Site = c.Site,
                                RoleID = c.RoleID,
                                SRecommend = c.SRecommend,
                                MRecommend = c.MRecommend,
                                Recommend = c.Recommend,
                                Currency = c.Currency,
                                CurrencyID = c.CurrencyID,
                                UpdateType = (custInfoFromCTS.Any(f => f.CustID == c.CustID &&
                                                         (!string.Equals(f.UserName, c.UserName)
                                                         || !string.Equals(f.UserName2, c.UserName2)
                                                         || f.SiteID != c.SiteID))) ? 1 : 0
                            }).ToList();

                        return JsonConvert.SerializeObject(customerInfo);
                    }
                case Constants.UpdateCustomer.FromProductStatus:
                    {
                        var customerInfo = custInfoFromMainDB
                            .Join(custInfoFromCTS, custMain => custMain.CustID,
                                                   custCTS => custCTS.CustID,
                                                   (custMain, custCTS) => custMain)
                            .Where(c => !custInfoFromCTS.Contains(c, new CustomerStaticEqualityComparer()))
                            .Select(c => new CustInfoModel
                            {
                                CustID = c.CustID,
                                CustStatusID = c.CustStatusID
                            }).ToList();

                        assCustForRollbacks = customerInfo.Select(x => new UpdateAssociationCustomerForRollbackModel
                        {
                            CustID = x.CustID,
                            CustStatusID = x.CustStatusID
                        });

                        return JsonConvert.SerializeObject(customerInfo);
                    }
                default:
                    return JsonConvert.SerializeObject(string.Empty);
            }
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType)
        {
            return dataAccess.GetCustomerFromMainDBForUpdate(custIds, sourceType) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType)
        {
            return dataAccess.GetCustomerFromCTSForUpdate(custIds, sourceType);
        }

        public TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            return dataAccess.GetUpdatedCustFromCustInfoHistory(lastUpdatedTime, lastUpdatedCustId, numOfAccount);
        }

        public TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            return dataAccess.GetUpdatedCustFromCustomerHistory(lastUpdatedTime, lastUpdatedCustId, numOfAccount);
        }

        public TranformCustModel GetUpdatedCustFromProductStatusHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            return dataAccess.GetUpdatedCustFromProductStatus(lastUpdatedTime, lastUpdatedCustId, numOfAccount);
        }

        private void UpdateAssociationCustomerForRollback(IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                dataAccess.UpdateAssociationCustomerForRollback(custInfoJson);
            }
        }

        public void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID, UpdateCustomer sourceType)
        {
            switch (sourceType)
            {
                case Constants.UpdateCustomer.FromCustInfo:
                    {
                        dataAccess.UpdateCustInfoComplete(customers, lastUpdateTime, lastUpdateCustID);
                        break;
                    }
                case Constants.UpdateCustomer.FromCustomer:
                    {
                        dataAccess.UpdateCustomerComplete(customers, lastUpdateTime, lastUpdateCustID);
                        break;
                    }
                case Constants.UpdateCustomer.FromProductStatus:
                    {
                        dataAccess.UpdateCustomerStatusComplete(customers, lastUpdateTime, lastUpdateCustID);
                        break;
                    }
                default:
                    return;
            }
        }

        public bool IsValidData(TranformCustModel data)
        {
            if (data != null && data.LastCustId != null && data.LastUpdatedTime != null)
            {
                return true;
            }

            return false;
        }

        #endregion Update Customers Info

        #region Update UserName2

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S126:\"if ... else if\" constructs should end with \"else\" clauses", Justification = "<Pending>")]
        public bool UpdateUserName2Package(ref UInt64 lastCTSCustId, bool isCustSub, int rowSize)
        {
            var scannedCusts = GetCustWithNullUserName2(rowSize, lastCTSCustId, isCustSub);
            var isContinue = false;

            if (scannedCusts.CustList.Any())
            {
                var batchCust = scannedCusts.CustList.Distinct().Chunk(rowSize);

                foreach (var custs in batchCust)
                {
                    var custFromMainDB = GetCustWithHavingUserName2(custs, isCustSub);

                    UpdateUsername2(custFromMainDB);
                }

                isContinue = true;
            }

            if (!isCustSub)
            {
                lastCTSCustId = scannedCusts.LastCTSCustID > 0 ? scannedCusts.LastCTSCustID : lastCTSCustId;
            }
            else if (scannedCusts.CustList.Any())
            {
                lastCTSCustId = scannedCusts.CustList.Max(c => c.CTSCustID);
            }

            return isContinue;
        }

        public ScannedCustModel GetCustWithNullUserName2(int rowSize, UInt64 lastCTSCustId, bool isCustSub)
        {
            return dataAccess.GetCustWithNullUserName2(rowSize, lastCTSCustId, isCustSub);
        }

        public IEnumerable<CustNullUserName2Model> GetCustWithHavingUserName2(IEnumerable<CustNullUserName2Model> custs, bool isCustSub)
        {
            return dataAccess.GetCustWithHavingUserName2(custs, isCustSub);
        }

        public void UpdateUsername2(IEnumerable<CustNullUserName2Model> customerList)
        {
            if (customerList != null && customerList.Any())
            {
                dataAccess.UpdateUsername2(customerList);
            }
        }

        #endregion Update UserName2

        #region Insert Reactivated Customers

        public IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime? lastScannedDate, DateTime? nextScannedDate)
        {
            if (lastScannedDate != null)
            {
                return dataAccess.GetReactivatedCustomers(lastScannedDate.Value, nextScannedDate);
            }

            return Enumerable.Empty<ReactivatedCustModel>();
        }

        public IEnumerable<long> CheckExistCTSArchiveCustomer(IEnumerable<ReactivatedCustModel> reactivedCusts)
        {
            if (reactivedCusts != null && reactivedCusts.Any())
            {
                return dataAccess.CheckExistCTSArchiveCustomer(JsonConvert.SerializeObject(reactivedCusts));
            }

            return Enumerable.Empty<long>();
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubFromMainDBByCustIDs(string custIds)
        {
            return dataAccess.GetCustSubFromMainDBByCustIDs(custIds) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        public void InsertReactivedCustomerLog(DateTime? lastScanTime, DateTime? requestTime, DateTime? maxLogTime, DateTime? custInfoTime)
        {
            dataAccess.InsertReactivedCustomerLog(lastScanTime, requestTime, maxLogTime, custInfoTime);
        }

        #endregion Insert Reactivated Customers

        #region Inactivate New Customers

        public InactivateNewCustomer GetInactivateNewCustomers()
        {
            return dataAccess.GetInactivateNewCustomers();
        }

        #endregion Inactivate New Customers
    }
}