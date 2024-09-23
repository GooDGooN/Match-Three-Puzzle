
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Utility
{
    #region PickRandom
    public static T PickRandom<T>(T[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }

    public static T PickRandom<T>(T[] arr, params T[] exceptArr )
    {
        var newArr = arr.Except(exceptArr).ToArray();
        return newArr[Random.Range(0, newArr.Length)];
    }

    public static int PickRandom(int includeMin, int excludeMax)
    {
        var newArr = new int[Mathf.Abs(excludeMax - includeMin)];
        for (int i = includeMin; i < excludeMax; i += 1)
        {
            newArr[i] = i;
        }
        return newArr[Random.Range(0, newArr.Length)];
    }

    public static int PickRandom(int includeMin, int excludeMax, params int[] exceptArr)
    {
        var newArr = new int[Mathf.Abs(excludeMax - includeMin)];
        for (int i = includeMin; i < excludeMax; i += 1)
        {
            newArr[i] = i;
        }
        newArr = newArr.Except(exceptArr).ToArray();
        return newArr[Random.Range(0, newArr.Length)];
    }
    #endregion

    #region GetEnumArray
    public static T[] GetEnumArray<T>(params T[] except) where T : System.Enum
    {
        var arr = System.Enum.GetValues(typeof(T));
        var result = new List<T>();
        foreach (T element in arr)
        {
            if (!except.Contains(element))
            {
                result.Add(element);
            }
        }
        return result.ToArray();
    }
    #endregion

    #region Choose
    public static T Choose<T>(params T[] values)
    {
        return values[Random.Range(0, values.Length)];
    }
    #endregion

    #region Direction
    public static (int, int)[] Get4DirTuples()
    {
        var result = new (int, int)[4]
        {
            // cos(0~270), sin(0~270)
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1),
        };
        return result;
    }
    #endregion
}
