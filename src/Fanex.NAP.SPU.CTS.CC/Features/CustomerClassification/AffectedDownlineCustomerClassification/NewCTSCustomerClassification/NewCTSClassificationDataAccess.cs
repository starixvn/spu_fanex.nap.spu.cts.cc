namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using System.Linq;

    public interface INewCTSClassificationDataAccess
    {
        NewCTSCustomerClassificationEntity GetNewCTSCustomerCategory();
    }

    public class NewCTSClassificationDataAccess : INewCTSClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public NewCTSClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public NewCTSCustomerClassificationEntity GetNewCTSCustomerCategory()
        {
            var criteria = new NewCTSCustomerCategoryGetCriteria();

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            var result = db.QueryMultiple(criteria);

            return new NewCTSCustomerClassificationEntity()
            {
                LastCTSCustID = criteria.LastCTSCustID,
                VVIPClassification = result?.Read<VVIPCustomerEntity>() ?? Enumerable.Empty<VVIPCustomerEntity>(),
                PAClassification = result?.Read<InsertProblemAccountEntity>() ?? Enumerable.Empty<InsertProblemAccountEntity>(),
                NormalClassification = result?.Read<NormalRescanEntity>() ?? Enumerable.Empty<NormalRescanEntity>()
            };
        }
    }
}