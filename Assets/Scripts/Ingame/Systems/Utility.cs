
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

    public static T PickRandom<T>(T[] arr, T[] excludeArr)
    {
        var newArr = arr.Except(excludeArr).ToArray();
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

    public static int PickRandom(int includeMin, int excludeMax, int[] excludeArr)
    {
        var newArr = new int[Mathf.Abs(excludeMax - includeMin)];
        for (int i = includeMin; i < excludeMax; i += 1)
        {
            newArr[i] = i;
        }
        newArr = newArr.Except(excludeArr).ToArray();
        return newArr[Random.Range(0, newArr.Length)];
    }
    #endregion

    #region GetEnumArray
    public static T[] GetEnumArray<T>() where T : System.Enum
    {
        var arr = System.Enum.GetValues(typeof(T));
        var result = new T[arr.Length];
        var index = 0;
        foreach (var element in arr)
        {
            result[index++] = (T)element;
        }
        return result;
    }
    #endregion
}
