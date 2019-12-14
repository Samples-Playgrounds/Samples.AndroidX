using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class LogTestInfoAttribute : BeforeAfterTestAttribute
    {
        private string formattedTestName;
        private readonly Stopwatch stopwatch = new Stopwatch();

        public override void Before(MethodInfo methodUnderTest)
        {
            var methodName = toSentenceCase(methodUnderTest.Name);
            formattedTestName = getFormattedName(methodUnderTest.ReflectedType, methodName).Replace("`1", "");
            Console.WriteLine(formattedTestName);

            stopwatch.Start();
        }

        public override void After(MethodInfo methodUnderTest)
        {
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed} - {formattedTestName}");
        }

        private string getFormattedName(Type typeInfo, string accumulator)
        {
            if (typeInfo.DeclaringType == null)
                return $"{typeInfo.Name}: {accumulator[0]}{accumulator.Substring(1).ToLower()}";

            return getFormattedName(typeInfo.DeclaringType, $"{toSentenceCase(typeInfo.Name)} {accumulator}");
        }

        public static string toSentenceCase(string value)
            => Regex.Replace(value, "[a-z][A-Z]", x => $"{x.Value[0]} {char.ToLower(x.Value[1])}");
    }
}
