namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPAPotentialClassificationDataAccess
    {
        IEnumerable<TWInitialGBGetChangesEntity> GetInitialTWGroupBetting(string custIDs);

        IEnumerable<RescanTWInitialSmartPAEntity> CheckExistInitialSmartPA(string custIDs);
    }

    public class PAPotentialClassificationDataAccess : IPAPotentialClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public PAPotentialClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<TWInitialGBGetChangesEntity> GetInitialTWGroupBetting(string custIDs)
        {
            var criteria = new RescanTWInitialGroupBettingCriteria() { CustIDs = custIDs };

            return repository.Fetch<TWInitialGBGetChangesEntity>(criteria);
        }

        public IEnumerable<RescanTWInitialSmartPAEntity> CheckExistInitialSmartPA(string custIDs)
        {
            var criteria = new RescanTWInitialSmartPACriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanTWInitialSmartPAEntity>(criteria);
        }
    }
}
