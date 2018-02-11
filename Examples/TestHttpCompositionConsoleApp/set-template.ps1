$ErrorActionPreference = "Stop"
$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    Import-Module "$location\bin\Debug\netcoreapp1.0\Serviceable.Objects.Instrumentation.dll";

    Set-Template -ServiceOrchestrator "orchestrator-X" -Data @{
        GraphNodes = @(
            @{
                TypeFullName = 'Serviceable.Objects.Instrumentation.Server.InstrumentationServerContext, Serviceable.Objects.Instrumentation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';
                Id = 'instrumentation-context';
            }
        );
        GraphVertices = @();
        Registrations = @();
        TemplateName = 'template-Y';
    };

    Set-Template -ServiceOrchestrator "orchestrator-X" -Data @{
        GraphNodes = @(
            @{
                TypeFullName='TestHttpCompositionConsoleApp.Contexts.ConsoleLog.ConsoleLogContext, TestHttpCompositionConsoleApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null';
                Id='console-log-context'
            }
        );
        GraphVertices = @();
        Registrations = @();
        ServiceName = 'service-Y';
    };
    
    New-Service -ServiceOrchestrator "orchestrator-X" -Data @{ ServiceName='service-Y'; TemplateName = 'template-Y' };
    Watch-ServiceUntilStarts -ServiceOrchestrator "orchestrator-X" -ServiceName "service-Y" -TimeoutInMilliseconds 60000;
    Close-Service -ServiceOrchestrator "orchestrator-X" -ServiceName 'service-Y';
    
} -args $location;