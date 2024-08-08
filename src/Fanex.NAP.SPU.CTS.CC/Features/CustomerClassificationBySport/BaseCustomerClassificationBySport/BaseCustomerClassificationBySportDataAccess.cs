namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using Fanex.Data.Repository;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationBySportDataAccess
    {
        ResultInsertClassificationBySportModel InsertCustomerClassificationBySport(CCInputFlow inputFlow, InsertCustomerClassificationBySportModel model);

        ResultRemoveCustomerClassificationBySportModel RemoveCustomerClassificationBySport(CCInputFlow inputFlow, RemoveCustomerClassificationBySportModel model);
    }

    public class BaseCustomerClassificationBySportDataAccess : IBaseCustomerClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public BaseCustomerClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ResultInsertClassificationBySportModel InsertCustomerClassificationBySport(CCInputFlow inputFlow, InsertCustomerClassificationBySportModel model)
        {
            var criteria = new InsertCustomerClassificationBySportCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<CustInfoClassificationBySportEntity>(criteria);

            return new ResultInsertClassificationBySportModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }

        public ResultRemoveCustomerClassificationBySportModel RemoveCustomerClassificationBySport(CCInputFlow inputFlow, RemoveCustomerClassificationBySportModel model)
        {
            var criteria = new RemoveCustomerClassificationBySportCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<ResultRemoveCustomerClassificationEntity>(criteria);

            return new ResultRemoveCustomerClassificationBySportModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }
    }
}