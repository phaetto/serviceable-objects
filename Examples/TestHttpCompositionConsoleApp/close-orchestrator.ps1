
PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    Import-Module "C:\sources\serviceable-objects\Examples\TestHttpCompositionConsoleApp\bin\Debug\netcoreapp1.0\Serviceable.Objects.Instrumentation.dll";

    Start-Sleep -s 2;
    Close-Orchestrator -ServiceOrchestrator "orchestrator-X";
}