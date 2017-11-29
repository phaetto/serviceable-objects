$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    $folder = "$location\bin\Debug\netcoreapp1.0";
    Set-Location -Path $folder;
    Import-Module "$folder\TestHttpCompositionConsoleApp.dll";

    #Get-Help Write-Message;
    #Get-Help Enqueue-Message;

    $message = Dequeue-Message -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "LALALA- queue-context";
    
    Write-Host "Message 1: '$message'";

    Write-Message -Data @{ Message = "Awesome instrumentation" } -PipeName "service-X.namedpipes-log-instrumentation-context";
    Write-Message -Data @{ Message = "Awesome instrumentation - again!" } -PipeName "service-X.namedpipes-log-instrumentation-context";

    Enqueue-Message @{ "Data" = "a message" } -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "queue-context";
    $message = Dequeue-Message -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "queue-context";
    
    Write-Host "Message 2: '$message'";

    Write-Host " --- Done --- ";
} -args $location;