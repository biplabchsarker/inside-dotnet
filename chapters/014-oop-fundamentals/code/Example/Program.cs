// Program.cs — .NET 10 console app
// Demonstrates: encapsulation (private fields behind properties), inheritance,
// and virtual dispatch — the same object called through a base-class reference
// always runs its OWN override, never the base's, regardless of which type
// the calling code declared the variable as.

Console.WriteLine("=== Inside .NET: Episode 15 — OOP Fundamentals demo ===\n");

// Encapsulation: Balance is only ever changed through Deposit/Withdraw,
// never assigned directly — the class protects its own invariant (never negative).
var account = new SavingsAccount(100m, interestRate: 0.02m);
account.Deposit(50m);
account.Withdraw(30m);
Console.WriteLine($"Account balance: {account.Balance:C}");

Console.WriteLine();

// Polymorphism: every element is stored and iterated as the BASE type,
// but each call to Speak() runs the ACTUAL (derived) type's override.
List<Animal> zoo = [new Dog("Rex"), new Cat("Whiskers"), new Animal("Generic Animal")];

foreach (var animal in zoo)
{
    // The declared type of `animal` here is Animal for every element —
    // yet the output differs per element. That's virtual dispatch:
    // the CLR looks up the method to run from the OBJECT's own method table,
    // not from the type of the reference used to call it.
    animal.Speak();
}

abstract class BankAccount
{
    // Encapsulation: the field is private; the outside world can only
    // observe or change it through the methods/properties this class exposes.
    private decimal _balance;

    protected BankAccount(decimal openingBalance) => _balance = openingBalance;

    public decimal Balance => _balance;

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        _balance += amount;
    }

    public virtual void Withdraw(decimal amount)
    {
        if (amount > _balance) throw new InvalidOperationException("Insufficient funds.");
        _balance -= amount;
    }
}

class SavingsAccount(decimal openingBalance, decimal interestRate) : BankAccount(openingBalance)
{
    private readonly decimal _interestRate = interestRate;

    // override replaces the SAME vtable slot BankAccount.Withdraw occupies —
    // any code holding a BankAccount reference to THIS object will run this
    // version, never the base one. That's the whole point of `virtual`.
    public override void Withdraw(decimal amount)
    {
        var fee = amount * 0.001m; // small withdrawal fee, savings-account-specific
        base.Withdraw(amount + fee);
    }
}

class Animal(string name)
{
    protected string Name { get; } = name;

    public virtual void Speak() => Console.WriteLine($"{Name} makes a generic animal sound.");
}

class Dog(string name) : Animal(name)
{
    public override void Speak() => Console.WriteLine($"{Name} says: Woof!");
}

class Cat(string name) : Animal(name)
{
    public override void Speak() => Console.WriteLine($"{Name} says: Meow!");
}
