using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SysML.Interactive
{
    public class SysMLKernel :
        Kernel,
        IKernelCommandHandler<SubmitCode>
    {
        private SysMLRpcClient SysMLRpcClient { get; set; }
        private Process SysMLProcess { get; set; }
        public SysMLInteractiveResult LastSubmissionResult { get; private set; }
        public string LastSubmissionSvg { get; private set; }

        public SysMLKernel(string name) : base(name)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = @"-jar C:\Users\tomma\Documents\programs\java\SysMLKernelServer\out\artifacts\out\SysMLKernelServer.jar",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            SysMLProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
            SysMLProcess.Start();
            SetProcessEventHandlers();

            SysMLRpcClient = new(SysMLProcess.StandardInput.BaseStream, SysMLProcess.StandardOutput.BaseStream);
        }

        private void SetProcessEventHandlers()
        {
            SysMLProcess.Exited += (o, args) =>
            {
                Console.WriteLine("process exited");
            };

            SysMLProcess.ErrorDataReceived += (o, args) =>
            {
                Console.WriteLine(args.Data);
            };
            SysMLProcess.BeginErrorReadLine();
        }

        public async Task HandleAsync(SubmitCode submitCode, KernelInvocationContext context)
        {
            var result = await SysMLRpcClient.EvalAsync(submitCode.Code);
            LastSubmissionResult = result;

            var sumbittedItems = LastSubmissionResult.Content.Select(c => c.Name);
            var svg = await SysMLRpcClient.GetSvgAsync(sumbittedItems, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            LastSubmissionSvg = svg;

            context.Publish(new ReturnValueProduced(result, submitCode, FormattedValue.FromObject(result)));
        }

        public new void Dispose()
        {
            SysMLProcess.Kill(true);
            base.Dispose();
        }
    }
}