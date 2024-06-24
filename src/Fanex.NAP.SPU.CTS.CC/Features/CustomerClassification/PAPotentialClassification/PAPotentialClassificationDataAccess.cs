namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService;
    using System.Collections.Generic;

    public interface IPAPotentialClassificationDataAccess
    {
        IEnumerable<RescanDangerousScorePAEntity> GetDangerousScorePA(string custIDs);

        IEnumerable<TWInitialGBGetChangesEntity> GetInitialTWGroupBetting(string custIDs);

        IEnumerable<TWInitialSmartEntity> GetInitialSmartPA(string custIDs);
    }

    public class PAPotentialClassificationDataAccess : BaseCustomerClassificationDataAccess, IPAPotentialClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public PAPotentialClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RescanDangerousScorePAEntity> GetDangerousScorePA(string custIDs)
        {
            var criteria = new RescanDangerousScorePACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanDangerousScorePAEntity>(criteria);
        }

        public IEnumerable<TWInitialGBGetChangesEntity> GetInitialTWGroupBetting(string custIDs)
        {
            var criteria = new RescanTWInitialGroupBettingCriteria() { CustIDs = custIDs };

            return repository.Fetch<TWInitialGBGetChangesEntity>(criteria);
        }

        public IEnumerable<TWInitialSmartEntity> GetInitialSmartPA(string custIDs)
        {
            var criteria = new CheckExistTWInitialSmartPACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<TWInitialSmartEntity>(criteria);
        }
    }
}
