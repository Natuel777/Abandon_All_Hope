using System.Collections.Generic;
using UnityEngine;

public static class ShuffleExtension
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
}
