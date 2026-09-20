using System.Linq.Expressions;
using System.Reflection;

Console.WriteLine("=== Chapter 017: Reflection & Expression Trees (Production) ===\n");

// -------------------------------------------------------------
// Pattern: High-Performance Compiled Expression Object Mapper
// -------------------------------------------------------------
Console.WriteLine("--- 1. High-Speed Compiled Object Mapper (AutoMapper Core Pattern) ---");

var sourceUser = new UserEntity
{
    Id = 1001,
    Username = "biplabsarker",
    Email = "biplab@example.com",
    TotalOrders = 48,
    IsActive = true
};

// Map entity to DTO using dynamically compiled JIT mapper
UserDto dto = FastMapper.Map<UserEntity, UserDto>(sourceUser);

Console.WriteLine($"Mapped DTO successfully:");
Console.WriteLine($"  Id: {dto.Id}");
Console.WriteLine($"  Username: {dto.Username}");
Console.WriteLine($"  Email: {dto.Email}");
Console.WriteLine($"  TotalOrders: {dto.TotalOrders}");

// 2. Proving zero-reflection repeated execution
Console.WriteLine("\n--- 2. High-Throughput Batch Execution ---");
const int iterations = 100_000;
var sw = System.Diagnostics.Stopwatch.StartNew();
for (int i = 0; i < iterations; i++)
{
    _ = FastMapper.Map<UserEntity, UserDto>(sourceUser);
}
sw.Stop();
Console.WriteLine($"Mapped {iterations:N0} objects in {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds / iterations:F2} μs / map)!");

// -------------------------------------------------------------
// Supporting Types
// -------------------------------------------------------------

public class UserEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public bool IsActive { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
}

/// <summary>
/// Compiles a typed delegate (TSource source) => new TTarget { PropA = source.PropA, ... }
/// at runtime using Expression Trees, completely eliminating reflection after the initial compile!
/// </summary>
public static class FastMapper
{
    public static TTarget Map<TSource, TTarget>(TSource source)
        where TSource : class
        where TTarget : class, new()
    {
        return MapperCache<TSource, TTarget>.MapFunc(source);
    }

    private static class MapperCache<TSource, TTarget>
        where TSource : class
        where TTarget : class, new()
    {
        public static readonly Func<TSource, TTarget> MapFunc = CompileMapper();

        private static Func<TSource, TTarget> CompileMapper()
        {
            var sourceParam = Expression.Parameter(typeof(TSource), "source");
            var bindings = new List<MemberBinding>();

            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name);

            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var targetProp in targetProps)
            {
                if (sourceProps.TryGetValue(targetProp.Name, out var sourceProp) &&
                    targetProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                {
                    var sourcePropAccess = Expression.Property(sourceParam, sourceProp);
                    var binding = Expression.Bind(targetProp, sourcePropAccess);
                    bindings.Add(binding);
                }
            }

            // new TTarget() { Prop1 = source.Prop1, ... }
            var memberInit = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            var lambda = Expression.Lambda<Func<TSource, TTarget>>(memberInit, sourceParam);

            return lambda.Compile();
        }
    }
}
