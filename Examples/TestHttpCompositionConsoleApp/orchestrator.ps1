$ErrorActionPreference = "Stop"

$location = Split-Path $MyInvocation.MyCommand.Path;

$appRuntime = "netcoreapp1.0";
Set-Location -Path $location;
#Copy-Item ..\..\..\powershell-binaries-$appRuntime\* -Destination . -Force;

$folder = "$location\bin\Debug\$appRuntime";
#$folder = "$location\bin\Release\$appRuntime\win-x64\publish";
Set-Location -Path $folder;
#Move-Item "$location\*.dll" $folder -Force;

# Initialize orchestrator
$serviceOrchestratorGraphTemplate = @{
    GraphNodes=@(
        @{ TypeFullName='Serviceable.Objects.Remote.Composition.ServiceOrchestrator.ServiceOrchestratorContext, Serviceable.Objects.Remote, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'; Id='server-orchestrator-context' },
        @{ TypeFullName='Serviceable.Objects.Instrumentation.Server.InstrumentationServerContext, Serviceable.Objects.Instrumentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'; Id='instrumentation-context' }
    );
    GraphVertices=@();
    Registrations=@();
};

$serviceGraphTemplate = "
{
GraphNodes: [
    { TypeFullName:'TestHttpCompositionConsoleApp.Contexts.Http.OwinHttpContext, TestHttpCompositionConsoleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', Id:'server-context' },
    { TypeFullName:'TestHttpCompositionConsoleApp.Contexts.Queues.QueueContext, TestHttpCompositionConsoleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', Id:'queue-context' },
    { TypeFullName:'TestHttpCompositionConsoleApp.Contexts.ConsoleLog.ConsoleLogContext, TestHttpCompositionConsoleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', Id:'console-log-context' },
    { TypeFullName:'Serviceable.Objects.IO.NamedPipes.Server.NamedPipeServerContext, Serviceable.Objects.IO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', Id:'namedpipes-log-instrumentation-context' },
    { TypeFullName:'Serviceable.Objects.Instrumentation.Server.InstrumentationServerContext, Serviceable.Objects.Instrumentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', Id:'instrumentation-context' },
],
GraphVertices: [
    { FromId:'server-context', ToId:'queue-context', },
    { FromId:'queue-context', ToId:'console-log-context',  },
    { FromId:'namedpipes-log-instrumentation-context', ToId:'console-log-context',  },
],
Registrations: [
    { Type:'TestHttpCompositionConsoleApp.ConfigurationSources.ServiceContainerConfigurationSource, TestHttpCompositionConsoleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', WithDefaultInterface:true },
],
}
";

$orchTemplateJson = ConvertTo-Json @{
    OrchestratorOverrideTemplate = $serviceOrchestratorGraphTemplate;
    ServiceOrchestratorConfiguration = @{
        EntryAssemblyFullPath = "C:\sources\serviceable-objects\Examples\TestHttpCompositionConsoleApp\bin\Debug\netcoreapp1.0\TestHttpCompositionConsoleApp.dll";
        OrchestratorName = "orchestrator-X";
        # UseChildProcesses = $true; # When this is true if parent process closes, all children are as well
        GraphTemplatesDictionary = @{
            "service-X"= $serviceGraphTemplate;
        };
        InBindingsPerService = @{
            "service-X" = @(
                @{
                    ContextTypeName = "TestHttpCompositionConsoleApp.Contexts.Http.OwinHttpContext, TestHttpCompositionConsoleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    ScaleSetBindings = @(
                        @{ Host = "localhost"; Port = "5000" },
                        @{ Host = "localhost"; Port = "5001" }
                    )
                }
            )
        }
    }
} -Depth 100;

$orchTemplateJson = $orchTemplateJson -replace "\`"", "'";
$orchTemplateJson = $orchTemplateJson -replace "\r\n", "";
$orchTemplateJson = $orchTemplateJson -replace "\\r\\n", "";
$orchTemplateJson = $orchTemplateJson -replace "\s+", " ";

echo $orchTemplateJson;

Start-Process -FilePath "c:\Program Files\dotnet\dotnet.exe" -Verb Open -ArgumentList "$folder\TestHttpCompositionConsoleApp.dll", "$orchTemplateJson";
