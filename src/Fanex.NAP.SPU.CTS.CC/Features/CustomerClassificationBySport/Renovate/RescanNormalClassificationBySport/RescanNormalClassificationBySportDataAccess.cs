namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using Fanex.Data.Repository;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanNormalClassificationBySportDataAccess
    {
        void PreprocesRescanNormalClassificationBySport(string custIds, BySport_FunctionId functionId);
    }

    public class RescanNormalClassificationBySportDataAccess : IRescanNormalClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public RescanNormalClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void PreprocesRescanNormalClassificationBySport(string custIds, BySport_FunctionId functionId)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new PreprocessRescanNormalClassificationBySportCriteria
                {
                    ListCustId = custIds,
                    FunctionId = (int)functionId
                };

                repository.Execute(criteria);
            }
        }
    }
}