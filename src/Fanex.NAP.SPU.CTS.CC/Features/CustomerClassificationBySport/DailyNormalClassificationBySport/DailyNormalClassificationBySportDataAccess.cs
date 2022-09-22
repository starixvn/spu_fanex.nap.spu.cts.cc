namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.Data.Repository;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationBySportDataAccess
    {
        DailyNormalAccountsClassificationBySportEntity GetDailyNormalAccountsClassificationBySport();

        void ScanDailyNormalAccountsClassificationBySport(string custs);

    }

    public class DailyNormalClassificationBySportDataAccess : IDailyNormalClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyNormalClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public DailyNormalAccountsClassificationBySportEntity GetDailyNormalAccountsClassificationBySport()
        {
            var criteria = new GetDailyNormalAccountsClassificationBySportCriteria();

            var data = repository.Fetch<NormalAccountBySportEntity>(criteria);

            return new DailyNormalAccountsClassificationBySportEntity
            {
                NormalAccounts = data ?? Enumerable.Empty<NormalAccountBySportEntity>(),
                LastCategoryID = criteria.LastCategoryID,
                LastCustID = criteria.LastCustID               
            };
        }

        public void ScanDailyNormalAccountsClassificationBySport(string custs)
        {
            if (!string.IsNullOrEmpty(custs))
            {
                var criteria = new ScanDailyNormalAccountsClassificationBySportCriteria
                {
                    CustomersXML = custs
                };

                repository.Execute(criteria);
            }
        }
    }
}
