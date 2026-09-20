using System.Reflection;

Console.WriteLine("=== Chapter 015: Delegates & Events (Example) ===");

// 1. A delegate is a type-safe object pointer to a method + target instance
var calculator = new Calculator(100);

// Pointing to an instance method
Func<int, int> addFunc = calculator.Add;

// Pointing to a static method
Func<int, int, int> multiplyFunc = Calculator.Multiply;

Console.WriteLine($"calculator.Add(25) = {addFunc(25)}");
Console.WriteLine($"Calculator.Multiply(4, 5) = {multiplyFunc(4, 5)}");

// 2. Under the hood: inspecting delegate fields via reflection
Console.WriteLine("\n--- Inspecting Delegate Internals ---");
PrintDelegateInternals("addFunc (Instance)", addFunc);
PrintDelegateInternals("multiplyFunc (Static)", multiplyFunc);

// 3. Basic Event Publisher-Subscriber
Console.WriteLine("\n--- Publisher-Subscriber Event Pattern ---");
var sensor = new TemperatureSensor();
var display = new ThermostatDisplay("Living Room");

// Subscribe
sensor.TemperatureChanged += display.OnTemperatureChanged;

// Simulate readings
sensor.UpdateTemperature(21.5);
sensor.UpdateTemperature(24.0);

// Unsubscribe
sensor.TemperatureChanged -= display.OnTemperatureChanged;
sensor.UpdateTemperature(26.0); // display should not react

static void PrintDelegateInternals(string label, Delegate del)
{
    var target = del.Target;
    var method = del.Method;
    
    // Internal fields in MulticastDelegate
    var methodPtrField = typeof(Delegate).GetField("_methodPtr", BindingFlags.NonPublic | BindingFlags.Instance);
    var targetField = typeof(Delegate).GetField("_target", BindingFlags.NonPublic | BindingFlags.Instance);
    
    var methodPtrVal = methodPtrField?.GetValue(del);
    var targetVal = targetField?.GetValue(del);

    Console.WriteLine($"[{label}]");
    Console.WriteLine($"  Target Type: {(target != null ? target.GetType().Name : "null (Static Method)")}");
    Console.WriteLine($"  Target Instance: {target ?? "null"}");
    Console.WriteLine($"  Method: {method.DeclaringType?.Name}.{method.Name}");
    Console.WriteLine($"  _methodPtr (native): 0x{((IntPtr)(methodPtrVal ?? IntPtr.Zero)):X}");
}

public class Calculator
{
    private readonly int _bias;
    public Calculator(int bias) => _bias = bias;

    public int Add(int value) => _bias + value;
    public static int Multiply(int a, int b) => a * b;
}

public class TemperatureSensor
{
    // Standard EventHandler<T> pattern
    public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

    public void UpdateTemperature(double newTemp)
    {
        Console.WriteLine($"[Sensor] Reading: {newTemp:F1}°C");
        TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(newTemp));
    }
}

public class TemperatureChangedEventArgs : EventArgs
{
    public double Temperature { get; }
    public TemperatureChangedEventArgs(double temperature) => Temperature = temperature;
}

public class ThermostatDisplay
{
    private readonly string _name;
    public ThermostatDisplay(string name) => _name = name;

    public void OnTemperatureChanged(object? sender, TemperatureChangedEventArgs e)
    {
        Console.WriteLine($"  [{_name}] Display updated to {e.Temperature:F1}°C");
    }
}
