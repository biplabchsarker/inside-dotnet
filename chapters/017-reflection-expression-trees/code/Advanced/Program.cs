using System.Linq.Expressions;
using System.Reflection;

Console.WriteLine("=== Chapter 017: Reflection & Expression Trees (Advanced) ===\n");

// 1. High-Performance Property Accessor Factory via Expression Trees
Console.WriteLine("--- 1. Compiled Expression Property Accessor vs Reflection ---");

PropertyInfo prop = typeof(Product).GetProperty("Price")!;

// Traditional Reflection (slow):
var product = new Product("Mechanical Keyboard", 129.99m);
object? valRefl = prop.GetValue(product);
Console.WriteLine($"[Reflection] PropertyInfo.GetValue: {valRefl}");

// Compiled Expression Getter (Fast: compiles to direct native read):
Func<Product, decimal> compiledGetter = CreateGetter<Product, decimal>(prop);
decimal valFast = compiledGetter(product);
Console.WriteLine($"[Expression Tree] Compiled Getter: {valFast}");

// 2. Dynamic Query Predicate Construction (How EF Core / LINQ works)
Console.WriteLine("\n--- 2. Building Dynamic Predicates at Runtime ---");

var inventory = new List<Product>
{
    new Product("USB-C Cable", 15.00m),
    new Product("Wireless Mouse", 49.99m),
    new Product("4K Monitor", 399.00m),
    new Product("Ergonomic Chair", 280.00m)
};

// Dynamically construct: p => p.Price > 40.00m
Expression<Func<Product, bool>> priceFilter = BuildGreaterThanFilter<Product, decimal>("Price", 40.00m);
Console.WriteLine($"Constructed Predicate: {priceFilter}");

var filtered = inventory.Where(priceFilter.Compile()).ToList();
Console.WriteLine("Filtered Products (Price > 40.00):");
foreach (var item in filtered)
{
    Console.WriteLine($"  - {item.Name}: {item.Price:C}");
}

// 3. ExpressionVisitor: Inspecting and Rewriting Expressions
Console.WriteLine("\n--- 3. ExpressionVisitor (AST Transformation) ---");
var visitor = new BinaryOpReplacer();
var modifiedExpr = visitor.Visit(priceFilter);
Console.WriteLine($"Rewritten Expression: {modifiedExpr}");

static Func<TTarget, TProp> CreateGetter<TTarget, TProp>(PropertyInfo property)
{
    ParameterExpression param = Expression.Parameter(typeof(TTarget), "instance");
    MemberExpression member = Expression.Property(param, property);
    return Expression.Lambda<Func<TTarget, TProp>>(member, param).Compile();
}

static Expression<Func<T, bool>> BuildGreaterThanFilter<T, TVal>(string propertyName, TVal threshold)
{
    ParameterExpression param = Expression.Parameter(typeof(T), "p");
    MemberExpression propAccess = Expression.Property(param, propertyName);
    ConstantExpression constant = Expression.Constant(threshold, typeof(TVal));
    BinaryExpression comparison = Expression.GreaterThan(propAccess, constant);
    return Expression.Lambda<Func<T, bool>>(comparison, param);
}

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}

public class BinaryOpReplacer : ExpressionVisitor
{
    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node.NodeType == ExpressionType.GreaterThan)
        {
            // Rewrite > to >=
            return Expression.GreaterThanOrEqual(node.Left, node.Right);
        }
        return base.VisitBinary(node);
    }
}
