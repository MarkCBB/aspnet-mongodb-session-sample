ASP.NET core MongoDB session sample 
===================================

This is a sample of an ASP.NET core MVC project that uses MongoDB to store the session data. Contains a demo/test controller and shows how to set up a connection using a connection string or MongoClientSettings object (see class Startup).

You can just clone and run it, but if you prefer to create it by your own from the scratch follow the next steps.

You'll find useful documentation about options, configurations, performance notes (indexing and cleaning) in this [wiki page](https://github.com/MarkCBB/Caching/wiki). **It is strongly recommended to read this documentation.**

# Steps

1. Make sure that you have created the TTL index in your mongoDB instance. [Learn more](https://github.com/MarkCBB/Caching/wiki#usage)

2. Run the following commands in a powershell:
```
dotnet new mvc -au None
dotnet add package Microsoft.AspNetCore.Session --version 2.1.1
dotnet add package MarkCBB.Extensions.Caching.MongoDB --version 2.1.1-rc-t002

dotnet remove package Microsoft.AspNetCore.CookiePolicy

dotnet restore
```
3. In Startup.cs ConfigureServices method.

Remove this bit of code:
```C#
services.Configure<CookiePolicyOptions>(options =>
{
	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
	options.CheckConsentNeeded = context => true;
	options.MinimumSameSitePolicy = SameSiteMode.None;
});
```

Add this bit:

```C#
services.AddDistributedMongoDBCache(o =>
{
	o.ConnectionString = "<enter a valid connection string>";
	o.DatabaseName = "<enter a valid DatabaseName>";
	o.CollectionName = "<enter a valid CollectionName>";
});

services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromSeconds(60);
    o.Cookie.Name = "Session";
    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.None;
});
```
4. in startup.cs Configure method file.

Remove this line:

```C#
app.UseCookiePolicy();
```

Add this line **before** ```app.UseMvc(routes =>...```
```C#
app.UseSession();
```

Session is ready to run, but you may want to set/get values and check that works...

5. Go to HomeController.cs file and replace the function Contact()

```C#
// You may need this using
using Microsoft.AspNetCore.Http;

//(...)
public IActionResult Contact()
{
    var auxInt = 1;
    var sessioNKey = "TotalLoadsInthisSession";
    var session = this.HttpContext.Session;
    var sessionLoads = session.GetInt32(sessioNKey);
    if(sessionLoads.HasValue)
    {
	auxInt = sessionLoads.Value + 1;
    }
    session.SetInt32(sessioNKey, auxInt);
    
    ViewData["TotalViewsMessage"] = auxInt;

    ViewData["Message"] = "Your contact page.";

    return View();
}
```

6. Go to the view Home/Contact.cshtml and add this line 
```C#
total views: @ViewData["TotalViewsMessage"]
```

All set to run the project. Save all files and run in a PowerShell:

```
dotnet run
```

You can check the updated value in the URL https://localhost:5001/home/Contact and in the database querying the collection:

```
SesionRS:PRIMARY> db.SessionData.find().pretty()
{
	"_id" : "223fd10c-7c2f-193a-322c-7ba731711270",
	"SlidingTimeTicks" : NumberLong(600000000),
	"AbsoluteExpirationTimeUtc" : ISODate("0001-01-01T00:00:00Z"),
	"EffectiveExpirationTimeUtc" : ISODate("2018-08-11T20:12:14.836Z"),
	"Value" : BinData(0,"AgAAAYRwkhnMHUMWKG/KmBfKhgoAF1RvdGFsTG9hZHNJbnRoaXNTZXNzaW9uAAAABAAAAAc=")
}
```
