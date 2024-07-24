# Blazor.Demo

Blazr.Demo contains solutions that implement my latest **Blazr Application Framework**.

The framework is loosely based on various coding ideologies.

1. *Clean Design*: code is split into domains.  Each domain is a separate project that enforces rigid clean design dependancies.
1. A Read Only CQS data pipeline.  All data pipeline objects are read only.  

1. ....

The repository contains:

1. Two demo solutions:
   1. Blazr.Weather - demonstrates the framework using the standard *WeatherForecast* template.
   1. Blazr.Invoicing - demonstrates some key complex object concepts using the classic *Invoice* aggregate context.
   
1. A set of Libraries that provide key functionalty.  These are all available as Nuget packages.

1. Documentation [in the form of MD files] as a set of notes scattered through the respository in the relevant places.

The two demo solutions implement various UI frontends.  These include Vanilla Bootstrap, MudBlazor and FluentUI in both Server and WASM deployments.

The Weather Solution is complete bar some final testing and clean up.

The Invoice Solution is has a completed data pipeline and Blazor Server FluentUI frontend.  The other UI's are work in progress.

The documentation is a work-in-progress.

Take and use what you wish.

**Please**:  

> If you're using it in a commercial setting, get your organisation to make a donation to a charity for it's usage.

> It doesn't matter how much, or where.  It will make a difference, and I will have made a [very small] contribution to making the world a better place.
