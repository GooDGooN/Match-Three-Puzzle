using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                matchablePointList.Add(new(ix, iy));
            }

            while (matchablePointList.Count > 0)
            {
                var targetRandomPointIndex = Random.Range(0, matchablePointList.Count);
                var targetRandomPiecePoint = matchablePointList[targetRandomPointIndex];
                var createDirection = new List<int>();

                //4 dir check
                for (int dir = 0; dir <= 270; dir += 90)
                {
                    var ix = (int)MyMath.GetCosAngle(dir, true);
                    var iy = (int)MyMath.GetSinAngle(dir, true);
                    var passable = true;

                    // check if direction is ok
                    for (int dist = 0; dist < 4; dist++)
                    {
                        ix *= dist;
                        ix += targetRandomPiecePoint.Item1;

                        iy *= dist;
                        iy += targetRandomPiecePoint.Item2;

                        if (ix != Mathf.Clamp(ix, 0, 9) || iy != Mathf.Clamp(iy, 0, 9))
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

                    if (passable)
                    {
                        createDirection.Add(dir);
                    }
                }
                if (createDirection.Count > 0)
                {
                    var finalDirection = createDirection[Random.Range(0, createDirection.Count)];
                    var ix = (int)MyMath.GetCosAngle(finalDirection);
                    var iy = (int)MyMath.GetSinAngle(finalDirection);
                    var exceptIndex = Utility.Choose(2, 3);

                    for (int dist = 0; dist < 4; dist++)
                    {
                        ix *= dist;
                        ix += targetRandomPiecePoint.Item1;

                        iy *= dist;
                        iy += targetRandomPiecePoint.Item2;
                        if (dist != exceptIndex)
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
            List<PieceType> excludeType = new();
            for (int iy = 0; iy <= fieldSize.y; iy++)
            {
                for (int ix = 0; ix <= fieldSize.x; ix++)
                {
                    if (puzzlePieces[ix, iy] == null)
                    {
                        if (ix >= 2)
                        {
                            if (puzzlePieces[ix - 2, iy].MyType == puzzlePieces[ix - 1, iy].MyType)
                            {
                                excludeType.Add(puzzlePieces[ix - 1, iy].MyType);
                            }
                        }
                        if (iy >= 2)
                        {
                            if (puzzlePieces[ix, iy - 2].MyType == puzzlePieces[ix, iy - 1].MyType)
                            {
                                excludeType.Add(puzzlePieces[ix, iy - 1].MyType);
                            }
                        }
                        InstantiatePiece(ix, iy, excludeType.ToArray());
                    }
                    else
                    {
                        if(puzzlePieces[ix, iy].IsMatchable())
                        {

                        }
                    }
                    excludeType.Clear();
                }
            }
        }
        #endregion
        #region Local Function

        void InstantiatePiece(int x, int y, params PieceType[] exceptType)
        {
            var pos = new Vector3(x, y);
            pos -= fieldSize / 2.0f;
            pos *= GameManager.PieceSize;

            var resultExcept = new PieceType[exceptType.Length + 1];
            resultExcept[0] = PieceType.None;
            for (int i = 0; i < exceptType.Length; i++)
            {
                resultExcept[i + 1] = exceptType[i];
            }

            puzzlePieces[x, y] = Instantiate(PuzzlePiecePrefab, pieceManager.PuzzlePieceContainer.transform).GetComponent<PuzzlePiece>();
            var target = puzzlePieces[x, y];
            target.MyIndex = (x, y);
            target.transform.localPosition = pos;
            target.MyManager = pieceManager;

            if (exceptType != null)
            {
                target.MyType = Utility.PickRandom(Utility.GetEnumArray<PieceType>(resultExcept));
            }
            else
            {
                target.MyType = Utility.PickRandom(Utility.GetEnumArray<PieceType>(PieceType.None));
            }
        }
        #endregion
    }


    private void TestCallAllPiece()
    {
        var fieldSize = GameManager.PieceFieldSize - new Vector3(1.0f, 1.0f, 0.0f);
        var indexCount = Utility.GetEnumArray<PieceType>().Length;
        var colorCount = new int[indexCount];
        for (int iy = 0; iy <= fieldSize.y; iy++)
        {
            for (int ix = 0; ix <= fieldSize.x; ix++)
            {
                colorCount[(int)pieceManager.PuzzlePieces[ix, iy].MyType]++; 
            }
        }
    }

    void Update()
    {
        
    }
}

