namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using System.Collections.Generic;

    public interface IAntiFraudCenterDataAccess
    {
        IEnumerable<GetAFCAccountEntity> GetAFCAccountInfo(string custIdJson);

        IEnumerable<GetB2CEntity> GetB2CCustIds(string custIds);

        IEnumerable<long> FilterCustIdB2CLicensee(string custIds);
    }

    public class AntiFraudCenterDataAccess : IAntiFraudCenterDataAccess
    {
        private readonly IDynamicRepository repository;

        public AntiFraudCenterDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetAFCAccountEntity> GetAFCAccountInfo(string custIdJson)
        {
            var criteria = new GetAFCAccountInfoCriteria
            {
                B2CCustIdJson = custIdJson
            };

            return repository.Fetch<GetAFCAccountEntity>(criteria);
        }

        public IEnumerable<GetB2CEntity> GetB2CCustIds(string custIds)
        {
            var criteria = new GetB2CCustIdsCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<GetB2CEntity>(criteria);
        }

        public IEnumerable<long> FilterCustIdB2CLicensee(string custIds)
        {
            var criteria = new FilterB2CLicenseeGetCriteria
            {
                CustIDs = custIds
            };

            return repository.Fetch<long>(criteria);
        }
    }
}