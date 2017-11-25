PowerShell -NoProfile -Command {
    $folder = 'C:\sources\serviceable-objects\Examples\TestHttpCompositionConsoleApp\bin\Debug\netcoreapp1.0';
    Set-Location -Path $folder;
    Import-Module "$folder\TestHttpCompositionConsoleApp.dll" -Verbose;

    Write-Message -message "hey";

    Write-Host "Done.";
}