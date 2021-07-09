namespace Fanex.NAP.SPU.CTS.Features.UserPermission
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.UserPermission.Criteria;

    public interface IUserPermissionDataAccess
    {
        void InsertUserPermission(string permissionUserIdsJson);
    }

    public class UserPermissionDataAccess : IUserPermissionDataAccess
    {
        readonly IDynamicRepository repository;

        public UserPermissionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertUserPermission(string permissionUserIdsJson)
        {
            var criteria = new InsertUserPermissionCriteria
            {
                UserIDList = permissionUserIdsJson
            };

            repository.Execute(criteria);
        }
    }
}
