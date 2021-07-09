
namespace Fanex.NAP.SPU.CTS.Features.UpdateLastLogin
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.UpdateLastLogin.Criterias;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public interface IUpdateLastLoginDataAccess
    {
        void TransformUpdatingLastLoginTime(int batchSize);
    }

    public class UpdateLastLoginDataAccess : IUpdateLastLoginDataAccess
    {
        readonly IDynamicRepository repository;

        public UpdateLastLoginDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void TransformUpdatingLastLoginTime(int batchSize)
        {
            var criteria = new TransformUpdatingLastLoginTimeCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }
    }
}
