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
    private Color32[] pieceColors = new Color32[] 
    {
        new Color32(60, 118, 255, 255), // blue
        new Color32(45, 255, 107, 255), // green
        new Color32(255, 163, 60, 255), // orange
        new Color32(255, 85, 225, 255), // pink
        new Color32(255, 55, 55, 255), // red
        new Color32(0, 232, 251, 255), // lightblue
        new Color32(255, 209, 51, 255), // yellow  
    };

    private Color32[] pieceBackgroundColors = new Color32[]
    {
        new Color32(182, 203, 255, 255), // blue
        new Color32(177, 255, 199, 255), // green
        new Color32(255, 220, 182, 255), // orange
        new Color32(255, 191, 243, 255), // pink
        new Color32(255, 197, 197, 255), // red
        new Color32(179, 248, 255, 255), // lightblue
        new Color32(255, 237, 178, 255), // yellow  
    };

    public Sprite[] PieceSprites;
    public Sprite[] SpecialPieceSprites;
    public SpriteRenderer BackgroundSpriteRenderer
    {
        get => transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

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
        BackgroundSpriteRenderer.color = pieceBackgroundColors[(int)MyType];
        GetComponent<SpriteRenderer>().color = pieceColors[(int)MyType];

        if (MySubType == PieceSubType.None)
        {
            GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = SpecialPieceSprites[(int)MySubType];
            BackgroundSpriteRenderer.color = Color.clear;
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

