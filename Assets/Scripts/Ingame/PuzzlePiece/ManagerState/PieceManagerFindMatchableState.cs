using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerFindMatchableState : BaseFSM<PuzzlePieceManager>
    {
        private List<PuzzlePiece> tempPuzzlePieceList = new();
        public override void StateEnter()
        {
            self.HintPieceList.Clear();

            while (self.BombGage >= self.MaxBombGage)
            {
                self.BombGage -= self.MaxBombGage;
                var except = self.PieceList.Where(piece => piece.MyType == PieceType.Vbomb || piece.MyType == PieceType.Hbomb).ToList();
                except.AddRange(self.HintPieceList);

                var target = Utility.PickRandom(self.PieceList.ToArray(), except.ToArray());
                target.TargetChangeType = Utility.Choose(PieceType.Hbomb, PieceType.Vbomb);
                target.MyAnimator.SetTrigger("ChangeType");
                self.bombPieceList.Add(target);
            }

            if (self.bombPieceList.Count > 0)
            {
                self.HintPieceList.Add(self.bombPieceList[0]);
                Debug.Log($"matchable is {self.HintPieceList.Last().MyIndex} Piece");
                stateManager.ChangeState<PieceManagerIdleState>();
                return;
            }

            foreach (var piece in self.PieceList)
            {
                foreach (var dir in Utility.Get4DirTuples())
                {
                    var rightMatrix3x3 = new PieceMatrix3x3();
                    for (int i = 0; i < rightMatrix3x3.Value.Length; i++)
                    {
                        /*
                        * tx = (mx + 1) * cos(dir) - (my - 1) * sin(dir)
                        * ty = (mx + 1) * sin(dir) + (my - 1) * cos(dir)
                        */
                        var mx = (i % 3);
                        var my = (i / 3);
                        var tx = piece.MyIndex.Item1 + ((mx + 1) * dir.Item1) - ((my - 1) * dir.Item2);
                        var ty = piece.MyIndex.Item2 + ((mx + 1) * dir.Item2) + ((my - 1) * dir.Item1);

                        if (self.IsPlaceAreExist(tx, ty))
                        {
                            if (self.MyPieceField[tx][ty].MyType == piece.MyType)
                            {
                                rightMatrix3x3[mx, my] = 1;
                            }
                        }
                    }


                    var matrixIndex = MatchablePattern.GetEqualMatrixIndex(rightMatrix3x3);
                    if (matrixIndex != -1)
                    {
                        // save hint
                        var posTupleSave = piece.MyIndex;
                        var testPosTuple = piece.MyIndex;
                        switch(matrixIndex)
                        {
                            case 1:
                                testPosTuple.Item1 += -dir.Item2;
                                testPosTuple.Item2 += dir.Item1;
                                break;
                            case 2:
                                testPosTuple.Item1 += dir.Item2;
                                testPosTuple.Item2 += -dir.Item1;
                                break;
                            default:
                                testPosTuple.Item1 += dir.Item1;
                                testPosTuple.Item2 += dir.Item2;
                                break;
                        }

                        piece.MyIndex = testPosTuple;
                        tempPuzzlePieceList = self.GetMatchablePieces(piece).ToList();
                        piece.MyIndex = posTupleSave;

                        if (self.HintPieceList.Count < tempPuzzlePieceList.Count)
                        {
                            self.HintPieceList = tempPuzzlePieceList.ToList();
                        }
                    }
                }
            }

            // Some piece is matchable
            if(self.HintPieceList.Count > 0)
            {
                Debug.Log($"matchable is {self.HintPieceList.Last().MyIndex} Piece");
                stateManager.ChangeState<PieceManagerIdleState>();
                return;
            }

            // Clear all field cause there is no matchable
            Debug.Log("there is nothing to do!!");
            for (int ix = 0; ix < self.FieldInfo.Width; ix++)
            {
                for (int iy = 0; iy < self.FieldInfo.Height; iy++)
                {
                    self.MyPieceField[ix][iy] = null;
                }
            }
            stateManager.ChangeState<PieceManagerGeneratePieceState>();
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
    }
}
