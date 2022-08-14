# Authentication and Authorization


## Identity

The term Identity defines the currently authenticated context.  It's not a *user* because it may be another computer running a process.  *Identity* is specific to the SPA instance.  You may hav two browser windows open logged in as two different users.

To manage the SPA *Identity* we define an interface for a DI service `IIdentityService`.  This:

1. Maintains a `ClaimsPrincipal' that represents the current *Identity*.
2. Provides a method to authenticate an *Identity* against a data store.
3. Provides an event that is raised when the *Identity* is changed.

`ClaimsPrincipal' is the primary DotNetCore class that represents an *Identity*.



  