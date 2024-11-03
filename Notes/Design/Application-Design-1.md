# Application Design

There is normally a gulf between the academic theoretical best design and the implemented design for a project.

That gap exists for two reasons:

1. Knowledge constraints.
2. Time constraints.

I can't bridge the time gap, but I can help bridge the knowledge gap.

First, some acquired knowledge:

> Experience in building Blazor applications combined with fundimental good practices drive your design in a certain directions.

## Scoped Services and DbContexts

In Blazor, *Scoped* services exist for the duration of the SPA session.  Unlike a server side web application, where it is restricted to the life of the *HttpRequest*.  A Blazor scoped service could live for minutes or hours.

This has a critical impact on how you use database contexts.  A single scoped *DbContext* will remain open for long periods and at some point two processes will try to access the same context.  

You therefore need to move to transactional *DbContexts* provided from a *DbContextFactory* that are open for milliseconds.  This drives your data pipeline design.  You no longer open editable contexts in EF, make changes and then update the context.  Instead you submit an update to the DbContext and update the context.
