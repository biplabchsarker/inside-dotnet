using System.Linq.Expressions;
using System.Reflection;

Console.WriteLine("=== Chapter 017: Reflection & Expression Trees (Example) ===\n");

// 1. Classical Reflection: Metadata Inspection and Late Binding
Console.WriteLine("--- 1. Metadata Inspection & Late Binding ---");
var order = new Order(101, "Acme Corp", 450.00m);

Type type = order.GetType();
Console.WriteLine($"Type: {type.FullName} (Assembly: {type.Assembly.GetName().Name})");

Console.WriteLine("Properties:");
foreach (var prop in type.GetProperties())
{
    Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name} = {prop.GetValue(order)}");
}

MethodInfo? discountMethod = type.GetMethod("ApplyDiscount");
if (discountMethod != null)
{
    // MethodInfo.Invoke requires object[] parameters and boxes arguments!
    object? result = discountMethod.Invoke(order, new object[] { 10m });
    Console.WriteLine($"Late-bound ApplyDiscount(10%) -> New Total: {result:C}");
}

// 2. Expression Trees: Code Represented as an Abstract Syntax Tree (AST)
Console.WriteLine("\n--- 2. Building and Inspecting an Expression Tree ---");

// Constructing AST: (decimal total, decimal discountPercent) => total - (total * (discountPercent / 100m))
ParameterExpression paramTotal = Expression.Parameter(typeof(decimal), "total");
ParameterExpression paramDiscount = Expression.Parameter(typeof(decimal), "discountPercent");

ConstantExpression hundred = Expression.Constant(100m, typeof(decimal));
BinaryExpression division = Expression.Divide(paramDiscount, hundred);
BinaryExpression discountAmount = Expression.Multiply(paramTotal, division);
BinaryExpression finalCalculation = Expression.Subtract(paramTotal, discountAmount);

Expression<Func<decimal, decimal, decimal>> lambdaExpr = 
    Expression.Lambda<Func<decimal, decimal, decimal>>(finalCalculation, paramTotal, paramDiscount);

Console.WriteLine($"Expression Tree Body: {lambdaExpr.Body}");
Console.WriteLine($"Expression Node Type: {lambdaExpr.NodeType}");
Console.WriteLine($"Parameters: {string.Join(", ", lambdaExpr.Parameters.Select(p => $"{p.Type.Name} {p.Name}"))}");

// 3. Compiling the AST to Native JIT Code
Console.WriteLine("\n--- 3. Compiling AST to Executable Machine Code ---");
Func<decimal, decimal, decimal> compiledFunc = lambdaExpr.Compile();

decimal discountedResult = compiledFunc(450.00m, 10m);
Console.WriteLine($"Compiled Expression Result: {discountedResult:C}");

public class Order
{
    public int OrderId { get; set; }
    public string Customer { get; set; }
    public decimal Total { get; set; }

    public Order(int orderId, string customer, decimal total)
    {
        OrderId = orderId;
        Customer = customer;
        Total = total;
    }

    public decimal ApplyDiscount(decimal percent)
    {
        Total -= Total * (percent / 100m);
        return Total;
    }
}
