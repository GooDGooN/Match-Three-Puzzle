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
}

public enum PieceSubType
{
    None = -1,
    Vbomb,
    Hbomb,
    CrossBomb,
    Rainbow,
}

public class PuzzlePiece : MonoBehaviour
{
    private Color[] specialTypeColors = new Color[] {
        new Color(0, 0, 0.8f),
        new Color(0, 0.8f, 0),
        new Color(0.8f, 0.5f, 0),
        new Color(0.8f, 0, 0.8f),
        new Color(0.8f, 0, 0),
        new Color(0, 0.8f, 0.8f),
        new Color(0.8f, 0.8f, 0),
    };

    public Sprite[] PieceSprites;
    public Sprite[] SpecialPieceSprites;
    public PieceType MyType;
    public PieceType TargetChangeType;
    public PieceSubType MySubType;
    public PieceSubType TargetChangeSubType;
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
        MySubType = PieceSubType.None;
        TargetChangeSubType = PieceSubType.None;
    }


    void Update()
    {
        if (MySubType == PieceSubType.None)
        {
            GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = specialTypeColors[(int)MyType];
            GetComponent<SpriteRenderer>().sprite = SpecialPieceSprites[(int)MySubType];
        }    
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

    public void ChangeToNewType()
    {
        if(TargetChangeType != PieceType.None)
        {
            MyType = TargetChangeType;
            TargetChangeType = PieceType.None;
        }

        if(TargetChangeSubType != PieceSubType.None)
        {
            MySubType = TargetChangeSubType;
            TargetChangeSubType = PieceSubType.None;
        }
    }

    public void ResetPiece((int, int) tuple)
    {
        var except = new PieceType[] { PieceType.None, PieceType.Block };
        MyType = Utility.PickRandom(Utility.GetEnumArray(except));
        MyIndex = tuple;
        transform.position = MyManager.GetPiecePosition(MyIndex) + (Vector3Int.up * 300);
    }
}

