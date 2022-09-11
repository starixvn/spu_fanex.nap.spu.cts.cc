namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.Data.Repository;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationBySportDataAccess
    {
        DailyNormalAccountsClassificationBySportEntity GetDailyNormalAccountsClassificationBySport();

        void ScanDailyNormalAccountsClassificationBySport(string custs, FunctionId funcID);

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

            var result = repository.Fetch<long>(criteria);

            return new DailyNormalAccountsClassificationBySportEntity
            {
                CustID = result ?? Enumerable.Empty<long>(),
                LastCategoryID = criteria.LastCategoryID,
                LastCustID = criteria.LastCustID,
                LastSportID = criteria.LastSportID
            };
        }

        public void ScanDailyNormalAccountsClassificationBySport(string custs, FunctionId funcID)
        {
            if (!string.IsNullOrEmpty(custs))
            {
                var criteria = new ScanDailyNormalAccountsClassificationBySportCriteria
                {
                    ListCustIDs = custs,                  
                    FunctionID = (int)funcID
                };

                repository.Execute(criteria);
            }
        }
    }
}
