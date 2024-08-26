using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMath
{
    #region Angle
    public static float GetRadian(float angle)
    {
        return Mathf.Deg2Rad * angle;
    }

    public static float GetCosAngle(float angle, bool needRound = false)
    {
        var result = Mathf.Cos(GetRadian(angle));
        if (needRound)
        {
            return Approximate01(result);
        }
        return result;
    }

    public static float GetSinAngle(float angle, bool needRound = false)
    {
        var result = Mathf.Sin(GetRadian(angle));
        if (needRound)
        {
            return Approximate01(result);
        }
        return result;
    }
    #endregion

    public static float Approximate01(float value)
    {
        if (Mathf.Approximately(value, 0) || Mathf.Approximately(value, 1))
        {
            value = Mathf.RoundToInt(value);
        }
        return value;
    }
}
