namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data.Repository;

    public interface INormalAccountInsertBySportDataAccess
    {
        GetNormalAccountBySportModel GetNormalClasificationAccount();

        void CompleteNormalAccountBySport(long scannedMaxId);
    }

    public class NormalAccountInsertBySportDataAccess : INormalAccountInsertBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountInsertBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNormalAccountBySportModel GetNormalClasificationAccount()
        {
            var criteria = new GetNormalAccountBySportCriteria();

            var data = repository.Fetch<NormalAccountBySportEntity>(criteria);

            return new GetNormalAccountBySportModel
            {
                NormalAccount = data,
                ScannedMaxId = criteria.ScannedMaxId
            };
        }

        public void CompleteNormalAccountBySport(long scannedMaxId)
        {
            var criteria = new CompleteNormalAccountBySportCriteria
            {
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }
    }
}