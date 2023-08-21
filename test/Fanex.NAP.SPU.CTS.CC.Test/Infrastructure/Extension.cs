namespace Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure
{
    using System.Linq;
    using System.Reflection;
    using System;

    public class Extension
    {
        public static MethodInfo GetPrivateMethod<T>(object instance, string methodName, object[] callParams) where T : class
        {
            var classType = instance.GetType();
            var methodList = classType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

            if (methodList is null || !methodList.Any())
                throw new EntryPointNotFoundException();

            var method = methodList.First(x => x.Name == methodName && !x.IsPublic && x.GetParameters().Length == callParams.Length);

            return method;
        }
    }
}