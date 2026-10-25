# Factory Method Pattern (Simple Factory Demo)

## 1. Pattern Overview

- **Official name (folder / learning goal):** Factory Method
- **Category:** Creational
- **Short definition (canonical Factory Method):** Define an interface for creating an object, but let subclasses decide which class to instantiate.

**Important for this repository:** The code in this folder is a **Simple Factory** (also called a static factory): one `PaymentProcessorFactory` with a `switch` on a provider name. It teaches the *idea* of centralizing creation behind an abstraction (`IPaymentProcessor`), but it is **not** the full GoF Factory Method structure (no Creator hierarchy with overridden factory methods). That difference is explained below so the learning stays accurate.

## 2. What Problem Does It Solve?

Client code often needs one of several related types (PayPal, Stripe, Fawry payment processors) but should not be littered with `new ConcreteType()` and provider-specific conditionals everywhere.

**Without a factory**, checkout or billing code might look like:

```csharp
IPaymentProcessor processor;
if (provider == "paypal")
    processor = new PayPalProcessor();
else if (provider == "stripe")
    processor = new StripeProcessor();
// repeated in many places...
processor.ProcessPayment(amount);
```

**Disadvantages:**

- Creation logic is duplicated across the app.
- Adding a provider means hunting down every `if`/`switch`.
- Callers become tightly coupled to concrete classes.

## 3. When Should We Use It?

**Appropriate when (Simple Factory, as in this project):**

- You create one of several implementations of the same interface based on a parameter (name, enum, config).
- You want a single place that maps “provider key → concrete type.”
- Callers should depend only on `IPaymentProcessor`.

**Canonical Factory Method is a better fit when:**

- Each product type is owned by a parallel creator subclass.
- Framework or base-class code must create products without knowing concrete classes, via an overridable `Create...()` method.

**Often unnecessary when:**

- You have only one implementation and no variation.
- A DI container already resolves the correct service by configuration.
- The mapping logic would be a single trivial `new` at one call site.

## 4. How Does It Work?

**Core idea in this project:** Ask a factory for an `IPaymentProcessor` by provider string; use the interface without caring which concrete class you received.

| Piece | Responsibility |
| --- | --- |
| `IPaymentProcessor` | Product interface: `ProcessPayment(decimal amount)`. |
| `PayPalProcessor` / `StripeProcessor` / `FawryProcessor` | Concrete products; each prints a provider-specific message. |
| `PaymentProcessorFactory` | Static factory that maps provider names to concrete types. |
| `Program` | Client: requests processors and processes sample amounts. |

**Execution flow:**

1. `Program` calls `PaymentProcessorFactory.CreateProcessor("paypal")`.
2. Factory normalizes the string with `ToLower()` and matches `"paypal"`.
3. Factory returns `new PayPalProcessor()` as `IPaymentProcessor`.
4. `Program` calls `ProcessPayment(100.00m)` on the interface.
5. The same flow repeats for `"stripe"` and `"fawry"`.
6. An unknown provider throws `ArgumentException("Invalid payment provider")`.

## 5. Code Walkthrough

### Files

- `Processors/IPaymentProcessor.cs` — product interface
- `Processors/PayPalProcessor.cs`
- `Processors/StripeProcessor.cs`
- `Processors/FawryProcessor.cs`
- `PaymentProcessorFactory.cs` — creation logic
- `Program.cs` — demo client

### Product interface

```csharp
public interface IPaymentProcessor
{
    void ProcessPayment(decimal amount);
}
```

All providers share this contract so clients stay decoupled from PayPal/Stripe/Fawry types.

### Concrete product example

```csharp
public class PayPalProcessor : IPaymentProcessor
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing ${amount} payment through PayPal.");
    }
}
```

These implementations are intentionally simple for learning (console output only—no real payment APIs).

### Simple factory

```csharp
public static class PaymentProcessorFactory
{
    public static IPaymentProcessor CreateProcessor(string provider)
    {
        return provider.ToLower() switch
        {
            "paypal" => new PayPalProcessor(),
            "stripe" => new StripeProcessor(),
            "fawry" => new FawryProcessor(),
            _ => throw new ArgumentException("Invalid payment provider"),
        };
    }
}
```

**Why this is useful:** creation and provider mapping live in one type; `Program` only sees `IPaymentProcessor`.

**Why this differs from canonical Factory Method:** there is no abstract Creator with subclasses like `PayPalPaymentService` overriding `CreateProcessor()`. A static switch-based factory is the **Simple Factory** idiom. Many tutorials still place it under a “Factory Method” folder name because both are creational and hide `new`, but the GoF structures differ.

### Client usage in `Program.cs`

```csharp
var paypal = PaymentProcessorFactory.CreateProcessor("paypal");
paypal.ProcessPayment(100.00m);
```

## 6. Practical Example

An e-commerce checkout must charge a customer through different gateways depending on region or user choice: PayPal, Stripe, or Fawry.

This project models that decision with `CreateProcessor(provider)` and then calls `ProcessPayment`. It does **not** implement HTTP calls, credentials, refunds, or webhooks—only the creational mapping and a stub `ProcessPayment` for each provider.

## 7. Advantages and Disadvantages

**Advantages (of this Simple Factory approach)**

- Centralizes `new` and provider mapping.
- Clients depend on `IPaymentProcessor`, not concrete classes.
- Easy to demo Open/Closed at the *usage* site: processing code does not change when you only swap which processor instance you received.

**Disadvantages / trade-offs**

- The factory itself must change when you add a provider (the `switch` grows).
- String keys are error-prone (`"Paypal"` works after `ToLower()`, but typos still fail at runtime).
- Not polymorphic creation via subclassing (true Factory Method).
- Static factories can be harder to mock than instance-based creators or DI.

## 8. Comparison With Similar Patterns

| Pattern | Intent vs this code |
| --- | --- |
| **Factory Method (GoF)** | Creator subclasses override a factory method; no central switch required in the base. This repo uses a central switch instead. |
| **Abstract Factory** | Creates *families* of related products (e.g., payment + receipt + refund APIs for one vendor). Here you create one product type only. |
| **Builder** | Assembles one complex object step by step. Here you pick an already-complete strategy-like processor in one call. |
| **Strategy** | Focuses on interchangeable *behavior* at runtime. Factories often *supply* strategy implementations; Strategy is about using them. |

## 9. Common Mistakes

- Calling the folder “Factory Method” while implementing only a static switch factory—and not recognizing the difference (this README calls that out on purpose).
- Returning concrete types from the factory instead of the interface.
- Scattering the same `switch` outside the factory again.
- Using magic strings without docs/tests; prefer enums or constants in production code.
- Putting business workflow inside the factory instead of only creation.

## 10. Interview Preparation

**Q1: What is the difference between Simple Factory and Factory Method?**  
**A:** Simple Factory uses one class (often static) to choose the product, commonly with `switch`/`if`. Factory Method lets subclasses of a Creator decide which product to instantiate by overriding a factory method.

**Q2: What does the client depend on in this sample?**  
**A:** `IPaymentProcessor` and `PaymentProcessorFactory`—not `PayPalProcessor` / `StripeProcessor` / `FawryProcessor` directly in the processing calls.

**Q3: How would you evolve this toward GoF Factory Method?**  
**A:** Introduce an abstract payment service/creator with `CreateProcessor()`, then subclasses such as a PayPal-specific creator that returns `new PayPalProcessor()`, moving the decision from a central switch to polymorphism.

**Scenario:** “Checkout must support PayPal, Stripe, and Fawry, and new providers will appear later. Processing code should not hardcode `new`.”  
**Approach:** Depend on `IPaymentProcessor`; isolate creation (Simple Factory now, or Creator subclasses / DI later). Keep each provider’s charge logic inside its processor class.

**.NET interview blurb:**  
“This project centralizes payment processor creation in `PaymentProcessorFactory`, which returns `IPaymentProcessor` implementations for PayPal, Stripe, and Fawry. Architecturally it’s a Simple Factory—great for learning decoupling from `new`—while true Factory Method would push creation into overridable creator subclasses. In production .NET I’d often register processors in DI and resolve by key instead of a static switch.”

## 11. Key Takeaways

- Goal: **decouple clients from concrete payment processor classes**.
- This folder implements **Simple Factory**, named Factory Method for learning organization.
- Products: `PayPalProcessor`, `StripeProcessor`, `FawryProcessor` behind `IPaymentProcessor`.
- `CreateProcessor(string)` maps provider names; unknown names throw.
- Implementations are stubs (console output) so you can focus on the creational design.
- Next learning step: refactor into Creator subclasses if you want a textbook Factory Method.
