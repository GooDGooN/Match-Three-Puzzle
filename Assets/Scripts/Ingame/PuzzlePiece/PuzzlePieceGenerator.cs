using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


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
        var fieldInfo = pieceManager.FieldInfo;
        var pieceField = pieceManager.PieceField;
        var pieceList = pieceManager.PieceList;
        #endregion
        #region Instantiate Minimum Matchable Piece
        {
            List<(int, int)> matchablePointList = new();
            var count = Random.Range(matchablePieceAmount.x, matchablePieceAmount.y + 1);
            for (int i = 0; i < count; i++)
            {
                var ix = Utility.PickRandom(0, fieldInfo.Width);
                var iy = Utility.PickRandom(0, fieldInfo.Height);
                matchablePointList.Add(new(ix, iy));
            }

            while (matchablePointList.Count > 0)
            {
                var targetRandomPointIndex = Random.Range(0, matchablePointList.Count);
                var targetRandomPiecePoint = matchablePointList[targetRandomPointIndex];
                var createDirection = new List<(int, int)>();

                //4 dir check
                foreach(var dir in Utility.Get4DirTuples())
                {
                    var ix = dir.Item1;
                    var iy = dir.Item2;
                    var passable = true;

                    // check if direction is ok
                    for (int dist = 0; dist < 4; dist++)
                    {
                        ix *= dist;
                        ix += targetRandomPiecePoint.Item1;

                        iy *= dist;
                        iy += targetRandomPiecePoint.Item2;

                        if (pieceManager.IsPlaceAreExist(ix, iy) || !pieceManager.IsPlaceEmpty(ix, iy))
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
                    var ix = finalDirection.Item1;
                    var iy = finalDirection.Item2;
                    var exceptIndex = Utility.Choose(2, 3);
                    var targetType = Utility.PickRandom(Utility.GetEnumArray<PieceType>(PieceType.None));

                    for (int dist = 0; dist < 4; dist++)
                    {
                        ix *= dist;
                        ix += targetRandomPiecePoint.Item1;

                        iy *= dist;
                        iy += targetRandomPiecePoint.Item2;
                        if (dist != exceptIndex)
                        {
                            InstantiatePiece(ix, iy, Utility.GetEnumArray(targetType));
                        }
                        else
                        {
                            InstantiatePiece(ix, iy, targetType);
                        }
                    }
                }
                matchablePointList.RemoveAt(targetRandomPointIndex);
            }
        }
        #endregion
        #region Instantiate All Piece
        {
            List<PieceType> exceptTypes = new();
            for (int iy = 0; iy < fieldInfo.Height; iy++)
            {
                for (int ix = 0; ix < fieldInfo.Width; ix++)
                {
                    if (pieceManager.IsPlaceEmpty(ix, iy))
                    {
                        if (ix >= 2)
                        {
                            if (pieceField[ix - 2][iy].MyType == pieceField[ix - 1][iy].MyType)
                            {
                                exceptTypes.Add(pieceField[ix - 1][iy].MyType);
                            }
                        }
                        if (iy >= 2)
                        {
                            if (pieceField[ix][iy - 2].MyType == pieceField[ix][iy - 1].MyType)
                            {
                                exceptTypes.Add(pieceField[ix][iy - 1].MyType);
                            }
                        }
                        InstantiatePiece(ix, iy, exceptTypes.ToArray());
                    }
                    else
                    {
                        if (pieceField[ix][iy].GetMatchablePieces().Length > 0)
                        {
                            var enumArr = Utility.GetEnumArray<PieceType>();
                            var exceptArr = pieceField[ix][iy].GetNearTypes();
                            pieceField[ix][iy].MyType = Utility.PickRandom(enumArr, exceptArr);
                        }
                    }
                    // TESTLOG!!
                    Debug.Log(pieceField[ix][iy].GetMatchablePieces().Length);
                    exceptTypes.Clear();
                }
            }
        }
        #endregion
        #region Local Function
        void InstantiatePiece(int x, int y, params PieceType[] exceptType)
        {
            var pos = pieceManager.GetPiecePosition(x, y);

            pieceList.Add(Instantiate(PuzzlePiecePrefab, pieceManager.PieceContainer.transform).GetComponent<PuzzlePiece>());
            pieceField[x][y] = pieceList.Last();

            var target = pieceList.Last();
            target.MyIndex = (x, y);
            target.transform.localPosition = pos;
            target.MyManager = pieceManager;

            var enumArr = Utility.GetEnumArray(PieceType.None);
            if (exceptType != null)
            {
                target.MyType = Utility.PickRandom(enumArr, exceptType);
            }
            else
            {
                target.MyType = Utility.PickRandom(enumArr);
            }
        }
        #endregion
    }
}

