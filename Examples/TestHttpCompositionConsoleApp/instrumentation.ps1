PowerShell -NoProfile -Command {
    $folder = 'C:\sources\serviceable-objects\Examples\TestHttpCompositionConsoleApp\bin\Debug\netcoreapp1.0';
    Set-Location -Path $folder;
    Import-Module "$folder\TestHttpCompositionConsoleApp.dll" -Verbose;

    #Get-Help Write-Message;
    Write-Message -Message "hey" -PipeName "testpipe" -NodeId "log-context";

    # Target examples:
    # Run-InstrumentationCommandOnCustomPipe -Pipe XXX -Data "hey" -OtherData "lo"
    # Run-InstrumentationCommandOnNode -Service XXX -ServiceContainer YYY -NodeId log-context -Data "hey" -OtherData "lo"
    # Do-SomethingOnServiceProcess -Service XXX -ServiceContainer YYY (add-node? merge-template? pause-service?, etc)
    # Do-SomethingOnServiceContainerProcess -ServiceContainer YYY (copy-service?, move-service?, update-service?, etc)

    Write-Host "Done.";
}