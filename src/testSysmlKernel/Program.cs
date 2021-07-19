using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;
using SysML.Interactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testSysMLKernel
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var input = @"
#!sysml
package 'Part Definition Example' {
	import ScalarValues::*;
	
	part def Vehicle {
		attribute mass : Real;
		attribute status : VehicleStatus;
		
		part eng : Engine;
		
		ref part driver : Person;
	}
	
	attribute def VehicleStatus {
		gearSetting : Integer;
		acceleratorPosition : Real;
	}
	
	part def Engine;	
	part def Person;
}
";

            // WITHOUT KERNEL

            //var psi = new ProcessStartInfo
            //{
            //    FileName = "java",
            //    Arguments = @"-jar C:\Users\tomma\Documents\programs\java\SysMLKernelServer\out\artifacts\out\SysMLKernelServer.jar",
            //    RedirectStandardInput = true,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //    UseShellExecute = false,
            //};

            //var sysMLProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
            //sysMLProcess.Start();

            //sysMLProcess.Exited += (o, args) =>
            //{
            //    Console.WriteLine("process exited");
            //};

            //sysMLProcess.ErrorDataReceived += (o, args) =>
            //{
            //    Console.WriteLine(args.Data);
            //};
            //sysMLProcess.BeginErrorReadLine();

            //var sysMLRpcClient = new SysMLRpcClient(sysMLProcess.StandardInput.BaseStream, sysMLProcess.StandardOutput.BaseStream);

            //var result = await sysMLRpcClient.EvalAsync(input);
            //Console.WriteLine(result?.ToString() ?? "(null)");

            //sysMLProcess.Kill();




            // KERNEL

            var compositeKernel = new CompositeKernel().UseSysML();

            //var compositeKernel = Kernel.Root as CompositeKernel;

            var errorMessage = string.Empty;
            ReturnValueProduced returnValue = null;
            var displayedValues = new List<DisplayedValueProduced>();

            compositeKernel.KernelEvents.Subscribe(e =>
            {
                switch(e)
                {
                    case CommandFailed cf:
                        {
                            errorMessage = cf.Message;
                        }
                        break;
                    case ReturnValueProduced rvp:
                        {
                            returnValue = rvp;
                        }
                        break;
                    case DisplayedValueProduced dvp:
                        {
                            displayedValues.Add(dvp);
                        }
                        break;
                }
            });
            var kernelCommandResult = await compositeKernel.SendAsync(new SubmitCode(input), System.Threading.CancellationToken.None);

            return 0;
        }
    }
}