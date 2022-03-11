namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerEvidenceInsertServices
    {
        ICustomerEvidenceInsertServices Start();

        IEnumerable<CustomerEvidenceFromFileEntity> GetCustomerEvidenceFromFile();

        bool RemoveOldEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, out IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList);

        bool InsertNewCustomerEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isOverwrite, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, EvidenceActionType actionType
              , IEnumerable<CustomerEvidenceRemovedEntity> custEviRemovedList, bool isPrevSuccess);

        bool SyncCustomerEvidenceToCTSCategory(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, EvidenceActionType actionType, bool isPrevSuccess);

        ICustomerEvidenceInsertServices UpdateCompleteCustEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isProcessed);

        ICustomerEvidenceInsertServices UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences);

        List<CustomerEvidenceFailedToProcessEntity> GetLogError();

        IEnumerable<IEnumerable<CustomerEvidenceFromFileEntity>> ChunkSize(List<CustomerEvidenceFromFileEntity> custEvidenceInFiles, int internalBatchSize);

        IEnumerable<CustEvidenceByReasonIdEntity> GetCustEvidenceByReasonId(IEnumerable<ProblemAccountsEntity> problemAccounts);

        void InsertCustEvidenceFromProblemAccount(IEnumerable<CustEvidenceByReasonIdEntity> custEvidences);
    }

    public class CustomerEvidenceInsertServices : ICustomerEvidenceInsertServices
    {
        private readonly ICustomerEvidenceInsertDataAccess customerEvidenceInsertDataAccess;
        private List<CustomerEvidenceFailedToProcessEntity> failedToProcessList;
        private readonly ISentryClient sentryClient;
        private const string ServiceNameCustEviInsert = "Customer Evidence Insert";

        public CustomerEvidenceInsertServices(ICustomerEvidenceInsertDataAccess customerEvidenceInsertDataAccess, ISentryClient sentryClient)
        {
            this.customerEvidenceInsertDataAccess = customerEvidenceInsertDataAccess;
            this.sentryClient = sentryClient;
        }

        public ICustomerEvidenceInsertServices Start()
        {
            this.failedToProcessList = new List<CustomerEvidenceFailedToProcessEntity>();

            return this;
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
                else if(dbResult.OutParam != null)
                {
                    var hashCustInfos = dbResult.OutParam["custInfos"];

                    if (hashCustInfos != null)
                    {
                        var custInfos = (IEnumerable<NewPAAssociationCustomerForRollbackEntity>)hashCustInfos;

                        if (custInfos.Any())
                        {
                            this.CheckNewPAAssociationCustomerForRollback(custInfos, ServiceNameCustEviInsert);
                        }
                    }
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
                else if (dbResult.OutParam != null)
                {
                    var hashCustInfos = dbResult.OutParam["custInfos"];

                    if (hashCustInfos != null)
                    {
                        var custInfos = (IEnumerable<NewPAAssociationCustomerForRollbackEntity>)hashCustInfos;

                        if (custInfos.Any())
                        {
                            this.CheckNewPAAssociationCustomerForRollback(custInfos, ServiceNameCustEviInsert);
                        }
                    }
                }

                return true;
            }

            return isPrevSuccess;
        }

        public ICustomerEvidenceInsertServices UpdateBetLimitOfFlaggedCustomer(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences)
        {
            var custIds = string.Join(",", custEvidences.Select(x => x.CustID).Distinct());

            if (custIds.Any())
            {
                var custIdsToString = string.Join(",", custIds);

                var dbResult = customerEvidenceInsertDataAccess.UpdateBetLimitOfFlaggedCustomer(custIdsToString);

                if (dbResult.ErrorCode != 0)
                {
                    WriteLogError("UpdateBetLimitOfFlaggedCustomer", dbResult.ErrorMsg, custEvidences);
                }
            }

            return this;
        }

        public ICustomerEvidenceInsertServices UpdateCompleteCustEvidence(IEnumerable<CustomerEvidenceFromFileEntity> custEvidences, bool isProcessed)
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

        private void CheckNewPAAssociationCustomerForRollback(IEnumerable<NewPAAssociationCustomerForRollbackEntity> assCustForRollbacks, string serviceName)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                try
                {
                    var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                    var updateResult = customerEvidenceInsertDataAccess.CheckNewPAAssociationCustomerForRollback(custInfoJson);
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "CheckNewPAAssociationCustomerForRollback throws exception", assCustForRollbacks, ex, sentryClient);
                }
            }
        }

        public IEnumerable<CustEvidenceByReasonIdEntity> GetCustEvidenceByReasonId(IEnumerable<ProblemAccountsEntity> problemAccounts)
        {
            var custReasons = problemAccounts.Select(x => new CustReasonEntity
            {
                CustID = x.CustID,
                ReasonID = x.ReasonID
            });

            var custInfoJson = JsonConvert.SerializeObject(custReasons);

            return customerEvidenceInsertDataAccess.GetCustEvidenceByReasonId(custInfoJson);
        }

        public void InsertCustEvidenceFromProblemAccount(IEnumerable<CustEvidenceByReasonIdEntity> custEvidences)
        {
            var CreatedBy = ImportEvidence_StarixITId;

            var objects = custEvidences.Select(x => new
            {
                x.CTSCustID,
                x.EvidenceID,
                x.Remark,
                CreatedBy
            });

            var jsonData = JsonConvert.SerializeObject(objects);

            var dbResult = customerEvidenceInsertDataAccess.InsertCustomerEvidenceMuitiple(jsonData, false);
        }
    }
}

