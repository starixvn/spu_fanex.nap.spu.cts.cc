namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRemoveAccountEvidenceServices
    {
        IRemoveAccountEvidenceServices InitialData();

        IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue();

        IRemoveAccountEvidenceServices RemoveCustEvidenceFromQueue(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, int actionId, string serviceName);

        IRemoveAccountEvidenceServices UpdateCustomerEvidenceComplete(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, string serviceName);

        IEnumerable<long> GetCustomerToUpdateCC();

        IRemoveAccountEvidenceServices CheckCleanCustFlaggedEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence);

        IRemoveAccountEvidenceServices UpdateBetLimitOfUnflaggedCustomer(string serviceName);
    }

    public class RemoveAccountEvidenceServices : IRemoveAccountEvidenceServices
    {
        private readonly IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess;
        private readonly ISentryClient sentryClient;
        private DBResult modifyResult;
        private DBResult updateResult;
        private IEnumerable<long> custToUpdateBetlimit;

        public RemoveAccountEvidenceServices(IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess, ISentryClient sentryClient)
        {
            this.removeAccountEvidenceDataAccess = removeAccountEvidenceDataAccess;
            this.sentryClient = sentryClient;
        }

        public IRemoveAccountEvidenceServices InitialData()
        {
            modifyResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            updateResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            custToUpdateBetlimit = new List<long>();

            return this;
        }

        public IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue()
        {
            return removeAccountEvidenceDataAccess.GetCustomerEvidenceQueue();
        }

        public IRemoveAccountEvidenceServices RemoveCustEvidenceFromQueue(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, int actionId, string serviceName)
        {
            var customerEvidenceJson = JsonConvert.SerializeObject(customerEvidence);

            modifyResult = removeAccountEvidenceDataAccess.RemoveCustEvidenceFromQueue(customerEvidenceJson, actionId);

            if (modifyResult.ErrorCode == -1)
            {
                Utilities.LogSentryError(serviceName, "ModifyCustomerEvidence", customerEvidenceJson, new Exception(modifyResult.ErrorMsg), sentryClient);
            }

            return this;
        }

        public IRemoveAccountEvidenceServices UpdateCustomerEvidenceComplete(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, string serviceName)
        {
            if (modifyResult.ErrorCode == 0 && customerEvidence != null && customerEvidence.Any())
            {
                var processedIdList = customerEvidence.Select(x => x.Id);

                if (processedIdList != null && processedIdList.Any())
                {
                    updateResult = removeAccountEvidenceDataAccess.UpdateCustomerEvidenceComplete(string.Join(",", processedIdList));
                }
            }

            if (updateResult.ErrorCode == -1)
            {
                Utilities.LogSentryError(serviceName, " UpdateCustomerEvidence", customerEvidence, new Exception(updateResult.ErrorMsg), sentryClient);
            }

            return this;
        }

        public IEnumerable<long> GetCustomerToUpdateCC()
        {
            if (modifyResult.ErrorCode == 0 && modifyResult.OutParam.ContainsKey("custIds"))
            {
                var custIds = modifyResult.OutParam["custIds"]?.ToString().Split(',').Select(long.Parse);

                return custIds;
            }

            return new List<long>();
        }

        public IRemoveAccountEvidenceServices CheckCleanCustFlaggedEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence)
        {
            if (customerEvidence != null && customerEvidence.Any())
            {
                var ctsCustIds = string.Join(",", customerEvidence.Select(c => c.CTSCustId).Distinct());

                custToUpdateBetlimit = removeAccountEvidenceDataAccess.CheckCleanCustFlaggedEvidence(ctsCustIds);
            }

            return this;
        }

        public IRemoveAccountEvidenceServices UpdateBetLimitOfUnflaggedCustomer(string serviceName)
        {
            if (custToUpdateBetlimit != null && custToUpdateBetlimit.Any())
            {
                var custIds = string.Join(",", custToUpdateBetlimit.Distinct());

                var result = removeAccountEvidenceDataAccess.UpdateBetLimitOfUnflaggedCustomer(custIds);

                if (result.ErrorCode == -1)
                {
                    Utilities.LogSentryError(serviceName, "UpdateBetLimitOfUnflaggedCustomer", custIds, new Exception(result.ErrorMsg), sentryClient);
                }
            }

            return this;
        }
    }
}