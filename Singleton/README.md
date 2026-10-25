# Singleton Pattern

## 1. Pattern Overview

- **Official name:** Singleton
- **Category:** Creational
- **Definition:** Ensures a class has only one instance and provides a global point of access to that instance.

## 2. What Problem Does It Solve?

Some objects should exist only once in an application: configuration, a shared cache, a logging gateway, or a connection manager. If every part of the code creates its own copy, you get inconsistent state, wasted resources, and hard-to-track bugs.

**Without the pattern**, code often looks like this:

```csharp
var config1 = new AppConfig();
var config2 = new AppConfig();
// config1 and config2 are different objects with separate state
```

**Disadvantages of that approach:**

- Multiple instances can drift out of sync (different counters, settings, or caches).
- Construction may be expensive or side-effecting (loading config files, opening connections).
- Callers must coordinate who “owns” the shared instance, which is easy to get wrong.

## 3. When Should We Use It?

**Appropriate when:**

- Exactly one shared instance is a real requirement (not just convenience).
- You need a well-known access point (for example `AppConfig.Instance`).
- Lazy or controlled initialization matters (create only when first used).

**Often unnecessary or overengineering when:**

- Dependency Injection already gives you a single registered service (`AddSingleton` in ASP.NET Core).
- The “shared” data is just immutable settings that can be passed as a normal object.
- You mainly want global variables; Singleton can hide dependencies and make testing harder.

## 4. How Does It Work?

**Core idea:** Hide the constructor, keep one static reference, and expose it through a single access point.

In this project:

| Piece | Responsibility |
| --- | --- |
| `AppConfig` | Holds app settings and shared mutable state (`UseCounter`). |
| Private constructor | Prevents `new AppConfig()` from outside the class. |
| `_instance` | Stores the one instance (or `null` until first use). |
| `_lock` | Synchronizes creation so only one thread creates the instance. |
| `Instance` property | Global access point; creates the object on first get if needed. |

**Collaboration / flow in this demo:**

1. `Program` asks for `AppConfig.Instance`.
2. If `_instance` is null, the getter takes a lock and creates `AppConfig` once.
3. A second call to `AppConfig.Instance` returns the same object.
4. Mutations through either variable (`config1` / `config2`) affect the same underlying instance.

## 5. Code Walkthrough

### Files

- `AppConfig.cs` — Singleton implementation
- `Program.cs` — demo that proves both references are the same instance

### Private constructor and shared fields

```csharp
private AppConfig()
{
    AppName = "Singleton Demo";
    Version = "1.0.0";
    Environment = "Development";
}

private static readonly Lock _lock = new();
private static AppConfig? _instance;
```

The constructor initializes read-only-style config values. Outside code cannot call `new AppConfig()`.

### Double-checked locking (active implementation)

```csharp
public static AppConfig Instance
{
    get
    {
        if (_instance is null)
        {
            lock (_lock)
                _instance ??= new AppConfig();
        }
        return _instance;
    }
}
```

- First null check avoids locking on every access after creation.
- Inside the lock, `_instance ??= new AppConfig()` creates the object only if still null (second check).
- This project uses .NET’s `Lock` type for the synchronization object.

### Alternative styles (commented in `AppConfig.cs`)

The file also documents two other common approaches for learning:

1. **Eager initialization** — create the instance when the type is loaded.
2. **`Lazy<T>`** — marked in comments as a preferred modern option; the runtime handles thread-safe lazy creation.

Those alternatives are commented out so the active demo focuses on double-checked locking.

### Demo in `Program.cs`

```csharp
AppConfig? config1 = AppConfig.Instance;
AppConfig? config2 = AppConfig.Instance;

config1?.UseCounter = ++config1.UseCounter;
config2?.UseCounter = ++config2.UseCounter;

Console.WriteLine($"Is ReferenceEquals : " + ReferenceEquals(config1, config2));
```

Both counters increment the same field, and `ReferenceEquals` should print `True`.

**Why this follows Singleton:** one instance, inaccessible public construction, and a single static access point.

**Learning note:** This is a classic manual Singleton. In real ASP.NET Core apps, the DI container’s singleton lifetime often replaces hand-rolled Singletons.

## 6. Practical Example

Imagine a small console or desktop app that needs shared runtime configuration: app name, version, and environment. Many modules read those values; only one in-memory copy should exist.

This repository models that with `AppConfig`. `Program` obtains the instance twice and updates `UseCounter` to show that both variables share one object—the same idea as several modules reading one config object in a larger app.

What is *not* implemented here: loading settings from files, environment variables, or a DI container. The sample focuses on instance uniqueness.

## 7. Advantages and Disadvantages

**Advantages**

- Guarantees a single shared instance.
- Lazy creation is possible (create on first use).
- Simple global access for small demos and utilities.

**Disadvantages / trade-offs**

- Global access can hide dependencies and couple code tightly.
- Harder to unit test (static state; need care to reset or avoid).
- Easy to misuse as a dumping ground for unrelated global state.
- Thread-safety must be designed deliberately if you write the pattern by hand.

## 8. Comparison With Similar Patterns

| Pattern | Intent vs Singleton |
| --- | --- |
| **Factory Method / Simple Factory** | Creates *objects of varying types*; does not limit how many instances exist. |
| **Builder** | Constructs a *complex object step by step*; each `Build()` usually yields a new instance. |
| **DI Singleton lifetime** | Same “one instance” goal, but the container owns lifetime and injection instead of a static `Instance` property. |

Use Singleton when *identity uniqueness* is the requirement. Use factories/builders when *how you create* objects is the concern.

## 9. Common Mistakes

- Forgetting a private constructor (callers can still `new` extra instances).
- Non-thread-safe lazy init in multi-threaded apps.
- Treating Singleton as “global variables with a fancy name.”
- Holding large disposable resources forever without a clear lifetime strategy.
- Using Singleton when constructor injection / DI would keep design cleaner.

## 10. Interview Preparation

**Q1: What does the Singleton pattern guarantee?**  
**A:** A class has at most one instance, with a global (or well-known) access point.

**Q2: How do you make Singleton thread-safe in C#?**  
**A:** Options include eager static initialization, `Lazy<T>`, or careful locking (as in this project’s double-checked approach). Prefer the simplest correct option for the runtime.

**Q3: Why can Singleton be problematic in tests?**  
**A:** Static shared state persists across tests, creates hidden coupling, and makes it harder to substitute fakes unless you redesign for interfaces/DI.

**Scenario:** “Multiple services need the same application settings object, and creating it twice would reload files and diverge.”  
**Approach:** Ensure one shared instance (manual Singleton or DI singleton registration), inject or access it consistently, keep it focused on configuration—not unrelated business logic.

**.NET interview blurb:**  
“Singleton ensures one instance with controlled access. In this sample, `AppConfig` uses a private constructor and a static `Instance` getter with double-checked locking. In modern .NET apps I usually prefer registering a singleton in the DI container instead of a static `Instance`, unless there’s a clear reason for a classic Singleton.”

## 11. Key Takeaways

- Singleton = **one instance + controlled access**.
- This repo’s `AppConfig` demonstrates **lazy, thread-aware** creation via double-checked locking.
- Commented alternatives show **eager** and **`Lazy<T>`** styles.
- Prefer DI singletons in larger .NET apps; use a manual Singleton only when it truly fits.
- Prove uniqueness with shared mutable state and `ReferenceEquals`, as `Program.cs` does.
