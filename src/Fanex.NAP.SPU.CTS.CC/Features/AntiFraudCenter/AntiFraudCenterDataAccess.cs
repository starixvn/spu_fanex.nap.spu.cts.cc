namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface IAntiFraudCenterDataAccess
    {
        IEnumerable<GetAccountEntity> GetB2CInfo(string custIdJson);

        IEnumerable<GetB2CEntity> GetB2CCustIds(string custIds);

        IEnumerable<FilterCustToSyncAFCKEntity> FilterLicenseeCust(string custIds);

        IEnumerable<CheckAccountExistEntity> CheckAccountValidation(int? participantID, IEnumerable<CheckAccountExistEntity> accountIDs);

        IEnumerable<ParticipantEntity> GetParticipant();
    }

    public class AntiFraudCenterDataAccess : IAntiFraudCenterDataAccess
    {
        private readonly IDynamicRepository repository;

        public AntiFraudCenterDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetAccountEntity> GetB2CInfo(string custIdJson)
        {
            var criteria = new GetAFCAccountInfoCriteria
            {
                B2CCustIdJson = custIdJson
            };

            return repository.Fetch<GetAccountEntity>(criteria);
        }

        public IEnumerable<GetB2CEntity> GetB2CCustIds(string custIds)
        {
            var criteria = new GetB2CCustIdsCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<GetB2CEntity>(criteria);
        }

        public IEnumerable<FilterCustToSyncAFCKEntity> FilterLicenseeCust(string custIds)
        {
            var criteria = new LicenseeCustFilterCriteria
            {
                CustIDs = custIds
            };

            return repository.Fetch<FilterCustToSyncAFCKEntity>(criteria);
        }

        public IEnumerable<CheckAccountExistEntity> CheckAccountValidation(int? participantID, IEnumerable<CheckAccountExistEntity> accountIDs)
        {
            var criteria = new AccountCheckExistCriteria
            {
                JsonAccountInfo = JsonConvert.SerializeObject(accountIDs),
                ParticipantID = participantID
            };

            return repository.Fetch<CheckAccountExistEntity>(criteria);
        }

        public IEnumerable<ParticipantEntity> GetParticipant()
        {
            var criteria = new ParticipantGetCriteria();

            return repository.Fetch<ParticipantEntity>(criteria);
        }
    }
}