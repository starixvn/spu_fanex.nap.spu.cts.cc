namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerEvidenceInsertServices
    {
        IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile();

        bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, out IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList);

        bool InsertNewCustomerEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isOverwrite, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, EvidenceActionType actionType
              , IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, EvidenceActionType actionType, bool isPrevSuccess);

        ICustomerEvidenceInsertServices UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isProcessed);

        ICustomerEvidenceInsertServices UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences);

        List<CustomerEvidenceFailedToProcessEntity> GetLogError();

        IEnumerable<IEnumerable<CustomerEvidenceFromFileEntity>> ChunkSize(List<CustomerEvidenceFromFileEntity> custEvidenceInFiles, int internalBatchSize);
    }

    public class CustomerEvidenceInsertServices : ICustomerEvidenceInsertServices
    {
        private readonly ICustomerEvidenceInsertDataAccess customerEvidenceInsertDataAccess;
        private readonly List<CustomerEvidenceFailedToProcessEntity> failedToProcessList;

        public CustomerEvidenceInsertServices(ICustomerEvidenceInsertDataAccess customerEvidenceInsertDataAccess)
        {
            this.customerEvidenceInsertDataAccess = customerEvidenceInsertDataAccess;
            this.failedToProcessList = new List<CustomerEvidenceFailedToProcessEntity>();
        }

        public IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile()
        {
            return customerEvidenceInsertDataAccess.GetCustomerEvidenceFromFile();
        }

        public bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, out IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList)
        {
            custEviRemovedList = new List<CustomerEvidenceRemovedEntity>();

            var ctsCustIdList = custEvidences.Select(x => new { x.CTSCustID }).Distinct();

            var jsonData = JsonConvert.SerializeObject(ctsCustIdList);

            var result = customerEvidenceInsertDataAccess.RemoveCustomerEvidenceByCTSCustIds(jsonData, false, false);

            if (result.CustEviRemovedList != null)
            {
                custEviRemovedList = result.CustEviRemovedList.ToList();
            }

            if (result.DBResult.ErrorCode != 0)
            {
                WriteLogError("RemoveOldEvidence", result.DBResult.ErrorMsg, custEvidences);

                return false;
            }

            return true;
        }

        public bool InsertNewCustomerEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isOverwrite, bool isPrevSuccess)
        {
            if (isPrevSuccess)
            {
                var count = 0;
                var isSuccess = true;
                var isDeadlock = false;
                var errorMsg = string.Empty;

                var objects = custEvidences.Select(x => new
                {
                    x.CTSCustID,
                    x.EvidenceID,
                    x.Remark,
                    x.CreatedBy
                });

                var jsonData = JsonConvert.SerializeObject(objects);

                do
                {
                    var dbResult = customerEvidenceInsertDataAccess.InsertCustomerEvidenceMuitiple(jsonData, isOverwrite);

                    isSuccess = dbResult.ErrorCode == 0;
                    errorMsg = dbResult.ErrorMsg;
                    isDeadlock = errorMsg != null && errorMsg.ToLower().Contains("deadlock");

                    if (isDeadlock)
                    {
                        Thread.Sleep(ImportEvidence_SleepTimes);
                    }

                    count++;
                } while (count < InternalRetry && isDeadlock);

                if (!isSuccess)
                {
                    WriteLogError("InsertNewCustEvidence", errorMsg, custEvidences);

                    return false;
                }

                return true;
            }

            return isPrevSuccess;
        }

        public bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, EvidenceActionType actionType,
            IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList, bool isPrevSuccess)
        {
            if (isPrevSuccess)
            {
                var objects = custEvidences.Select(x => new
                {
                    CustId = x.CustID,
                    CTSCustId = x.CTSCustID,
                    SubscriberId = x.SubscriberID,
                    EvidenceId = x.EvidenceID,
                    ActionType = (int)actionType
                }).ToList();

                if (custEviRemovedList != null && custEviRemovedList.Any())
                {
                    var removeds = custEviRemovedList.Select(x => new
                    {
                        x.CustId,
                        x.CTSCustId,
                        x.SubscriberId,
                        x.EvidenceId,
                        x.ActionType
                    });

                    objects.AddRange(removeds);
                }

                var jsonData = JsonConvert.SerializeObject(objects);

                var dbResult = customerEvidenceInsertDataAccess.SyncCustomerEvidenceToCategory(jsonData, false);

                if (dbResult.ErrorCode != 0)
                {
                    WriteLogError("InsertNewCustEvidence", dbResult.ErrorMsg, custEvidences);

                    return false;
                }

                return true;
            }

            return isPrevSuccess;
        }

        public bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, EvidenceActionType actionType, bool isPrevSuccess)
        {
            if (isPrevSuccess)
            {
                var objects = custEvidences.Select(x => new
                {
                    CustId = x.CustID,
                    CTSCustId = x.CTSCustID,
                    SubscriberId = x.SubscriberID,
                    EvidenceId = x.EvidenceID,
                    ActionType = (int)actionType
                });

                var jsonData = JsonConvert.SerializeObject(objects);

                var dbResult = customerEvidenceInsertDataAccess.SyncCustomerEvidenceToCategory(jsonData, false);

                if (dbResult.ErrorCode != 0)
                {
                    WriteLogError("InsertNewCustEvidence", dbResult.ErrorMsg, custEvidences);

                    return false;
                }

                return true;
            }

            return isPrevSuccess;
        }

        public ICustomerEvidenceInsertServices UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences)
        {
            var ctsCustIds = string.Join(",", custEvidences.Select(x => x.CTSCustID).Distinct());

            var associatedList = customerEvidenceInsertDataAccess.GetAssociatedAccountByCTSCustIds(ctsCustIds);

            if (associatedList.Any())
            {
                var custIds = string.Join(",", associatedList.Distinct());

                var dbResult = customerEvidenceInsertDataAccess.UpdateBetLimitOfFlaggedCustomer(custIds);

                if (dbResult.ErrorCode != 0)
                {
                    WriteLogError("UpdateBetLimitOfFlaggedCustomer", dbResult.ErrorMsg, custEvidences);
                }
            }

            return this;
        }

        public ICustomerEvidenceInsertServices UpdateProcessedCustEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isProcessed)
        {
            var idList = string.Join(",", custEvidences.Select(x => x.ID));

            var dbResult = customerEvidenceInsertDataAccess.UpdateProcessedCustEvidence(idList, isProcessed);

            if (dbResult.ErrorCode != 0)
            {
                WriteLogError("UpdateProcessedCustEvidence", dbResult.ErrorMsg, custEvidences);
            }

            return this;
        }

        public List<CustomerEvidenceFailedToProcessEntity> GetLogError()
        {
            return failedToProcessList;
        }

        private void WriteLogError(string method, string errorMessage, IEnumerable<CustomerEvidenceFromFileEntity> errorCustEvidences)
        {
            failedToProcessList.AddRange(
                errorCustEvidences.Select(x => new CustomerEvidenceFailedToProcessEntity
                {
                    FileKey = x.FileKey,
                    SubscriberID = x.SubscriberID,
                    CTSCustID = x.CTSCustID,
                    EvidenceID = x.EvidenceID,
                    ErrorContent = method + " " + errorMessage
                })
            );
        }

        public IEnumerable<IEnumerable<CustomerEvidenceFromFileEntity>> ChunkSize(List<CustomerEvidenceFromFileEntity> custEvidenceInFiles, int internalBatchSize)
        {
            var queue = new Queue<CustomerEvidenceFromFileEntity>();
            var totalItem = custEvidenceInFiles.Count;

            for (int index = 0; index < totalItem; index++)
            {
                if (queue.Count >= internalBatchSize)
                {
                    var prevIndex = index - 1;

                    if (custEvidenceInFiles[index].CTSCustID == custEvidenceInFiles[prevIndex].CTSCustID)
                    {
                        queue.Enqueue(custEvidenceInFiles[index]);

                        continue;
                    }

                    var ret = queue;
                    queue = new Queue<CustomerEvidenceFromFileEntity>();

                    yield return ret;
                }

                queue.Enqueue(custEvidenceInFiles[index]);
            }

            yield return queue;
        }
    }
}