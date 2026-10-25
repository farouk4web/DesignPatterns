# Strategy Pattern

## 1. Pattern Overview

- **Official name:** Strategy
- **Category:** Behavioral
- **Definition:** Define a family of interchangeable algorithms, encapsulate each one, and make them swappable at runtime so clients can vary behavior independently of the code that uses it.

## 2. What Problem Does It Solve?

Checkout (or similar flows) often need different discount rules: none, seasonal, loyalty, coupon. Without Strategy, those rules tend to pile into one class as `if`/`switch` branches.

**Without the pattern**, code might look like:

```csharp
decimal CalculateFinalTotal(decimal cartTotal, string discountType, decimal couponPercent)
{
    if (discountType == "seasonal")
        return cartTotal * 0.90m;
    if (discountType == "loyalty")
        return cartTotal * 0.80m;
    if (discountType == "coupon")
        return cartTotal * (1 - couponPercent / 100);
    return cartTotal;
}
```

**Disadvantages of that alternative:**

- Every new discount rule edits the same method (harder to maintain, riskier merges).
- Discount logic mixes with checkout orchestration.
- Runtime switching is awkward; unit-testing one rule means dragging the whole method along.
- Open/Closed Principle suffers: open for modification instead of extension.

## 3. When Should We Use It?

**Appropriate when:**

- You have several ways to perform the same responsibility (here: apply a discount).
- The algorithm should be selectable or replaceable at runtime.
- You want each variant isolated in its own class for clarity and testing.

**Often unnecessary when:**

- There is only one algorithm and no expected variation.
- A simple conditional is clearer and will not grow.
- The “strategies” differ only by a single config value that a parameterized method can handle without new types.

## 4. How Does It Work?

**Core idea:** The context owns a strategy reference and delegates the varying work to it. Clients can inject a strategy or replace it later.

In this project:

| Piece | Responsibility |
| --- | --- |
| `IDiscountStrategy` | Strategy interface: `ApplyDiscount(decimal)`. |
| `NoDiscountStrategy`, `SeasonalDiscountStrategy`, `LoyaltyDiscountStrategy`, `CouponDiscountStrategy` | Concrete algorithms. |
| `CheckoutContext` | Context: holds the current strategy, allows `SetStrategy`, computes final total via delegation. |
| `Program` | Client: builds a context and swaps strategies on the same cart. |

**Execution flow:**

1. `Program` creates `CheckoutContext` with `NoDiscountStrategy`.
2. `CalculateFinalTotal(200)` calls `_discountStrategy.ApplyDiscount` → returns `200`.
3. `SetStrategy(new SeasonalDiscountStrategy())` replaces the algorithm.
4. The same `CalculateFinalTotal(200)` now returns `180` (10% off).
5. Further swaps to loyalty (20% off) and a 15% coupon change results without changing `CheckoutContext` itself.

## 5. Code Walkthrough

### Files

- `IDiscountStrategy.cs` — strategy contract
- `CheckoutContext.cs` — context
- `Strategies/NoDiscountStrategy.cs`
- `Strategies/SeasonalDiscountStrategy.cs`
- `Strategies/LoyaltyDiscountStrategy.cs`
- `Strategies/CouponDiscountStrategy.cs`
- `Program.cs` — runtime swap demo

### Strategy interface

```csharp
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal totalAmount);
}
```

Every discount variant implements this one method.

### Context: hold, replace, delegate

```csharp
public class CheckoutContext
{
    private IDiscountStrategy _discountStrategy;

    public CheckoutContext(IDiscountStrategy discountStrategy) { /* null-check + assign */ }

    public void SetStrategy(IDiscountStrategy discountStrategy) { /* null-check + replace */ }

    public decimal CalculateFinalTotal(decimal cartTotal)
    {
        return _discountStrategy.ApplyDiscount(cartTotal);
    }
}
```

- Constructor sets the initial strategy.
- `SetStrategy` enables runtime changes (core Strategy behavior).
- `CalculateFinalTotal` does not know *which* discount runs—only that something implementing `IDiscountStrategy` will.

### Concrete strategies

| Class | Behavior in this sample |
| --- | --- |
| `NoDiscountStrategy` | Returns the amount unchanged. |
| `SeasonalDiscountStrategy` | Multiplies by `0.90m` (10% off). |
| `LoyaltyDiscountStrategy` | Multiplies by `0.80m` (20% off). |
| `CouponDiscountStrategy` | Takes `discountPercentage` and applies `total * (1 - percent/100)`. |

Example:

```csharp
public class LoyaltyDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount * 0.80m; // 20% off
    }
}
```

**Why this follows Strategy:** algorithms are encapsulated, share a common interface, and the context delegates to a replaceable strategy object.

**Learning note:** Discount math is simplified (fixed percentages, console demo only). There is no persistence, tax, shipping, or payment gateway—only interchangeable discount behavior.

## 6. Practical Example

An online store checkout must apply different discounts depending on campaign, membership, or a typed coupon—without rewriting the checkout class each time.

This sample models that with `CheckoutContext` and a `$200` cart. `Program` starts with no discount, then switches to seasonal, loyalty, and a 15% coupon. Expected outputs for `$200`: `$200.00`, `$180.00`, `$160.00`, `$170.00`.

## 7. Advantages and Disadvantages

**Advantages**

- Add a new discount by adding a class, not by growing a giant `switch`.
- Runtime swap via `SetStrategy`.
- Each algorithm is easy to read and unit-test in isolation.
- Context stays focused on orchestration (here: calling apply-discount).

**Disadvantages / trade-offs**

- More types than a small `if` chain.
- Client (or DI) must choose the right strategy.
- Strategies that need lots of shared data can push you toward richer context/parameters.
- Overkill when variation is unlikely.

## 8. Comparison With Similar Patterns

| Pattern | Difference in intent |
| --- | --- |
| **State** | Looks similar (context + interchangeable objects), but State changes behavior based on *internal state transitions*; Strategy is chosen for an *algorithm policy* (often by the client). |
| **Template Method** | Varies steps via inheritance in a fixed skeleton; Strategy varies the whole algorithm via composition. |
| **Simple Factory / Factory Method** | Create the right object; Strategy is about *using* interchangeable behavior. A factory may *supply* a strategy, but that is optional and separate. |
| **Decorator** | Adds responsibilities around an object; Strategy replaces one algorithm with another. |

## 9. Common Mistakes

- Putting all discount `if`s back inside the context (defeats the pattern).
- Forgetting a way to change strategy when runtime switching is a requirement.
- Leaking concrete strategy types all through business logic instead of depending on `IDiscountStrategy`.
- Confusing Strategy with State because both use composition of similar-looking classes.
- Creating a new strategy class for every tiny numeric difference when one parameterized strategy (like `CouponDiscountStrategy`) is enough.

## 10. Interview Preparation

**Q1: What is the Strategy pattern?**  
**A:** Encapsulate interchangeable algorithms behind a common interface and let a context use/replace them without changing its own code.

**Q2: How does this project change behavior at runtime?**  
**A:** `CheckoutContext.SetStrategy(...)` replaces `_discountStrategy`; the next `CalculateFinalTotal` call uses the new algorithm.

**Q3: Strategy vs State?**  
**A:** Strategy selects a policy/algorithm (often externally). State models states and transitions where the object’s condition drives the next behavior.

**Scenario:** “Checkout must support no discount, seasonal sales, loyalty pricing, and percentage coupons, and the rule can change during the session.”  
**Approach:** Define `IDiscountStrategy`, implement each rule as a class, inject into `CheckoutContext`, call `SetStrategy` when the rule changes—as this sample does.

**.NET interview blurb:**  
“Strategy lets me swap algorithms through composition. In this sample, `CheckoutContext` depends on `IDiscountStrategy` and delegates `CalculateFinalTotal` to `ApplyDiscount`. `Program` starts with `NoDiscountStrategy` and uses `SetStrategy` to switch to seasonal, loyalty, and coupon discounts on the same cart without modifying the context class.”

## 11. Key Takeaways

- Strategy = **family of algorithms** + **common interface** + **context that delegates**.
- This demo applies it to checkout discounts.
- `SetStrategy` is what makes runtime interchangeability explicit.
- `NoDiscountStrategy` is a clean default (Null Object–style strategy).
- Prefer Strategy when behavior variants will grow; prefer a simple conditional when they will not.
