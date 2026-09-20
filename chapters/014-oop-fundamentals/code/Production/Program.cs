// Program.cs — .NET 10 console app
// A realistic production shape: an OrderService depends only on the
// IPaymentProcessor ABSTRACTION, never on a concrete processor. Which
// concrete implementation actually runs is decided once, at composition
// time (here, hard-coded for simplicity; in a real app, by a DI container
// reading configuration) — the OrderService's own code never changes no
// matter how many payment providers get added later. This is the entire
// architectural payoff of interface-based polymorphism: new behavior
// without touching or recompiling existing, already-tested code.

Console.WriteLine("=== Inside .NET: Episode 15 — production interface-based polymorphism ===\n");

IPaymentProcessor processor = args.Contains("--paypal")
    ? new PayPalProcessor()
    : new CreditCardProcessor();

var orderService = new OrderService(processor);
orderService.PlaceOrder(orderId: "ORD-1001", amount: 249.99m);

// Swapping the concrete implementation requires no change whatsoever to
// OrderService — it only ever calls through the IPaymentProcessor interface.
var orderServiceWithPayPal = new OrderService(new PayPalProcessor());
orderServiceWithPayPal.PlaceOrder(orderId: "ORD-1002", amount: 89.50m);

interface IPaymentProcessor
{
    bool Charge(decimal amount);
}

class CreditCardProcessor : IPaymentProcessor
{
    public bool Charge(decimal amount)
    {
        Console.WriteLine($"  [CreditCard] Charging {amount:C} to card...");
        return true;
    }
}

class PayPalProcessor : IPaymentProcessor
{
    public bool Charge(decimal amount)
    {
        Console.WriteLine($"  [PayPal] Redirecting {amount:C} through PayPal...");
        return true;
    }
}

class OrderService(IPaymentProcessor paymentProcessor)
{
    // OrderService depends on the INTERFACE, never on a concrete class.
    // Every call below goes through interface dispatch (an interface method
    // table lookup, not a class vtable lookup) — see Under the Hood and
    // Performance Notes for exactly why that costs a little more than a
    // plain virtual call, and why that cost is almost always worth paying
    // for this much decoupling.
    private readonly IPaymentProcessor _paymentProcessor = paymentProcessor;

    public void PlaceOrder(string orderId, decimal amount)
    {
        Console.WriteLine($"Placing order {orderId} for {amount:C}...");
        if (_paymentProcessor.Charge(amount))
        {
            Console.WriteLine($"Order {orderId} confirmed.\n");
        }
        else
        {
            Console.WriteLine($"Order {orderId} FAILED — payment declined.\n");
        }
    }
}
