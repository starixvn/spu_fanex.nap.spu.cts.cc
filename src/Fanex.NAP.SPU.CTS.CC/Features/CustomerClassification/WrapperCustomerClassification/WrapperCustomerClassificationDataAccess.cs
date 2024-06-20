namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.WrapperCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IWrapperCustomerClassificationDataAccess
    {
        ResultInsertCustomerClassificationModel InsertNormalCustomerClassification(InsertCustomerClassificationModel model);
    }

    public class WrapperCustomerClassificationDataAccess : IWrapperCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public WrapperCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }
        
        public ResultInsertCustomerClassificationModel InsertNormalCustomerClassification(InsertCustomerClassificationModel model)
        {
            return InsertCustomerClassification(CCInsertInputFlow.Normal, model);
        }

        private ResultInsertCustomerClassificationModel InsertCustomerClassification(CCInsertInputFlow inputFlow, InsertCustomerClassificationModel model)
        {
            var criteria = new InsertCustomerClassificationCriteria
            {
                InputFlowID = (int)inputFlow,
                CustClassificationJson = model.CustClassificationJson
            };

            var custIDs = repository.Fetch<long>(criteria);

            return new ResultInsertCustomerClassificationModel { CustIDs = custIDs, ErrorMessage = criteria.ErrorMessage };
        }
    }
}
