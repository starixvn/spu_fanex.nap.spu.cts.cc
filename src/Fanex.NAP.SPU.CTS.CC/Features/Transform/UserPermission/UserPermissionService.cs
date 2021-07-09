namespace Fanex.NAP.SPU.CTS.Features.UserPermission
{
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.SPU.CTS.Features.UserPermission.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;

    public interface IUserPermissionService
    {
        void InsertUserPermission(IEnumerable<UserPermissionModel> userPermissions);

        List<UserPermissionModel> GetUserPermissionFromNAP(string functionName, int functionKey, int siteId, bool isWriteLogInfo);

        UserPermissionModel TransformUserPermission(UserPolicy userPolicy, int functionKey);
    }

    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private readonly UserService userService;

        public UserPermissionService(IUserPermissionDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
            userService = new UserService();
        }

        public void InsertUserPermission(IEnumerable<UserPermissionModel> userPermissions)
        {
            if (userPermissions != null && userPermissions.Any())
            {
                dataAccess.InsertUserPermission(JsonConvert.SerializeObject(userPermissions));
            }
        }

        public List<UserPermissionModel> GetUserPermissionFromNAP(string functionName, int functionKey, int siteId, bool isWriteLogInfo)
        {
            var userPolicy = userService.GetUserPolicies(functionName, siteId);
            Utilities.LogSentryInfo("UserPermisison", "Get User Permission", userPolicy, isWriteLogInfo, sentryClient);

            if (userPolicy != null && userPolicy.Any())
            {
                var userPermission = userPolicy.Select(x => TransformUserPermission(x, functionKey));
                return userPermission.ToList();
            }

            return new List<UserPermissionModel>();
        }

        public UserPermissionModel TransformUserPermission(UserPolicy userPolicy, int functionKey)
        {
            return new UserPermissionModel
            {
                UserID = userPolicy.User.Id,
                FuncID = functionKey
            };
        }
    }
}
