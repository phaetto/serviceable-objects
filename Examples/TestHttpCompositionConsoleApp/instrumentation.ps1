$ErrorActionPreference = "Stop"

$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    $appRuntime = "netcoreapp1.0";
    Set-Location -Path $location;
    #Copy-Item ..\..\..\powershell-binaries-$appRuntime\* -Destination . -Force;

    $folder = "$location\bin\Debug\$appRuntime";
    #$folder = "$location\bin\Release\$appRuntime\win-x64\publish";
    Set-Location -Path $folder;
    Move-Item "$location\*.dll" $folder -Force;

    Import-Module "$folder\TestHttpCompositionConsoleApp.dll";

    #Get-Help Write-Message;
    #Get-Help Enqueue-Message;

    Dequeue-Message -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "queue-context";

    # This is an error
    #$message = Dequeue-Message -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "LALALA- queue-context";
    #$message = if ($queueItem -eq $null) { "<was null>" } else { $queueItem.Data };
    #Write-Host "Error got: '$message'";

    Write-Message -Data @{ Message = "Awesome instrumentation" } -PipeName "service-X.namedpipes-log-instrumentation-context";
    Write-Message -Data @{ Message = "Awesome instrumentation - again!" } -PipeName "service-X.namedpipes-log-instrumentation-context";

    Enqueue-Message @{ "Data" = "a message" } -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "queue-context";
    $queueItem = Dequeue-Message -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "queue-context";
    $message = if ($queueItem -eq $null) { "<was null>" } else { $queueItem.Data };

    Write-Host "Message got: '$message'";

    $queueItem = Dequeue-Message -ServiceOrchestrator "orchestrator-X" -ServiceName "service-X" -ContextId "queue-context";
    $message = if ($queueItem -eq $null) { "<was null>" } else { $queueItem.Data };
    
    Write-Host "Message got: '$message'";

    Write-Host " --- Done --- ";
} -args $location;