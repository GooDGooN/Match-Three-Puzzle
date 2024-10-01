using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PuzzlePieceManager;

public static class ExtentionMethod
{
    public static int GetIndex(this PuzzlePiece[] pieces, PuzzlePiece target)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == target)
            {
                return i;
            }
        }
        return -1;
    }
}
