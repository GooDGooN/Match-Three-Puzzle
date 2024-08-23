
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzlePieceGenerator : MonoBehaviour
{
    public GameObject PuzzlePiecePrefab;
    private PuzzlePieceManager pieceManager;
    private Vector2Int matchablePieceAmount = new Vector2Int(3, 5);

    private void Awake() 
    {
        pieceManager = GetComponentInParent<PuzzlePieceManager>();
    }   

    private void Start()
    {
        #region Initialize
        var fieldWidth = GameManager.PieceFieldSize.x;
        var fieldHeight = GameManager.PieceFieldSize.y;
        var fieldSize = GameManager.PieceFieldSize - new Vector3(1.0f, 1.0f, 0.0f);
        var puzzlePieces = pieceManager.PuzzlePieces;
        #endregion

        #region Instantiate Minimum Matchable Piece
        {
            List<(int, int)> matchablePointList = new();
            var count = Random.Range(matchablePieceAmount.x, matchablePieceAmount.y + 1);
            for (int i = 0; i < count; i++)
            {
                var ix = Utility.PickRandom(0, fieldWidth);
                var iy = Utility.PickRandom(0, fieldHeight);
                matchablePointList.Add(new (ix, iy));
            }

            while(matchablePointList.Count > 0)
            {
                var targetRandomPointIndex = Random.Range(0, matchablePointList.Count);
                var targetRandomPiecePoint = matchablePointList[targetRandomPointIndex];
                var createDirection = new List<int>();

                //4 dir check

                for (int i = 0; i <= 270; i += 90)
                {
                    var ix = (int)Mathf.Cos(i);
                    var iy = (int)Mathf.Sin(i);
                    var passable = true;

                    // check if direction is ok
                    for (int j = 0; j < 4; j++)
                    {
                        ix *= j;
                        ix += targetRandomPiecePoint.Item1;

                        iy *= j;
                        iy += targetRandomPiecePoint.Item2;

                        if(ix != Mathf.Clamp(ix, 0, 9) || iy != Mathf.Clamp(iy, 0, 9))
                        {
                            passable = false;
                            break;
                        }

                        if (puzzlePieces[ix, iy] != null)
                        {
                            passable = false;
                            break;
                        }
                    }

                    if(passable)
                    {
                        createDirection.Add(i);
                    }
                }
                if(createDirection.Count > 0)
                {
                    var finalDirection = createDirection[Random.Range(0, createDirection.Count)];
                    var ix = (int)Mathf.Cos(finalDirection);
                    var iy = (int)Mathf.Sin(finalDirection);
                    var exceptIndex = Utility.Choose(2, 3);
                    
                    for (int i = 0; i < 4; i++)
                    {
                        ix *= i;
                        ix += targetRandomPiecePoint.Item1;

                        iy *= i;
                        iy += targetRandomPiecePoint.Item2;
                        if(i != exceptIndex)
                        {
                            InstantiatePiece(ix, iy);
                        }
                    }
                }
                matchablePointList.RemoveAt(targetRandomPointIndex);
            }
        }
        #endregion
        #region Instantiate All Piece
        {
            for (int iy = 0; iy <= fieldSize.y; iy++)
            {
                for (int ix = 0; ix <= fieldSize.x; ix++)
                {
                    var excludeType = new PieceType[2];

                    if (ix >= 2)
                    {
                        if(puzzlePieces[ix - 2, iy].MyType == puzzlePieces[ix - 1, iy].MyType)
                        {
                            excludeType[0] = puzzlePieces[ix - 1, iy].MyType;
                        }
                    }
                    if (iy >= 2)
                    {
                        if (puzzlePieces[ix, iy - 2].MyType == puzzlePieces[ix, iy - 1].MyType)
                        {
                            excludeType[1] = puzzlePieces[ix, iy - 1].MyType;
                        }
                    }
                    InstantiatePiece(ix, iy, excludeType);
                }
            }
        }
        #endregion
        #region Local Function
        void InstantiatePiece(int x, int y, PieceType[] exceptType = null)
        {
            var pieceSize = GameManager.PieceSize + 4;

            var pos = new Vector3(x, y);
            pos -= fieldSize / 2.0f;
            pos *= pieceSize;

            puzzlePieces[x, y] = Instantiate(PuzzlePiecePrefab, transform).GetComponent<PuzzlePiece>();
            var target = puzzlePieces[x, y];
            target.transform.localPosition = pos;
            
            if(exceptType != null)
            {
                target.MyType = Utility.PickRandom(Utility.GetEnumArray<PieceType>(), exceptType);
            }
            else
            {
                target.MyType = Utility.PickRandom(Utility.GetEnumArray<PieceType>());
            }
        }


        #endregion


    }


    void Update()
    {
        
    }
}

