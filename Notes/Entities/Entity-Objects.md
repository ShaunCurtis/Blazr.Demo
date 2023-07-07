# Entity Objects

## Identity

All entity objects have some form of unique identifier.  I use a `Guid`, so define an interface that all entity objects implement:

```csharp
public interface IGuidIdentity 
{ 
    public Guid Uid { get; }
}
```

I don't need to use specific naming  such as `CutomerUid` or `ProductUid` in the native objects.  The value is unique.  I do use specific naming where the Uid is a foreign key:  In the Invoice object, `CustomerUid` identifies the customer.

## State

All entity objects also maintain state.

 - Is this a new record I've just created and haven't yet saved?
 - Is this a deleted invoice item in an invoice I haven't yet saved?

```csharp
public interface IStateEntity 
{ 
    public int StateCode { get; }
}
```
 
There are four base states for an entity object: 

1. An existing record.
2. A mutated record i.e. it has the same identity as the record in the data store, but some or all of the data has changed.
3. A deleted record i.e. it still exists in the data store, but is marked in-memory for deletion.
4. A new record that currently only exists in-memory and has not yet bwen saved to the data store.


The common states are defined as constants in `StateCodes`. 

```
public static class StateCodes
{
    public const int Record = 1;
    public const int New = 0;
    public const int Delete = int.MinValue;

    public static bool IsUpdate(int value) => value > 0;
    public static bool IsNew(int value) => value == 0;
    public static bool IsDeleted(int value) => value == Delete;
}
```

## Immutability

All data objects are immutable.  You can't change them once they've been created.

They are implemented as records with `{get; init;}` property definitions.


## Example

Here's the `Customer` object.

It's:

1. sealed - it's not designed for inheritance
2. A record - immutable
3. Implements `IGuidIdentity`.
4. Implements `IStateEntity`.
5. Implements `ICommandEntity`.  The Command data pipeline can persist it to the data store independantly.  Data objects that are part of an `Aggregate` entity can't be persisted separately. 
6. `Uid` is defined as the data store key.
7. `StateCode` is not persisted to the data store.  It's a in-memory data object property for tracking state outside the data store.

```csharp
public sealed record Customer : IGuidIdentity, IStateEntity, ICommandEntity
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;
    [NotMapped] public int StateCode { get; init; } = 1;
    public string CustomerName { get; init; } = "Not Set";
}
```

## Editing

Data objects are edited through a record edit context: a class that contains the properties that can be edited.

`BlazrEditContext` provides the boilerplate code [see the Record Edit Context section for more detail].

The data class implementation:

1. Defines thw editable properties
2. The `[TrackState]` attribute tells the state tracker which properties to track.
3. `MapToContext` maps the provided record to the edit context properties.
4. `MapToRecord` outputs a record based on the current content of the edit context.

The edit context maintains an internal copy of the original record and provides the edit state through thw `IsDirty` property. 


```csharp
public sealed class CustomerEditContext : BlazrEditContext<Customer>
{
    [TrackState] public string CustomerName { get; set; } = string.Empty;

    public CustomerEditContext() : base() { }

    protected override void MapToContext(Customer record)
    {
        this.Uid = record.Uid;
        internalStateCode = record.StateCode;
        this.CustomerName = record.CustomerName;
    }

    protected override Customer MapToRecord()
        => new()
        {
            Uid = this.Uid,
            StateCode = this.StateCode,
            CustomerName = this.CustomerName,
        };
}
```

Edit forms create record edit contexts based on the record to edit.  Controls point to and update the edit context properties.  The record is saved by passing the record from `AsRecord` into the data pipeline. 

## Validation

I use Fluent Validation.  Each edit object has a validator.

Here's the Customer validator.  Note it's against the `CustomerEditContext`, not the `Customer` record.

```csharp
public class CustomerValidator : AbstractValidator<CustomerEditContext>
{
    public CustomerValidator()
    {
        this.RuleFor(p => p.CustomerName)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
```




