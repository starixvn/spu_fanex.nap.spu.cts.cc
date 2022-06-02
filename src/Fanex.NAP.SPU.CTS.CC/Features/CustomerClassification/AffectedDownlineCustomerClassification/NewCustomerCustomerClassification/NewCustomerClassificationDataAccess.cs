namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Linq;

    public interface INewCustomerClassificationDataAccess
    {
        NewCustomerClassificationEntity GetNewCTSCustomerCategory();
    }

    public class NewCustomerClassificationDataAccess : INewCustomerClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public NewCustomerClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public NewCustomerClassificationEntity GetNewCTSCustomerCategory()
        {
            var criteria = new NewCustomerCategoryGetCriteria();

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            var result = db.QueryMultiple(criteria);

            return new NewCustomerClassificationEntity()
            {
                VVIPClassification = result?.Read<VVIPCategoryEntity>() ?? Enumerable.Empty<VVIPCategoryEntity>(),
                PAClassification = result?.Read<InsertProblemAccountEntity>() ?? Enumerable.Empty<InsertProblemAccountEntity>(),
                NormalClassification = result?.Read<NormalRescanEntity>() ?? Enumerable.Empty<NormalRescanEntity>(),
                LastCTSCustID = criteria.LastCTSCustID
            };
        }
    }
}