using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerRefillState : BaseFSM<PuzzlePieceManager>
    {
        public override void StateEnter()
        {
            for (int ix = 0; ix < self.PieceField.Length; ix++)
            {
                var col = self.PieceField[ix];
                if (col.Count < self.FieldInfo.Height)
                {
                    for(int iy = 0;  iy < col.Count; iy++)
                    {
                        var targetPiece = self.PieceField[ix][iy];
                        if(targetPiece.MyIndex.Item2 > iy)
                        {
                            self.RepositionPiece(targetPiece, (ix, iy));
                        }
                    }

                    while(col.Count != self.FieldInfo.Height)
                    {
                        var newPiece = GetUseablePiece();
                        newPiece.MyType = Utility.PickRandom(Utility.GetEnumArray(PieceType.None));
                        newPiece.MyIndex = (ix, col.Count);
                        self.PieceField[ix].Add(newPiece);

                        var pos = self.GetPiecePosition(newPiece.MyIndex) + (Vector3Int.up * 300);
                        newPiece.transform.position = pos;
                        self.RepositionPiece(newPiece, newPiece.MyIndex);
                    }
                }
            }

            stateManager.ChangeState<PieceManagerIdleState>();
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

        private PuzzlePiece GetUseablePiece()
        {
            foreach (var piece in self.PieceList)
            {
                if (piece.MyIndex == (-1, -1))
                {
                    return piece;
                }
            }
            return null;
        }
    }
}