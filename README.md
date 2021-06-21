# Async

AIR's solution for handing operations with late completion.

## Features

At its core this provides interfaces with no return or a single generic return value, for wrapping an operation so that a following action can be scheduled via `.Then`. Beyond dealing with long running, external, or parallel processes, Async can also be used to organise the sequence of actions or the flow of data in one place before sending off those instructions to run.

The package also includes:

- `Immediate`; an async that isn't async. Makes writing tests easy.
- `AsyncProgress`; an async that can return percentage completion.
- `AsyncHandleUnion`; takes multiple Asyncs and only invokes its `Then` when all of the Asyncs it holds have completed.
- `Async.Catch`; an optional method to signal failure of the Async or the `Then`.

## Basic Usage

There are two sides to this;

1. How an Async concretion is made, prepared, and returned via it's interface.
2. How an IAsync is received and bound.

### An object representing a process by exposing IAsync

```csharp
public class Foo
{
    private AsyncHandle _asyncHandle = new AsyncHandle();
    public IAsync Async => _asyncHandle;

    public void FlushProcessResults(List<Datum> data)
    {
        //some long running process
        foreach (var item in data)
        {
            // something
        }
        _asyncHandle.Complete();
    }
}
```

### Configuring a followup action via the `.Then`

```csharp
var asyncHandle = Foo.Async;    //the property from our previous fragment

asyncHandle.Then(() => Notification.Information("Results have Flushed"));
```

Note: If the async has already completed before the `.Then`, the ThenHandler is called immediately.

## Installation

Add to unity via the package manager. Use 'Add git package' with `https://github.com/AnImaginedReality/Async.git`.
