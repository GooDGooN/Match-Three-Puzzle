using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerGeneratePieceState : BaseFSM<PuzzlePieceManager>
    {
        public override void StateEnter()
        {
            #region Initialize
            var fieldInfo = self.FieldInfo;
            var pieceField = self.PieceField;
            var pieceList = self.PieceList;
            var usablePieceIndex = 0;
            #endregion
            #region Instantiate Minimum Matchable Piece
            {
                List<(int, int)> matchablePointList = new();
                var count = Random.Range(self.matchablePieceAmount.x, self.matchablePieceAmount.y + 1);
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
                    foreach (var dir in Utility.Get4DirTuples())
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

                            if (self.IsPlaceAreExist(ix, iy) || !self.IsPlaceEmpty(ix, iy))
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
                        if (self.IsPlaceEmpty(ix, iy))
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
                            if (self.GetMatchablePieces(pieceField[ix][iy]).Length > 0)
                            {
                                var enumArr = Utility.GetEnumArray<PieceType>();
                                var exceptArr = pieceField[ix][iy].GetNearTypes();
                                pieceField[ix][iy].MyType = Utility.PickRandom(enumArr, exceptArr);
                            }
                        }
                        exceptTypes.Clear();
                    }
                }
            }
            #endregion
            #region Local Function
            void InstantiatePiece(int x, int y, params PieceType[] exceptType)
            {
                var pos = self.GetPiecePosition(x, y);

                if (pieceList.Count < fieldInfo.Width * fieldInfo.Height)
                {
                    pieceList.Add(Instantiate(self.PuzzlePiecePrefab, self.PieceContainer.transform).GetComponent<PuzzlePiece>());
                    pieceField[x][y] = pieceList.Last();
                }
                else
                {
                    pieceField[x][y] = pieceList[usablePieceIndex++];
                }

                var target = pieceField[x][y];
                target.MyIndex = (x, y);
                target.transform.localPosition = pos + Vector3.up * (500.0f + y * 200.0f);
                target.MyManager = self;

                if (x == self.FieldInfo.Width - 1 && y == self.FieldInfo.Height - 1)
                {
                    self.RepositionPiece(target, target.MyIndex, GererateDone);
                }
                else
                {
                    self.RepositionPiece(target, target.MyIndex, null);
                }

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

        public override void StateExit()
        {
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
        }

        private void GererateDone()
        {
            Debug.Log("Piece generate is done!");
            stateManager.ChangeState<PieceManagerIdleState>();
        }
    }

}