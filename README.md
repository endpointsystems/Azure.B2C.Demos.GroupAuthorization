Vote here to get group memberships added to Azure AD B2C Claims: https://feedback.azure.com/forums/169401-azure-active-directory/suggestions/10123836-get-user-membership-groups-in-the-claims-with-ad-b

# Azure.B2C.Demos.GroupAuthorization

In this project, we demonstrate the ability to verify a user's existence against an Active Directory group from within an Azure Active Directory B2C tenant. To do this, we use a custom authentication service that implements the `Microsoft.AspNetCore.Authorization.IAuthorizationService` interface and uses the [Azure AD Graph API](https://docs.microsoft.com/en-us/previous-versions/azure/ad/graph/api/api-catalog) to access and confirm our user is a member of the specified group. 

This demo is *very* limited in scope, only showing how to build middleware to verify an `[Authorize]` call, but there's no stopping you (or anyone else) from taking things up a notch. 😎

This demo also uses the `HttpClient`-driven approach to development over using `ActiveDirectoryClient` for the simple fact that `ActiveDirectoryClient` isn't supported on Linux.

## Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{

	//register our custom authorization service
    services.AddSingleton<IAuthorizationService,GroupAuthService>();

	//some parts of our application require a user to be a member of the Employers AD group.
	services.AddAuthorization(options => options.AddPolicy("Employers",builder => builder.RequireRole("Employers")));

}
```

## HomeController.cs

```csharp
	[Authorize("Employers")]
	public IActionResult Auth()
	{
	    return View();
	}

```

## For the nitty-gritty

All of the dirty work is done in the [GroupAuthService](https://github.com/endpointsystems/Azure.B2C.Demos.GroupAuthorization/blob/master/AzureB2CWithGroups/Services/GroupAuthService.cs) authorization service. 

Enjoy!

