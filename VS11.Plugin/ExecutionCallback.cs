using System;
using Chutzpah.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Chutzpah.VS11
{
    public class ExecutionCallback : RunnerCallback
    {
        private readonly IFrameworkHandle frameworkHandle;

        public ExecutionCallback(IFrameworkHandle frameworkHandle)
        {
            this.frameworkHandle = frameworkHandle;
        }

        public override void FileError(TestError error)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Error, GetFileErrorMessage(error));
        }

        public override void FileLog(TestLog log)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, GetFileLogMessage(log));
        }

        public override void ExceptionThrown(Exception exception, string fileName)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Error, GetExceptionThrownMessage(exception, fileName));
        }

        public override void TestStarted(TestCase test)
        {
            var testCase = test.ToVsTestCase();

            // The test case is starting
            frameworkHandle.RecordStart(testCase);

        }

        public override void TestFinished(TestCase test)
        {
            var testCase = test.ToVsTestCase();
            var results = test.ToVsTestResults();
            var outcome = ChutzpahExtensionMethods.ToVsTestOutcome(test.Passed);

            // Record a result (there can be many)
            foreach (var result in results)
            {
                frameworkHandle.RecordResult(result);
            }

            // The test case is done
            frameworkHandle.RecordEnd(testCase, outcome);
        }

    }
}