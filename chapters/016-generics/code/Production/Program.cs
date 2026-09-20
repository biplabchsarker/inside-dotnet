Console.WriteLine("=== Chapter 016: Generics Under the Hood (Production) ===\n");

// -------------------------------------------------------------
// Pattern 1: High-Performance Generic Type Cache (Zero-Lookup Dictionary)
// -------------------------------------------------------------
Console.WriteLine("--- 1. Generic Type Cache (Zero-Cost Type Metadata Lookup) ---");

// Accessing metadata without ConcurrentDictionary hashing:
Console.WriteLine($"Order metadata: ID={TypeMetadata<Order>.TypeId}, Name={TypeMetadata<Order>.TypeName}");
Console.WriteLine($"Customer metadata: ID={TypeMetadata<Customer>.TypeId}, Name={TypeMetadata<Customer>.TypeName}");
Console.WriteLine($"Order metadata (second access, purely static pointer read): ID={TypeMetadata<Order>.TypeId}");

// -------------------------------------------------------------
// Pattern 2: Zero-Allocation Constrained Generic Object Pool
// -------------------------------------------------------------
Console.WriteLine("\n--- 2. High-Throughput Zero-Allocation Generic Buffer Pool ---");

var pool = new StructBufferPool<BufferSegment>(capacity: 4);

using (var leased = pool.Rent())
{
    leased.Value.Write(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF });
    Console.WriteLine($"  Leased buffer has {leased.Value.Length} bytes written.");
} // Disposed here -> automatically resets and returns to pool!

using (var leasedAgain = pool.Rent())
{
    Console.WriteLine($"  Leased buffer from pool: Cleaned? {leasedAgain.Value.Length == 0}");
}

// -------------------------------------------------------------
// Supporting Types
// -------------------------------------------------------------

public class Order { }
public class Customer { }

/// <summary>
/// Uses the CLR's generic static field partitioning to achieve O(1) direct memory reads
/// for type metadata, completely eliminating lock contention and dictionary hash lookups.
/// </summary>
public static class TypeMetadata<T>
{
    public static readonly int TypeId;
    public static readonly string TypeName;

    static TypeMetadata()
    {
        TypeName = typeof(T).Name;
        TypeId = TypeIdGenerator.Next();
        Console.WriteLine($"  [JIT Init] Registered TypeMetadata<{TypeName}> with ID={TypeId}");
    }
}

internal static class TypeIdGenerator
{
    private static int _counter;
    public static int Next() => Interlocked.Increment(ref _counter);
}

public interface IResettable
{
    void Reset();
}

public struct BufferSegment : IResettable
{
    private readonly byte[] _buffer = new byte[1024];
    public int Length { get; private set; }

    public BufferSegment() { }

    public void Write(ReadOnlySpan<byte> data)
    {
        data.CopyTo(_buffer.AsSpan(Length));
        Length += data.Length;
    }

    public void Reset()
    {
        Length = 0;
        Array.Clear(_buffer, 0, _buffer.Length);
    }
}

public class StructBufferPool<T> where T : struct, IResettable
{
    private readonly T[] _pool;
    private int _count;
    private readonly object _lock = new();

    public StructBufferPool(int capacity)
    {
        _pool = new T[capacity];
        for (int i = 0; i < capacity; i++)
        {
            _pool[i] = new T();
        }
        _count = capacity;
    }

    public LeasedStruct<T> Rent()
    {
        lock (_lock)
        {
            if (_count > 0)
            {
                _count--;
                return new LeasedStruct<T>(this, _pool[_count]);
            }
        }
        return new LeasedStruct<T>(this, new T());
    }

    internal void Return(T item)
    {
        // Zero-boxing constrained call! JIT devirtualizes item.Reset()
        item.Reset();
        lock (_lock)
        {
            if (_count < _pool.Length)
            {
                _pool[_count] = item;
                _count++;
            }
        }
    }
}

public readonly ref struct LeasedStruct<T> where T : struct, IResettable
{
    private readonly StructBufferPool<T> _owner;
    public ref T Value => ref System.Runtime.CompilerServices.Unsafe.AsRef(in _value);
    private readonly T _value;

    public LeasedStruct(StructBufferPool<T> owner, T value)
    {
        _owner = owner;
        _value = value;
    }

    public void Dispose()
    {
        _owner.Return(_value);
    }
}
