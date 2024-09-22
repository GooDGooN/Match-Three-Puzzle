using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceMatrix3x3 
{
    public int[] Value = new int[9];
    public PieceMatrix3x3(int[,] target = null)
    {
        if(target != null)
        {
            var index = 0;
            foreach(var value in target)
            {
                Value[index++] = value;
            }
        }
    }

    public int this[int ix, int iy]
    {
        get => Value[ix + (iy * 3)];
        set => Value[ix + (iy * 3)] = value;
    }
    public int this[int index]
    {
        get => Value[index];
        set => Value[index] = value;
    }
}

public class MatchablePattern
{
    /*
     * 000 | 100 | 000 | 110 | 000 
     * 011 | 010 | 010 | 000 | 000
     * 000 | 000 | 100 | 000 | 110 ก่
     */
    public static readonly int[,] MatchablePatternIndexs = new int[5,2] {
        { 4, 5 },
        { 4, 6 },
        { 0, 4 },
        { 6, 7 },
        { 0, 1 },
    };

    public static bool CheckEqual(PieceMatrix3x3 matrix)
    {
        for(int i = 0; i < 5; i++)
        {
            var index1 = MatchablePatternIndexs[i, 0];
            var index2 = MatchablePatternIndexs[i, 1];
            if (matrix[index1] == 1 && matrix[index2] == 1)
            {
                return true;
            }
        }
        return false;
    }
}
