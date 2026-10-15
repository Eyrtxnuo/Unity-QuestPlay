 using System.Runtime.CompilerServices;
 using Unity.Mathematics;

 public class FloatQuantizer
{

    private const uint MASK20 = 0xFFF00000u;

    // riduce la precisione del float azzerando i bit finali della mantissa.
    // Lo uso per ridurre gli step possibili per evitare di generare infiniti valori per WaitsCache 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float QuantizeFloat(float value)
    {
        uint raw = math.asuint(value);
        return math.asfloat(raw & MASK20);
    }
}