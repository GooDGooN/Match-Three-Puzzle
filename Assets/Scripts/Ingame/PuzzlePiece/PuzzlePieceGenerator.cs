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

                        if (ix != Mathf.Clamp(ix, 0, fieldInfo.Width - 1) || iy != Mathf.Clamp(iy, 0, fieldInfo.Height - 1))
                        {
                            passable = false;
                            break;
                        }

                        if (pieceField[ix, iy] != null)
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
            List<PieceType> exceptTypes = new();
            for (int iy = 0; iy < fieldInfo.Height; iy++)
            {
                for (int ix = 0; ix < fieldInfo.Width; ix++)
                {
                    if (pieceField[ix, iy] == null)
                    {
                        if (ix >= 2)
                        {
                            if (pieceField[ix - 2, iy].MyType == pieceField[ix - 1, iy].MyType)
                            {
                                exceptTypes.Add(pieceField[ix - 1, iy].MyType);
                            }
                        }
                        if (iy >= 2)
                        {
                            if (pieceField[ix, iy - 2].MyType == pieceField[ix, iy - 1].MyType)
                            {
                                exceptTypes.Add(pieceField[ix, iy - 1].MyType);
                            }
                        }
                        InstantiatePiece(ix, iy, exceptTypes.ToArray());
                    }
                    else
                    {
                        if(pieceField[ix, iy].IsMatchable())
                        {
                            pieceField[ix, iy].MyType = Utility.PickRandom(Utility.GetEnumArray(pieceField[ix, iy].GetNearPieces()));
                        }
                    }
                    Debug.Log(pieceField[ix, iy].IsMatchable());
                    exceptTypes.Clear();
                }
            }
        }
        #endregion
        #region Local Function

        void InstantiatePiece(int x, int y, params PieceType[] exceptType)
        {
            var pos = new Vector3(x, y);
            pos -= fieldInfo.Size / 2;
            pos *= pieceManager.PieceSize;

            var resultExcept = new PieceType[exceptType.Length + 1];
            resultExcept[0] = PieceType.None;
            for (int i = 0; i < exceptType.Length; i++)
            {
                resultExcept[i + 1] = exceptType[i];
            }
            pieceList.Add(Instantiate(PuzzlePiecePrefab, pieceManager.PieceContainer.transform).GetComponent<PuzzlePiece>());
            pieceField[x, y] = pieceList.Last();

            var target = pieceField[x, y];
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


/*    private void TestCallAllPiece()
    {
        var fieldRange = GameManager.PieceFieldSize;
        var indexCount = Utility.GetEnumArray<PieceType>().Length;
        var colorCount = new int[indexCount];
        for (int iy = 0; iy < fieldRange.y; iy++)
        {
            for (int ix = 0; ix < fieldRange.x; ix++)
            {
                colorCount[(int)pieceManager.PieceField[ix, iy].MyType]++; 
            }
        }
    }*/

    void Update()
    {
        
    }
}

