$env:DOTNET_ROOT = "C:\Users\HP\Downloads\dotnet-install-temp"
$env:Path = "$env:DOTNET_ROOT;$env:Path"
& "$env:DOTNET_ROOT\dotnet.exe" run --project CarRental.WebApi/CarRental.WebApi.csproj
