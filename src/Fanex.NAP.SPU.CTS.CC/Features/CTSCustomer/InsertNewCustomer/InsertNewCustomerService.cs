namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Sentry;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;

    public interface IInsertNewCustomerService
    {
        LastScannedCustModel GetLastInsertCust();

        InsertCustomerModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByLastCustSub(int numOfAccount, Int64 lastScannedCustID);

        bool InsertNewCustomer(bool isNewCustomer, InsertCustomerModel data, InsertNewCustRescanEntity paras);
    }

    public class InsertNewCustomerService : IInsertNewCustomerService
    {
        private readonly IInsertNewCustomerDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly ISentryClient _sentryClient;

        public InsertNewCustomerService(IInsertNewCustomerDataAccess dataAccess, ICustClassToMainDBService custClassToMainDBService, ISentryClient sentryClient)
        {
            _dataAccess = dataAccess;
            _custClassToMainDBService = custClassToMainDBService;
            _sentryClient = sentryClient;
        }

        public LastScannedCustModel GetLastInsertCust()
        {
            return _dataAccess.GetLastInsertCust();
        }

        public InsertCustomerModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID)
        {
            return _dataAccess.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);
        }

        public IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByLastCustSub(int numOfAccount, Int64 lastScannedCustID)
        {
            return _dataAccess.GetCustSubFromMainDBByLastCustSub(numOfAccount, lastScannedCustID);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S4462:Calls to \"async\" methods should not be blocking", Justification = "<Pending>")]
        public bool InsertNewCustomer(bool isNewCustomer, InsertCustomerModel data, InsertNewCustRescanEntity paras)
        {
            if (data == null && data.CustomerInfos.IsNullOrEmpty())
            {
                return true;
            }

            var chunkCusts = data.CustomerInfos.Distinct().Chunk(paras.InternalBatchSize);

            foreach (var batchCust in chunkCusts)
            {
                var customers = _dataAccess.InsertNewCTSCustomer(batchCust.ToList(), isNewCustomer);

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

            var result = _custClassToMainDBService.PushExternalPackageByCustId(
                            custIds,
                            new PushDataToExternalEntity
                            {
                                ExternalBatchSize = paras.ExternalBatchSize,
                                IsUpdateDangerToMainDB = false,
                                IsWriteLog = false,
                                ServiceName = paras.serviceName,
                                RoleGroup = roleGroup
                            });

            return result;
        }

        private bool UpdateComleteAndRetranCustomer(IEnumerable<InsertNewCustomerResultModel> custIdList, long? lastCustID, bool isInsertMissingCust)
        {
            if (custIdList != null && custIdList.Any())
            {
                _dataAccess.UpdateCompleteAndRetranCustomer(JsonConvert.SerializeObject(custIdList), lastCustID, isInsertMissingCust);
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

                _dataAccess.UpdateAssociationCustomerForArchive(custInfoJson);
            }

            return true;
        }

        private async Task<bool> InsertVIP_BAClassificationAsync(IEnumerable<CustomerMainDBEntity> customer, string insertVIPUrl, string insertBAUrl, string serviceName)
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
                    Utilities.LogSentryError(serviceName, $"Insert Lic VIP/BA", new { custIdsToInsertVIP, custIdsToInsertBA }, ex, _sentryClient);

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
    }
}
