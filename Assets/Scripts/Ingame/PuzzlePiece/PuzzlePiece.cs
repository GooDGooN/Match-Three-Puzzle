using System;
using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = PieceSprites[(int)MyType];
    }

    // Update is called once per frame
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
            var dirs = Utility.Get4DirTuples();
            foreach (var dir in dirs)
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

    public PieceType[] GetNearPieces()
    {
        var fieldInfo = MyManager.FieldInfo;

        var result = new List<PieceType>();
        for (int dir = 0; dir <= 270; dir += 90)
        {
            var dirx = (int)MyMath.GetCosAngle(dir, true);
            var diry = (int)MyMath.GetSinAngle(dir, true);

            if (dirx != Mathf.Clamp(dirx, 0, fieldInfo.Width - 1) || diry != Mathf.Clamp(diry, 0, fieldInfo.Height - 1))
            {
                continue;
            }
            if (MyManager.IsPlaceEmpty(dirx, diry))
            {
                continue;
            }
            result.Add(MyManager.PieceField[dirx][diry].MyType);
        }
        return result.ToArray();
    }


}

