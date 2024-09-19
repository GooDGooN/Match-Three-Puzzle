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

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
    }


    void Update()
    {
    }

    public PuzzlePiece[] GetMatchablePieces()
    {
        var fieldInfo = MyManager.FieldInfo;
        var pieces = MyManager.PieceField;
        var resultList = new List<PuzzlePiece>();
        var testList = new List<PuzzlePiece>();

        if (pieces != null)
        {
            foreach (var dir in Utility.Get4DirTuples())
            {
                var nextPos = (MyIndex.Item1 + dir.Item1, MyIndex.Item2 + dir.Item2);
                while (MyManager.IsPlaceAreExist(nextPos) && !MyManager.IsPlaceEmpty(nextPos))
                {
                    var target = pieces[nextPos.Item1][nextPos.Item2];
                    if (target.MyType != MyType)
                    {
                        break;
                    }
                    testList.Add(target);
                    nextPos.Item1 += dir.Item1;
                    nextPos.Item2 += dir.Item2;
                }
                if(testList.Count >= 2)
                {
                    resultList.AddRange(testList);
                }
                testList.Clear();
            }
        }
        if(resultList.Count > 0)
        {
            resultList.Add(this);
        }
        return resultList.ToArray();
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

}

