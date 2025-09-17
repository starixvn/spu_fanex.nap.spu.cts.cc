namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections.Generic; 
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountDataAccess
    {       
        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(string custJson, FunctionId funcId);
             
        int CheckNewPAAssociationCustomerForRollback(string custInfoJson);

        DBResult UpdateBetLimitOfFlaggedCustomer(string custIds);

        DBResult UpdateBetLimitOfUnflaggedCustomer(string custIds);      
    }

    public class ProblemAccountDataAccess : BaseCustomerClassificationDataAccess, IProblemAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProblemAccountDataAccess()
        {
            repository = new DynamicRepository();
        }       

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(string custJson, FunctionId funcId)
        {
            var criteria = new ProblemAccountLosingClassificationGetCriteria
            {
                CustJson = custJson,
                FunctionId = (int)funcId
            };

            return repository.Fetch<ProblemAccountLosingClassificationEntity>(criteria);
        }            

        public int CheckNewPAAssociationCustomerForRollback(string custInfoJson)
        {
            var criteria = new CheckNewPAAssociationCustomerForRollbackCriteria
            {
                CustInfoJson = custInfoJson
            };

            return repository.Execute(criteria);
        }

        public DBResult UpdateBetLimitOfFlaggedCustomer(string custIds)
        {
            var criteria = new BetLimitByEvidenceUpdateCriteria
            {
                CustIDList = custIds
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult UpdateBetLimitOfUnflaggedCustomer(string custIds)
        {
            var criteria = new BetLimitByEvidenceRemoveCriteria
            {
                CustIDList = custIds
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage
            };
        }     
    }
}