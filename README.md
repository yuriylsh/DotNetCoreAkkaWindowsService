# .Net Core 2.1 Windows Service using Akka.Net 

An example of creating Windows service using .Net Core 2.1

Using Akka.Net system as a background service.

## Resources

* [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1)
* [Microsoft's sample for GenericHost](https://github.com/aspnet/Hosting/tree/release/2.1/samples/GenericHostSample)
* [Host ASP.NET Core in a Windows Service](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.1)
* [Current Directory For Windows Service Is Not What You Expect](https://haacked.com/archive/2004/06/29/current-directory-for-windows-service-is-not-what-you-expect.aspx/)

## Notes
The sample is contains a single background service.

Akka.Net system is set up to use Serilog for logging, Autofac for dependency injection.

## Service installation

Assuming we are using PowerShell, the project is cloned into ```C:\code\sample\``` and the project was compiled like this:
```PowerShell
PS> dotnet publish C:\code\sample\DotNetCoreAkkaWindowsService.csproj -c Release -o C:\services\DotNetCoreAkkaWindowsService
```

### Create service
```PowerShell
PS> sc.exe create DotNetCoreAkkaService binPath= "C:\services\DotNetCoreAkkaWindowsService\DotNetCoreAkkaWindowsService.exe" start= delayed-auto
```
This will create service called DotNetCoreAkkaService with startup type Automatic (Delayed Start). Notice spacing around = sign, it's intended and has to be that way. Refer to documentation for sc.exe for more options.

### Stop service
```PowerShell
PS> sc.exe stop DotNetCoreAkkaService
```

### Delete service
```PowerShell
PS> sc.exe stop DotNetCoreAkkaService; sc.exe delete DotNetCoreAkkaService
```