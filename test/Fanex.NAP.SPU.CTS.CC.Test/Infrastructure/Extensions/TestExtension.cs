namespace Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions
{
    using System.Linq;
    using System.Reflection;
    using System;

    public class TestExtension
    {
        public static MethodInfo GetPrivateMethod(Type type, string methodName)
        {
            var methodList = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(x => x.Name == methodName && x.IsPrivate);

            if (methodList is null || !methodList.Any())
                throw new EntryPointNotFoundException();

            var method = methodList.First();

            return method;
        }
    }
}