$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    $folder = "$location\bin\Debug\netcoreapp1.0";
    Set-Location -Path $folder;
    Import-Module "$folder\TestHttpCompositionConsoleApp.dll";

    Get-Help Write-Message;
    #Get-Help Enqueue-Message;

    # Invalid
    #Write-Message -Data @{ Message = "Awesome instrumentation" } -PipeName "service-X.namedpipes-instrumentation-context";
    
    Write-Message -Data @{ Message = "Awesome instrumentation" } -PipeName "service-X.namedpipes-log-instrumentation-context";
    Write-Message -Data @{ Message = "Awesome instrumentation - again!" } -PipeName "service-X.namedpipes-log-instrumentation-context";

    Enqueue-Message @{ "Data" = "a message" } -ServiceName "service-X" -NodeId "namedpipes-instrumentation-context" -ContextId "queue-context";
    $message = Dequeue-Message -ServiceName "service-X" -NodeId "namedpipes-instrumentation-context" -ContextId "queue-context";
    
    # target:
    #$message = Dequeue-Message -ServiceName "service-X" -ContextId "queue-context";

    Write-Host "Message: '$message'";

    Write-Host " --- Done --- ";
} -args $location;