# Streak

[![Build status](https://ci.appveyor.com/api/projects/status/upradamdnf1deiq3?svg=true)](https://ci.appveyor.com/project/fedjabosnic/streak)

A high performance file-backed event stream library.

Streak allows you to write to and read from *event streams*, called streaks, which are essentially append-only logs.

A streak is single writer and multiple reader (both of which can be local or remote). As well as enumerating over existing events in a streak, readers can also read from the streak in a continuous manner, which essentially means blocking until more events are available.


> The original use case for streak was as a file-backed store-and-forward layer for a group of services receiving 10,000s of messages per second. We are now also hoping to use streak as the core storage mechanism for a new event store.

#### Features

- Extremely fast
  - Streak uses sequential reads/writes to its backing files
  - Run the performance tests locally to see what speed you get
- Works across boundaries with no need for additional locking or remoting mechanisms
  - Can be used for cross thread/process communication
  - Relies on existing file semantics
- Replication built in
  - Built in replication mechanisms ensure data is replicated as required (see demo)

> We're just getting started here - lots of features planned and hopefully progress will be swift


#### TODO

- Performance improvements
- Beautiful developer api
- Transactional inserts
- Compression
- Sharding


### Usage

Install nuget package `Streak`.


```
// Create a writable streak
var streak = new Streak(@"c:\streaks\abc", writer: true);

// Assign some writable replicas
var replica1 = new Streak(@"d:\streaks\abc", writer: true);
var replica2 = new Streak(@"\\server\c$\streaks\abc", writer: true);
var replica3 = new Streak(@"\\fileshare\streaks\abc", writer: true);

// Setup replication (async by default)
streak.ReplicateTo(replica1);
streak.ReplicateTo(replica2);
streak.ReplicateTo(replica3);

// Save some events
streak.Save(new List<Event>
{
    new Event { Type = "Test.Event", Data = $" Tick: 1", Meta = $" CorrelationId: 1" },
    new Event { Type = "Test.Event", Data = $" Tick: 2", Meta = $" CorrelationId: 2" },
    new Event { Type = "Test.Event", Data = $" Tick: 3", Meta = $" CorrelationId: 3" }
});

// Open a readable streak to one of the replicas
var reader = new Streak(@"\\fileshare\streaks\abc");

// Receive and process events continuously as they arrive
foreach (var e in reader.Get(from: 1000, to: 5000, continuous: true))
{
    // Do some cool event processing
}

```
