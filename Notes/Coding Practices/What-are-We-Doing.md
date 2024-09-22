# What Are We Doing

All too often we jump into a project and start writing code without really considering what we're doing.

To demonstrate, here's a *Jump in and Write* component:

```csharp
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: <FluentBadge Appearance="Appearance.Neutral">@currentCount</FluentBadge></p>

<FluentButton Appearance="Appearance.Accent" @onclick="IncrementCount">Click me</FluentButton>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

It's very recognisable if you've deployed a Blazor solution.

This looks a little different [and at first sight, a little more complex].

```csharp
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: <FluentBadge Appearance="Appearance.Neutral">@this.Presenter.State.Count</FluentBadge></p>

<FluentButton Appearance="Appearance.Accent" @onclick="this.Presenter.Increment">Click me</FluentButton>

@code {
    private readonly CounterPresenter Presenter = new();

    public readonly record struct CounterState(int Count);

    public sealed class CounterPresenter
    {
        public CounterState State { get; private set; } = new(0);

        public void Increment()
        {
            this.State = State with { Count = this.State.Count + 1 };
        }
    }
}
```

The counter state is separated out into a readonly value object.

The logic [that mutates the state] moves from the component to a presentation layer object.
   
The component is now only responsible for UI activity.

Yes, the old component is less complex, and in it's simplicity, it's purpose and logic are clear.  But, how many real components are that simple?  Components that display/mutate complex data can soon grow to several hundred lines of code: clarity and purpose fade into the wall of code.  *You can't see the wood for the trees.*

This implementation demonstrates some important practices:

1. The *Separation of Concerns* principle.
2. Taming *Rampant Mutation*.  There's only one controlled way to change the state of the counter data.
3. Separating the data state from the logic makes persisting the state relatively trivial.

