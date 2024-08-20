
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utility
{
    public static T PickRandom<T>(T[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }
    public static T PickRandom<T>(T[] arr, T[] excludeArr)
    {
        var newArr = arr.Except(excludeArr).ToArray();
        Debug.Log(newArr.Length);
        return newArr[Random.Range(0, newArr.Length)];
    }

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

    public static int PickRandom(int includeMin, int excludeMax, int[] excludeArr)
    {
        if(includeMin == excludeMax)
        {
            if(excludeArr.Contains(includeMin))
            {
                return 0;
            }
            return includeMin;
        }

        var increament = excludeMax > includeMin ? 1 : -1;
        var newArr = new int[Mathf.Abs(excludeMax - includeMin)];
        for(int i = includeMin; i < excludeMax; i += increament)
        {
            newArr[i] = i;
        }
        newArr = newArr.Except(excludeArr).ToArray();
        
        return newArr[Random.Range(0, newArr.Length)];
    }

}
