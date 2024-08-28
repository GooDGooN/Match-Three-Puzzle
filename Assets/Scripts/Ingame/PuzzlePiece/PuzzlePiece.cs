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

    public bool IsMatchable()
    {
        var fieldInfo = MyManager.FieldInfo;

        var pieces = MyManager.PieceField;
        if(pieces != null)
        {
            for (int dir = 0; dir <= 90; dir += 90)
            {
                var dirx = (int)MyMath.GetCosAngle(dir, true);
                var diry = (int)MyMath.GetSinAngle(dir, true);
                var dupCount = 0;
                for (int dist = -2; dist < 3; dist++)
                {
                    var targetXIndex = MyIndex.Item1 + (dirx * dist);
                    var targetYIndex = MyIndex.Item2 + (diry * dist);
                    if (targetXIndex != Mathf.Clamp(targetXIndex, 0, fieldInfo.Width - 1) || targetYIndex != Mathf.Clamp(targetYIndex, 0, fieldInfo.Height - 1))
                    {
                        continue;
                    }
                    else
                    {
                        if(pieces[targetXIndex, targetYIndex] != null)
                        {
                            var targetType = pieces[targetXIndex, targetYIndex].MyType;
                            if (targetType == MyType && targetType != PieceType.None)
                            {
                                dupCount++;
                            }
                            else
                            {
                                dupCount = 0;
                                continue;
                            }
                        }
                    }
                }
                if (dupCount >= 3)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public PieceType[] GetNearPieces()
    {
        var fieldInfo = MyManager.FieldInfo;

        var result = new List<PieceType>();
        for (int dir = 0; dir <= 270; dir += 90)
        {
            var dirx = (int)MyMath.GetCosAngle(dir, true);
            var diry = (int)MyMath.GetSinAngle(dir, true);
            var targetPiece = MyManager.PieceField[dirx, diry];
            if (dirx != Mathf.Clamp(dirx, 0, fieldInfo.Width - 1) || diry != Mathf.Clamp(diry, 0, fieldInfo.Height - 1))
            {
                continue;
            }
            if (targetPiece == null)
            {
                continue;
            }
            result.Add(targetPiece.MyType);
        }
        return result.ToArray();
    }
}
