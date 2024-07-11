namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data.Repository;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IBaseCustomerClassificationDataAccess
    {
        GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs);

        ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model);

        ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model);
    }

    public class BaseCustomerClassificationDataAccess : IBaseCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public BaseCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }      

        public GetRescanCustomerClassificationModel GetRescanCustomerClassification(string custIDs)
        {
            var criteria = new GetRescanCustomerClassificationCriteria
            {
                CustIDs = custIDs
            };

            var custInfos = repository.Fetch<GetRescanCustomerClassificationEntity>(criteria);

            return new GetRescanCustomerClassificationModel { CustInfos = custInfos };
        } 

        public ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            var criteria = new InsertCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                FromAction = model.FromAction,
                UplineRoleID = model.UplineRoleID,
                QueueID = model.QueueID,
                AffectDownline = model.AffectDownline,
                IsToQueue = model.IsToQueue,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<CustInfoClassificationEntity>(criteria);

            return new ResultInsertCustomerClassificationModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }

        public ResultRemoveCustomerClassificationModel RemoveCustomerClassification(CCInputFlow inputFlow, RemoveCustomerClassificationModel model)
        {
            var criteria = new RemoveCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                FromAction = model.FromAction,
                UplineRoleID = model.UplineRoleID,
                QueueID = model.QueueID,
                AffectDownline = model.AffectDownline,
                IsFromQueue = model.IsFromQueue,
                CustClassificationJson = model.CustClassificationJson
            };

            var custInfos = repository.Fetch<ResultRemoveCustomerClassificationEntity>(criteria);

            return new ResultRemoveCustomerClassificationModel { CustInfos = custInfos, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
