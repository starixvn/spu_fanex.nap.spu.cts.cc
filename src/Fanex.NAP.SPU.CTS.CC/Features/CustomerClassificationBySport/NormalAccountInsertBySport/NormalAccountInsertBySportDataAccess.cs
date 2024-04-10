namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using System;
    using System.Linq;

    public interface INormalAccountInsertBySportDataAccess
    {
        GetNormalAccountBySportModel GetNormalAccount();

        InsertNormalAccountBySportModel InsertNormalAccount(string custIdList);

        void CompleteNormalAccountBySport(string customersXML, long scannedMaxId);
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

            var custNAInserted = repository.Fetch<InsertNormalAccountCustInfoEntity>(criteria);

            return new InsertNormalAccountBySportModel { CustNAInserted = custNAInserted, ErrorMessage = criteria.ErrorMessage };
        }

        public void CompleteNormalAccountBySport(string customersXML, long scannedMaxId)
        {
            var criteria = new CompleteNormalAccountBySportCriteria
            {
                ScannedMaxId = scannedMaxId,
                CustomersXML = customersXML
            };

            repository.Execute(criteria);
        }

    }
}
