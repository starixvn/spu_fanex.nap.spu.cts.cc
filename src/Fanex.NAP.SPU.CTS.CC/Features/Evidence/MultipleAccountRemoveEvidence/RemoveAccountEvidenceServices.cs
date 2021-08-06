namespace Fanex.NAP.SPU.CTS.Features.Evidence.MultipleAccountRemoveEvidence
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

        IRemoveAccountEvidenceServices ModifyCustomerEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, int actionId, string serviceName);

        IRemoveAccountEvidenceServices UpdateCustomerEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, string serviceName);

        IEnumerable<long> PrepareCustRescanClassification();
    }

    public class RemoveAccountEvidenceServices : IRemoveAccountEvidenceServices
    {
        private readonly IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess;
        private readonly ISentryClient sentryClient;
        private DBResult modifyResult;
        private DBResult updateResult;

        public RemoveAccountEvidenceServices(IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess, ISentryClient sentryClient)
        {
            this.removeAccountEvidenceDataAccess = removeAccountEvidenceDataAccess;
            this.sentryClient = sentryClient;
        }

        public IRemoveAccountEvidenceServices InitialData()
        {
            modifyResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            updateResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };

            return this;
        }

        public IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue()
        {
            return removeAccountEvidenceDataAccess.GetCustomerEvidenceQueue();
        }

        public IRemoveAccountEvidenceServices ModifyCustomerEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, int actionId, string serviceName)
        {
            var customerEvidenceJson = JsonConvert.SerializeObject(customerEvidence);

            modifyResult = removeAccountEvidenceDataAccess.ModifyCustomerEvidence(customerEvidenceJson, actionId);

            if (modifyResult.ErrorCode == -1)
            {
                Utilities.LogSentryError(serviceName, "ModifyCustomerEvidence", customerEvidenceJson, new Exception(modifyResult.ErrorMsg), sentryClient);
            }

            return this;
        }

        public IRemoveAccountEvidenceServices UpdateCustomerEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, string serviceName)
        {
            if (modifyResult.ErrorCode == 0 && customerEvidence != null && customerEvidence.Any())
            {
                var processedIdList = customerEvidence.Select(x => x.Id);

                if (processedIdList != null && processedIdList.Any())
                {
                    updateResult = removeAccountEvidenceDataAccess.UpdateCustomerEvidence(string.Join(",", processedIdList));
                }
            }

            if (updateResult.ErrorCode == -1)
            {
                Utilities.LogSentryError(serviceName, " UpdateCustomerEvidence", customerEvidence, new Exception(updateResult.ErrorMsg), sentryClient);
            }

            return this;
        }

        public IEnumerable<long> PrepareCustRescanClassification()
        {
            if (updateResult.ErrorCode == 0 && modifyResult.OutParam.ContainsKey("custIds"))
            {
                var custIds = modifyResult.OutParam["custIds"]?.ToString().Split(',').Select(long.Parse);

                return custIds;
            }

            return new List<long>();
        }
    }
}