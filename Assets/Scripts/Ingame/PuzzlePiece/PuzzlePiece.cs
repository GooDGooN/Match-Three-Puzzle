using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
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
        var fieldSize = GameManager.PieceFieldSize - new Vector3(1.0f, 1.0f, 0.0f);
        var pieces = MyManager?.PuzzlePieces;
        if(pieces != null)
        {
            for (int dir = 0; dir <= 270; dir += 90)
            {
                var dirx = (int)Mathf.Sin(dir);
                var diry = (int)Mathf.Cos(dir);
                var dupCount = 0;
                for (int dist = 0; dist < 3; dist++)
                {
                    var targetXIndex = MyIndex.Item1 + (dirx * dist);
                    var targetYIndex = MyIndex.Item2 + (diry * dist);
                    if (targetXIndex != Mathf.Clamp(targetXIndex, 0, fieldSize.x) || targetYIndex != Mathf.Clamp(targetYIndex, 0, fieldSize.y))
                    {
                        continue;
                    }
                    else
                    {
                        if(pieces[targetXIndex, targetYIndex] == null)
                        {
                            continue;
                        }
                        else if (pieces[targetXIndex, targetYIndex].MyType == MyType)
                        {
                            dupCount++;
                        }
                        else
                        {
                            continue;
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

}
