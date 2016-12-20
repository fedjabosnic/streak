# Streak

[![Build status](https://ci.appveyor.com/api/projects/status/upradamdnf1deiq3?svg=true)](https://ci.appveyor.com/project/fedjabosnic/streak)

A high performance general use event store.

Streak allows you to write to and read from *event streams*, called streaks. Streaks are file backed and guarantee event order based on event position numbers and timestamps. When consuming a streak, you can either consume existing events and return, or you can consume existing events and subscribe to future events by specifying the `continuous` flag. In essence, the enumerable will block until more events are available...

> In the current release, it is **not possible** to use the same streak instance for writing and reading, this will corrupt the data and lead to bad stuff. This will be fixed in the next version...

#### Features

- Extremely fast
- Works across threads and processes (even across machines but shhh we didn't say that)

#### TODO

- Transactional inserts
- Compression
- Sharding


### Usage

Install nuget package `Streak`.

Add the relevant using statement:

```
using Streak.Store;
```

To write to a streak:

```
// Open streams to the index and the events (files do not need to already exist)
var index = File.Open(@"c:\streaks\abc.i", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
var events = File.Open(@"c:\streaks\abc.e", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

var streak = new Streak(index, events);

// Save some events
streak.Save(new List<Event>
{
    new Event { Type = "Test.Event", Data = $" Tick: 1", Meta = $" CorrelationId: 1" },
    new Event { Type = "Test.Event", Data = $" Tick: 2", Meta = $" CorrelationId: 2" },
    new Event { Type = "Test.Event", Data = $" Tick: 3", Meta = $" CorrelationId: 3" }
});
```

To read from a streak:

```
// Open streams to the index and the events (files must already exist)
var index = File.Open(@"c:\streaks\abc.i", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
var events = File.Open(@"c:\streaks\abc.e", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

var streak = new Streak(index, events);

// Read some events
foreach (var e in r_streak.Get(from: 10, to: 300, continuous: true))
{
    // Do some cool event processing
}
```
