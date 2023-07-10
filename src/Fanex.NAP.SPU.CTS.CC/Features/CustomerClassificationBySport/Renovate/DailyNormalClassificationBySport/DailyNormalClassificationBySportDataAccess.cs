namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using Fanex.Data.Repository;
    using System.Linq;    

    public interface IDailyNormalClassificationBySportDataAccess
    {
        NormalAccountBySportModel GetDailyNormalAccountsClassificationBySport();

        void PreprocessDailyNormalAccountsClassificationBySport(string customersXML);
    }

    public class DailyNormalClassificationBySportDataAccess : IDailyNormalClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyNormalClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public NormalAccountBySportModel GetDailyNormalAccountsClassificationBySport()
        {
            var criteria = new GetDailyNormalAccountsClassificationBySportCriteria();

            var data = repository.Fetch<NormalAccountBySportEntity>(criteria);

            return new NormalAccountBySportModel
            {
                NormalAccounts = data ?? Enumerable.Empty<NormalAccountBySportEntity>(),
                LastCategoryID = criteria.LastCategoryID,
                LastCustID = criteria.LastCustID
            };
        }

        public void PreprocessDailyNormalAccountsClassificationBySport(string customersXML)
        {
            if (!string.IsNullOrEmpty(customersXML))
            {
                var criteria = new PreprocessDailyNormalAccountsClassificationBySportCriteria
                {
                    CustomersXML = customersXML
                };

                repository.Execute(criteria);
            }
        }
    }
}
