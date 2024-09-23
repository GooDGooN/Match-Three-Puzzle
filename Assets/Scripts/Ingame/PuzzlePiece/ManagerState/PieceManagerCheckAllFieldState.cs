using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerCheckAllFieldState : BaseFSM<PuzzlePieceManager>
    {
        public override void StateEnter()
        {
            foreach (var piece in self.PieceList)
            {
                foreach(var dir in Utility.Get4DirTuples())
                {
                    var rightMatrix3x3 = new PieceMatrix3x3();
                    var selectPieceList = new List<PuzzlePiece>();
                    for (int mx = 0; mx < 3; mx++)
                    {
                        for (int my = 0; my < 3; my++)
                        {
                            /*
                             * tx = (mx + 1) * cos(dir) - (my - 1) * sin(dir)
                             * ty = (mx + 1) * sin(dir) + (my - 1) * cos(dir)
                             */
                            var tx = piece.MyIndex.Item1 + ((mx + 1) * dir.Item1) - ((my - 1) * dir.Item2);
                            var ty = piece.MyIndex.Item2 + ((mx + 1) * dir.Item2) + ((my - 1) * dir.Item1);

                            if (self.IsPlaceAreExist(tx, ty))
                            {
                                if (self.PieceField[tx][ty].MyType == piece.MyType)
                                {
                                    selectPieceList.Add(self.PieceField[tx][ty]);
                                    rightMatrix3x3[mx, my] = 1;
                                }
                            }
                        }
                    }
                    if(MatchablePattern.CheckEqual(rightMatrix3x3))
                    {
                        Debug.Log($"matchable is {piece.MyIndex} Piece");
                        stateManager.ChangeState<PieceManagerIdleState>();
                        return;
                    }
                }
            }

            #region Clear all field cause there is no matchable
            Debug.Log("there is nothing to do!!");
            for (int ix = 0; ix < self.FieldInfo.Width; ix++)
            {
                for (int iy = 0; iy < self.FieldInfo.Height; iy++)
                {
                    self.PieceField[ix][iy] = null;
                }
            }
            stateManager.ChangeState<PieceManagerGeneratePieceState>();
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
    }
}
