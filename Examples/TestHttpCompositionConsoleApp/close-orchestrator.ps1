$ErrorActionPreference = "Stop"
$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    Set-Location -Path $location;

    Import-Module "$location\bin\Debug\netcoreapp1.0\Serviceable.Objects.Instrumentation.dll";

    Start-Sleep -s 2;
    Close-Orchestrator -ServiceOrchestrator "orchestrator-X";
} -args $location;