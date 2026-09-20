Console.WriteLine("=== Chapter 018: Records & Pattern Matching (Production) ===\n");

Console.WriteLine("--- High-Throughput Event-Sourced Aggregate via Records & Pattern Matching ---\n");

// 1. Initial State
var account = new BankAccount(Guid.NewGuid(), "Biplab Sarker", 0m, AccountStatus.Active, 0);
Console.WriteLine($"Initial State: {account}");

// 2. Stream of Domain Events (Immutable Records)
IDomainEvent[] events =
{
    new FundsDeposited(account.Id, 1000.00m, DateTime.UtcNow),
    new FundsWithdrawn(account.Id, 250.00m, DateTime.UtcNow),
    new FundsDeposited(account.Id, 500.00m, DateTime.UtcNow),
    new AccountFrozen(account.Id, "Suspicious international IP detected", DateTime.UtcNow),
    new FundsWithdrawn(account.Id, 100.00m, DateTime.UtcNow) // Should be rejected by pattern matching
};

// 3. Evolving Aggregate State with Non-Destructive Mutation (`with`)
foreach (var evt in events)
{
    var transition = account.Apply(evt);
    if (transition.Success)
    {
        account = transition.NewState;
        Console.WriteLine($"[APPLIED] {evt.GetType().Name} -> New Balance: {account.Balance:C} (Version {account.Version})");
    }
    else
    {
        Console.WriteLine($"[REJECTED] {evt.GetType().Name}: {transition.ErrorMessage}");
    }
}

Console.WriteLine($"\nFinal Immutable State: {account}");

// -------------------------------------------------------------
// Domain Model (Records & Pattern Matching)
// -------------------------------------------------------------

public enum AccountStatus { Active, Frozen, Closed }

public record BankAccount(
    Guid Id,
    string Owner,
    decimal Balance,
    AccountStatus Status,
    int Version)
{
    public (bool Success, BankAccount NewState, string? ErrorMessage) Apply(IDomainEvent domainEvent)
    {
        return (this, domainEvent) switch
        {
            // Case 1: Active account receiving a valid deposit
            ({ Status: AccountStatus.Active }, FundsDeposited { Amount: > 0 } dep)
                => (true, this with { Balance = Balance + dep.Amount, Version = Version + 1 }, null),

            // Case 2: Active account with sufficient funds withdrawing money
            ({ Status: AccountStatus.Active } acc, FundsWithdrawn w) when acc.Balance >= w.Amount
                => (true, this with { Balance = Balance - w.Amount, Version = Version + 1 }, null),

            // Case 3: Overdraft attempt on active account
            ({ Status: AccountStatus.Active }, FundsWithdrawn w)
                => (false, this, $"Insufficient funds: Attempted to withdraw {w.Amount:C}, current balance {Balance:C}"),

            // Case 4: Freezing an active account
            ({ Status: AccountStatus.Active }, AccountFrozen f)
                => (true, this with { Status = AccountStatus.Frozen, Version = Version + 1 }, null),

            // Case 5: Attempting any operation on a frozen account
            ({ Status: AccountStatus.Frozen }, _)
                => (false, this, $"Operation rejected: Account {Id} is frozen!"),

            // Fallback: Unknown or invalid event
            _ => (false, this, "Unrecognized domain event or invalid state transition")
        };
    }
}

// Immutable Domain Events (Positional Records)
public interface IDomainEvent { Guid AggregateId { get; } DateTime Timestamp { get; } }

public record FundsDeposited(Guid AggregateId, decimal Amount, DateTime Timestamp) : IDomainEvent;
public record FundsWithdrawn(Guid AggregateId, decimal Amount, DateTime Timestamp) : IDomainEvent;
public record AccountFrozen(Guid AggregateId, string Reason, DateTime Timestamp) : IDomainEvent;
