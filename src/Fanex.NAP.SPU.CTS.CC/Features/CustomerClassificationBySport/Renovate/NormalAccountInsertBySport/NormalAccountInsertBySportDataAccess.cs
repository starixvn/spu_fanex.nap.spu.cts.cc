namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport
{
    using Fanex.Data.Repository;
    using System;
    using System.Linq;

    public interface INormalAccountInsertBySportDataAccess
    {
        GetNormalAccountBySportModel GetNormalAccount();

        InsertNormalAccountBySportModel InsertNormalAccount(string custIdList);

        void CompleteNormalAccountBySport(string custIdList, long scannedMaxId);
    }

    public class NormalAccountInsertBySportDataAccess : INormalAccountInsertBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountInsertBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNormalAccountBySportModel GetNormalAccount()
        {
            var criteria = new GetNormalAccountBySportCriteria();

            var data = repository.Fetch<GetNormalAccountBySportEntity>(criteria);

            return new GetNormalAccountBySportModel
            {
                NormalAccount = data,
                ScannedMaxId = criteria.ScannedMaxId,
                IsContinute = data != null && data.Any()
            };
        }

        public InsertNormalAccountBySportModel InsertNormalAccount(string custIdList)
        {
            var criteria = new InsertNormalAccountBySportCriteria
            {
                NewClassification = custIdList
            };

            var custIds = repository.Fetch<long>(criteria);

            return new InsertNormalAccountBySportModel { CustIds = custIds, ErrorMessage = criteria.ErrorMessage };
        }

        public void CompleteNormalAccountBySport(string custIdList, long scannedMaxId)
        {
            var criteria = new CompleteNormalAccountBySportCriteria
            {
                ScannedMaxId = scannedMaxId,
                ListCustId = custIdList
            };

            repository.Execute(criteria);
        }

    }
}
