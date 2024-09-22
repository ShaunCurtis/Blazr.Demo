Command/Query Seperation

*CQRS* is a data pipeline pattern, but it's basic premis can be applied more widely.

A *Command* method returns either status information or nothing.  Commands mutate state, they **NEVER** return data.

A *Query* method returns data.  Queries **NEVER** mutate state.  They have no **NO SIDE EFFECTS**.

If you apply this pattern to all your methods, you will create cleaner code with fewer bugs.
