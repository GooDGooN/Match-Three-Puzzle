
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        var result = new T[arr.Length - except.Length];
        var index = 0;
        foreach (var element in arr)
        {
            if (!except.Contains((T)element))
            {
                result[index++] = (T)element;
            }
        }
        return result;
    }
    #endregion

    #region Choose
    public static T Choose<T>(params T[] values)
    {
        return values[Random.Range(0, values.Length)];
    }
    #endregion

}
