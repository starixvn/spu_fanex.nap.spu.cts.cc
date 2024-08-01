namespace Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions
{
    using System.Linq;
    using System.Reflection;
    using System;

    public class TestExtension
    {
        public static MethodInfo GetPrivateMethod(Type type, string methodName, int pos = 0)
        {
            var methodList = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(x => x.Name == methodName && x.IsPrivate);

            if (methodList is null || !methodList.Any())
                throw new EntryPointNotFoundException();

            var method = methodList.ElementAt(pos);

            return method;
        }

        public static FieldInfo GetPrivateFieldInfo(Type type, string fieldName, int pos = 0)
        {
            var fieldInfo = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(x => x.Name == fieldName && x.IsPrivate);

            if (fieldInfo is null || !fieldInfo.Any())
                throw new EntryPointNotFoundException();

            var field = fieldInfo.ElementAt(pos);

            return field;
        }
    }
}