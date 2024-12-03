namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface INormalDangerousAssociationCheckDataAccess
    {
        public void PreprocessDangerousAssociation(IEnumerable<PreprocessDangerousAssociationModel> data);
    }

    public class NormalDangerousAssociationCheckDataAccess : INormalDangerousAssociationCheckDataAccess
    {
        public IDynamicRepository _repository;

        public NormalDangerousAssociationCheckDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public void PreprocessDangerousAssociation(IEnumerable<PreprocessDangerousAssociationModel> data)
        {
            var criteria = new PreprocessDangerousAssociationCriteria()
            {
                CustJson = JsonConvert.SerializeObject(data)
            };

            _repository.Execute(criteria);
        }
    }
}
