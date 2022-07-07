# View Services

## Entities

Services are based on *Entities*:  in the Blazor template *WeatherForecast* is an entity.

For each entity there are four, with one or more optional, view services:

1. ListServices - handles getting collections of items.
2. RecordServices - handles getting indivual records.
3. EditServices - handles CRUD operartions on a record.
4. EntityService - handles custom operations associated with an entity.
5. FKServices - handles foreign Key operstions where the entity is used as a foreign key in other entities.

There's also an `Entity` class, derived from `IEntity`. for each entity to act as a unique identifier for the entity in DI service instances.  We'll see this in action later.

The services are split this way to obey the 'S' of the SOLID Principles- *Single Reposibility Principle*.

