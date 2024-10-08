using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum PieceType
{
    None = -2,
    Block,
    Blue,
    Green,
    Orange,
    Pink,
    Red,
    Sky,
    Yellow,
    Rainbow,
}

public enum PieceSubType
{
    None,
    Vbomb,
    Hbomb,
}

public class PuzzlePiece : MonoBehaviour
{
    public Sprite[] PieceSprites;
    public Sprite[] SpecialPieceSprites;
    public PieceType MyType;
    public PieceType TargetChangeType;
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
                result.Add(MyManager.MyPieceField[pos.Item1, pos.Item2]);
            }
        }
        return result.ToArray();
    }

    public void RefreshSprite()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
    }

    public void RemoveSelf()
    {
        MyManager.BombPieceList.Remove(this);
        var ty = MyManager.MyPieceField[MyIndex.Item1].GetIndex(this);
        MyManager.MyPieceField[MyIndex.Item1, ty] = null;
        MyIndex = (-1, -1);
        transform.position = new Vector2(0, -500.0f);
        MyManager.BombGage++;
    }

    public void ChangeToNewType()
    {
        MyType = TargetChangeType;
        TargetChangeType = PieceType.None;
    }

}

