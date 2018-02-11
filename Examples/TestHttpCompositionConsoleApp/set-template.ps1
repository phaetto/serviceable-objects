$ErrorActionPreference = "Stop"
$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    Import-Module "$location\bin\Debug\netcoreapp1.0\Serviceable.Objects.Instrumentation.dll";

    Set-Template -ServiceOrchestrator "orchestrator-X" -Data @{
        GraphNodes = @();
        GraphVertices = @();
        Registrations = @();
        ServiceName = 'Example-Service';
    };
} -args $location;