namespace Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface IAntiFraudCenterDataAccess
    {
        IEnumerable<GetB2CEntity> GetB2CCustIds(string custIds);

        IEnumerable<FilterCustToSyncAFCKEntity> FilterLicenseeCust(string custIds);

        IEnumerable<CheckAccountExistEntity> CheckAccountValidation(int? participantID, IEnumerable<CheckAccountExistEntity> accountIDs);

        IEnumerable<ParticipantEntity> GetParticipant();

        void InsertErrorLog(int? participantID, string errorMessage, string dataJson);
    }

    public class AntiFraudCenterDataAccess : IAntiFraudCenterDataAccess
    {
        private readonly IDynamicRepository repository;

        public AntiFraudCenterDataAccess()
        {
            repository = new DynamicRepository();
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

        public void InsertErrorLog(int? participantID, string errorMessage, string dataJson)
        {
            var criteria = new ErrorLogInsertCriteria
            {
                ErrorMessage = errorMessage,
                DataJson = dataJson,
                ParticipantID = participantID
            };

            repository.Execute(criteria);
        }
    }
}