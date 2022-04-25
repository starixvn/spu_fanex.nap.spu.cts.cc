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

        IRemoveAccountEvidenceServices RemoveCustEvidenceFromQueue(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, int actionId, string serviceName, ISentryClient sentryClient);

        IRemoveAccountEvidenceServices UpdateCustomerEvidenceComplete(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, string serviceName, ISentryClient sentryClient);

        ResultRemoveCustEviFromQueueEntity GetCustomerToUpdateCC();

        IRemoveAccountEvidenceServices CheckCleanCustFlaggedEvidence(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence);

        IRemoveAccountEvidenceServices UpdateBetLimitOfUnflaggedCustomer(string serviceName, ISentryClient sentryClient);
    }

    public class RemoveAccountEvidenceServices : IRemoveAccountEvidenceServices
    {
        private readonly IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess;
        private DBResult removeResult;
        private DBResult updateResult;
        private IEnumerable<CheckCleanEntity> custToUpdateBetlimit;

        public RemoveAccountEvidenceServices(IRemoveAccountEvidenceDataAccess removeAccountEvidenceDataAccess)
        {
            this.removeAccountEvidenceDataAccess = removeAccountEvidenceDataAccess;
        }

        public IRemoveAccountEvidenceServices InitialData()
        {
            removeResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            updateResult = new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
            custToUpdateBetlimit = new List<CheckCleanEntity>();

            return this;
        }

        public IEnumerable<CustomerEvidenceQueueEntity> GetCustomerEvidenceQueue()
        {
            return removeAccountEvidenceDataAccess.GetCustomerEvidenceQueue();
        }

        public IRemoveAccountEvidenceServices RemoveCustEvidenceFromQueue(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, int actionId, string serviceName, ISentryClient sentryClient)
        {
            var customerEvidenceJson = JsonConvert.SerializeObject(customerEvidence);

            removeResult = removeAccountEvidenceDataAccess.RemoveCustEvidenceFromQueue(customerEvidenceJson, actionId);

            if (removeResult.ErrorCode == -1)
            {
                Utilities.LogSentryError(serviceName, "ModifyCustomerEvidence", customerEvidenceJson, new Exception(removeResult.ErrorMsg), sentryClient);
            }

            return this;
        }

        public IRemoveAccountEvidenceServices UpdateCustomerEvidenceComplete(IEnumerable<CustomerEvidenceQueueEntity> customerEvidence, string serviceName, ISentryClient sentryClient)
        {
            if (removeResult.ErrorCode == 0 && customerEvidence != null && customerEvidence.Any())
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

        public ResultRemoveCustEviFromQueueEntity GetCustomerToUpdateCC()
        {
            if (removeResult.ErrorCode == 0 && removeResult.OutParam.ContainsKey("custIds"))
            {
                var custIds = removeResult.OutParam["custIds"]?.ToString().Split(',').Select(long.Parse);

                return new ResultRemoveCustEviFromQueueEntity { CustIds = custIds, IsRemoveSuccess = (removeResult.ErrorCode == 0) };
            }

            return new ResultRemoveCustEviFromQueueEntity { CustIds = new List<long>(), IsRemoveSuccess = (removeResult.ErrorCode == 0) };
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

        public IRemoveAccountEvidenceServices UpdateBetLimitOfUnflaggedCustomer(string serviceName, ISentryClient sentryClient)
        {
            if (custToUpdateBetlimit != null && custToUpdateBetlimit.Any())
            {
                var custIds = string.Join(",", custToUpdateBetlimit.Select(c => c.CustID).Distinct());

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