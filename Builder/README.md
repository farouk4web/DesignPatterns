# Builder Pattern

## 1. Pattern Overview

- **Official name:** Builder
- **Category:** Creational
- **Definition:** Separates the construction of a complex object from its representation so the same construction process can create objects step by step, with clear control over required and optional parts.

## 2. What Problem Does It Solve?

Some objects have many fields. Some are required, some optional, and construction order or validation matters. A single constructor with many parameters becomes hard to read and easy to misuse (telescoping constructors, wrong argument order, half-initialized objects).

**Without the pattern**, email creation might look like:

```csharp
var email = new Email(
    Guid.NewGuid(),
    DateTime.Now,
    EmailReason.Welcome,
    "recipient@example.com",
    "Subject",
    "<p>Body</p>",
    attachments);
```

Or callers might set mutable properties one by one with no validation until much later.

**Disadvantages of that alternative:**

- Long constructors are hard to read and easy to break when parameters are reordered.
- Optional parts (like attachments) force overloads or nullable parameters everywhere.
- Validation is scattered or missing until the object is already in use.
- The product type may need a large public mutable surface just to support construction.

## 3. When Should We Use It?

**Appropriate when:**

- An object has many construction steps or optional pieces.
- You want a fluent, readable construction API.
- You need validation before the final object is produced (`Build()`).
- The product should be immutable (or mostly immutable) after creation.

**Often unnecessary when:**

- The object has only a few required fields and a simple constructor is clear.
- You only need object initializers for a flat DTO with no rules.
- A factory method that returns a fully built simple object is enough.

## 4. How Does It Work?

**Core idea:** Accumulate construction state in a builder, then produce the final product in one `Build()` call after validation.

In this project:

| Piece | Responsibility |
| --- | --- |
| `Email` | Product: immutable email data (`init` properties). |
| `EmailAttachment` | Supporting model for attachment file name and content. |
| `EmailReason` | Enum describing why the email is sent. |
| `EmailBuilder` | Fluent API that stores temporary fields and builds `Email`. |
| `Program` | Demonstrates chaining setters and calling `Build()`. |

**Collaboration / execution flow:**

1. `Program` creates `EmailBuilder`.
2. Fluent methods (`SetTo`, `SetSubject`, …) store values on the builder and return `this` for chaining.
3. `Build()` validates required fields (`To`, `Subject`, `HTMLBody`).
4. If valid, `Build()` creates a new `Email` with those values.
5. `Program` reads properties from the finished `Email`.

## 5. Code Walkthrough

### Files

- `EmailBuilder.cs` — builder
- `Models/Email.cs` — product
- `Models/EmailAttachment.cs` — attachment model
- `Enums/EmailReason.cs` — reason enum
- `Program.cs` — usage demo

### Product: `Email`

```csharp
internal class Email
{
    public Guid Id { get; init; }
    public DateTime SentOn { get; init; }
    public EmailReason Reason { get; init; }
    public string To { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string HTMLBody { get; init; } = string.Empty;
    public IReadOnlyList<EmailAttachment>? Attachments { get; init; }
}
```

`init` accessors encourage immutability after construction: once built, callers are not meant to reshape the email through ordinary setters.

### Fluent setters on `EmailBuilder`

Each setter updates private builder state and returns the builder:

```csharp
public EmailBuilder SetTo(string to)
{
    _To = to;
    return this;
}
```

Attachments are stored as a read-only list:

```csharp
public EmailBuilder SetAttachments(List<EmailAttachment> emailAttachments)
{
    _Attachments = emailAttachments.AsReadOnly();
    return this;
}
```

### Validation in `Build()`

```csharp
public Email Build()
{
    if (string.IsNullOrWhiteSpace(_To))
        throw new InvalidOperationException("To is required");
    // Subject and Body similarly required...

    return new Email { /* mapped fields */ };
}
```

**Why this follows Builder:** construction is stepwise and fluent; the product is assembled only at the end; required rules live in one place (`Build()`).

**Learning note / simplification:** This is a practical fluent Builder for one product type. The GoF pattern sometimes also includes a Director that orchestrates a fixed construction sequence. This repository does not implement a Director—`Program` drives the steps directly, which is common in real C# fluent builders.

## 6. Practical Example

You need to compose a welcome email: recipient, subject, HTML body, optional attachments, metadata (`Id`, `SentOn`, `Reason`).

That matches this sample: `Program` builds a welcome email with two attachments using `EmailBuilder`, then prints the result. There is no SMTP sending layer here—the focus is constructing a valid `Email` object safely and readably.

## 7. Advantages and Disadvantages

**Advantages**

- Readable construction for objects with many fields.
- Required-field validation at a single checkpoint.
- Product can stay immutable while the builder holds mutable draft state.
- Optional parts (attachments) do not explode the constructor signature.

**Disadvantages / trade-offs**

- Extra type (`EmailBuilder`) to maintain alongside the product.
- Callers must remember to call `Build()`.
- Builder instances are typically not thread-safe if reused concurrently.
- Without a Director, construction sequences can still vary between call sites.

## 8. Comparison With Similar Patterns

| Pattern | Difference in intent |
| --- | --- |
| **Factory / Factory Method** | Choose *which* concrete type to create; usually one-shot creation, not multi-step field assembly. |
| **Abstract Factory** | Create *families* of related objects; not step-by-step building of one complex object. |
| **Prototype** | Create by copying an existing instance; Builder constructs from parts/parameters. |
| **Telescoping constructors** | Same problem space, but Builder scales more cleanly as optional parameters grow. |

Use Builder when the pain is **complex assembly**. Use a factory when the pain is **choosing a subtype**.

## 9. Common Mistakes

- Skipping validation in `Build()` and allowing invalid products.
- Making the product fully mutable and bypassing the builder afterward.
- Reusing one builder for multiple products without clearing state (stale field bugs).
- Confusing Builder with Factory just because both “create objects.”
- Overusing Builder for tiny types that need only a constructor.

## 10. Interview Preparation

**Q1: What problem does Builder solve?**  
**A:** Readable, controlled construction of complex objects, especially with optional parts and validation.

**Q2: Why return `this` from setter methods?**  
**A:** To enable fluent chaining: `builder.SetTo(...).SetSubject(...).Build()`.

**Q3: How is Builder different from Factory Method?**  
**A:** Factory Method focuses on which subclass to instantiate; Builder focuses on how to assemble a complex object’s data step by step.

**Scenario:** “An `Email` needs To/Subject/Body required, attachments optional, and should be immutable after creation.”  
**Approach:** Use a builder that collects fields, validates required ones in `Build()`, and maps into an `init`-only product—as this project does.

**.NET interview blurb:**  
“Builder separates how we construct a complex object from the final representation. In this sample, `EmailBuilder` provides a fluent API and validates required fields before creating an immutable `Email`. That’s clearer than a long constructor and keeps validation in one place.”

## 11. Key Takeaways

- Builder = **step-by-step construction** + final `Build()`.
- This demo builds an `Email` with required fields and optional attachments.
- Fluent setters return `this`; `Build()` enforces rules.
- No Director class here—construction is driven by `Program`, which is a common real-world simplification.
- Prefer Builder for complex products; prefer simple constructors for simple types.
