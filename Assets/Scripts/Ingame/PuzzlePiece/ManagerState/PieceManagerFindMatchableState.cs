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
/*
 * !!TEST!!
            while (self.BombGage >= self.MaxBombGage)
            {
                self.BombGage -= self.MaxBombGage;
                var except = self.PieceList.Where(piece => piece.MyType == PieceType.Vbomb || piece.MyType == PieceType.Hbomb).ToList();
                except.AddRange(self.HintPieceList);

                var target = Utility.PickRandom(self.PieceList.ToArray(), except.ToArray());
                target.TargetChangeType = Utility.Choose(PieceType.Hbomb, PieceType.Vbomb);
                target.MyAnimator.SetTrigger("ChangeType");
                self.BombPieceList.Add(target);
            }

            if (self.BombPieceList.Count > 0)
            {
                self.HintPieceList.Add(self.BombPieceList[0]);
                Debug.Log($"matchable is {self.HintPieceList.Last().MyIndex} Piece");
                stateManager.ChangeState<PieceManagerIdleState>();
                return;
            }
*/
            foreach (var piece in self.PieceList)
            {
                foreach (var dir in Utility.Get4DirTuples())
                {

                    var tx = piece.MyIndex.Item1 + dir.Item1;
                    var ty = piece.MyIndex.Item2 + dir.Item2;

                    if (self.IsPlaceAreExist(tx, ty) && self.MyPieceField[tx,ty].MyType != piece.MyType)
                    {
                        var posTupleSave = piece.MyIndex;

                        piece.MyIndex = (tx, ty);
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
