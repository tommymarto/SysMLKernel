using Microsoft.DotNet.Interactive.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;
using SysML.Interactive;
using System;
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
            //            var input = @"
            //package BlockTest {
            //    part f: A;
            //    public block A {
            //        part b: B;
            //        public port c: C;
            //    }
            //    abstract block B {
            //        public abstract part a: A;
            //        port x: ~C;
            //        package P { }
            //    }
            //    private port def C {
            //        private in ref y: A, B;
            //        import y as z1;
            //        alias y as z2;
            //    }
            //}";

            var input = @"
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

            //var inputA = @"{""issues"":[],""syntaxErrors"":[],""semanticErrors"":[],""warnings"":[],""content"":{""name"":""BlockTest"",""kind"":""PACKAGE"",""ownedElements"":[""f"",""A"",""B"",""C""]}}";


            // SYSTEM.TEXT.JSON

            //var x = JsonSerializer.Deserialize<SysMLInteractiveResult>(inputA, new JsonSerializerOptions()
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    Converters =
            //    {
            //        new JsonStringEnumConverter()
            //    }
            //});


            // NEWTONSOFT

            //var x = new JsonSerializer()
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    Converters =
            //    {
            //        new StringEnumConverter()
            //    }
            //};

            //using TextReader sr = new StringReader(inputA);
            //SysMLInteractiveResult sysMLInteractiveResult = x.Deserialize<SysMLInteractiveResult>(new JsonTextReader(sr));


            // RPC FROM PROCESS

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

            var kernel = new SysMLKernel("SysML");
            var kernelCommandResult = await kernel.SendAsync(new SubmitCode(input), System.Threading.CancellationToken.None);
            var x = kernel.LastSubmissionResult;
            var y = kernel.LastSubmissionSvg;

            return 0;
        }
    }
}