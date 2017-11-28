$location = Split-Path $MyInvocation.MyCommand.Path;

PowerShell -NoProfile -Command {
    param (
        [String] $location
    )

    $folder = "$location\bin\Debug\netcoreapp1.0";
    Set-Location -Path $folder;
    Import-Module "$folder\TestHttpCompositionConsoleApp.dll" -Verbose;

    #Get-Help Write-Message;
    Write-Message -Message "Awesome instrumentation" -PipeName "container-X.service-X.testpipe";
    Write-Message -Message "Awesome instrumentation - again" -PipeName "container-X.service-X.testpipe";


    # Target examples:
    # Run-InstrumentationCommandOnCustomPipe -Pipe XXX -Data "hey" -OtherData "lo"
    # Run-InstrumentationCommandOnNode -Service XXX -ServiceContainer YYY -NodeId log-context -Data "hey" -OtherData "lo"
    # Do-SomethingOnServiceProcess -Service XXX -ServiceContainer YYY (add-node? merge-template? pause-service?, etc)
    # Do-SomethingOnServiceContainerProcess -ServiceContainer YYY (copy-service?, move-service?, update-service?, etc)

    Write-Host "Done.";
} -args $location