namespace Fanex.NAP.SPU.CTS.Tests.Features.Transform
{
    using Fanex.NAP.SPU.CTS.Features.UserPermission;
    using NSubstitute;
    using Sentry;

    public class UserPermissionUnitTest
    {
        private readonly UserPermissionService permissionService;
        private readonly IUserPermissionDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public UserPermissionUnitTest()
        {
            dataAccess = Substitute.For<IUserPermissionDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            permissionService = new UserPermissionService(dataAccess, sentryClient);
        }
    }
}