using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum PieceType
{
    None = -1,
    Blue,
    Green,
    Orange,
    Pink,
    Red,
    Sky,
    Yellow,
}

public class PuzzlePiece : MonoBehaviour
{
    public Sprite[] PieceSprites;
    public PieceType MyType;
    public (int, int) MyIndex;
    public PuzzlePieceManager MyManager;
    public Animator MyAnimator
    {
        get => GetComponent<Animator>();
    }

    public int[] testpos = new int[2];

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
    }


    void Update()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
        testpos[0] = MyIndex.Item1;
        testpos[1] = MyIndex.Item2;
    }
    public PieceType[] GetNearTypes()
    {
        var result = new List<PieceType>();
        foreach(var item in GetNearPieces())
        {
            result.Add(item.MyType);
        }
        return result.ToArray();
    }

    public PuzzlePiece[] GetNearPieces()
    {
        var fieldInfo = MyManager.FieldInfo;
        var result = new List<PuzzlePiece>();

        foreach (var dir in Utility.Get4DirTuples())
        {
            var pos = (dir.Item1 + MyIndex.Item1, dir.Item2 + MyIndex.Item2);
            if (MyManager.IsPlaceAreExist(pos) && !MyManager.IsPlaceEmpty(pos))
            {
                result.Add(MyManager.PieceField[pos.Item1][pos.Item2]);
            }
        }
        return result.ToArray();
    }

    public void RefreshSprite()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
    }
}

