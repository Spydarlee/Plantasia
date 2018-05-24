using System.Collections.Generic;

public static class ExtensionMethods
{
    // -------------------------------------------------------------------------------

    public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (toMin - fromMin) * (toMax - fromMax) + fromMax;
    }

    // -------------------------------------------------------------------------------

    public static void Swap<T>(this List<T> sourceList, int lhs, int rhs)
    {
        T temp = sourceList[lhs];
        sourceList[lhs] = sourceList[rhs];
        sourceList[rhs] = temp;
    }

    // -------------------------------------------------------------------------------
}