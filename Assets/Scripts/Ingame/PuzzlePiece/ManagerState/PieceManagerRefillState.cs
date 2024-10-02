using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerRefillState : BaseFSM<PuzzlePieceManager>
    {
        private float delay;
        public override void StateEnter()
        {
            self.repositionedPieceQueue.Clear();
            delay = 0.25f;
        }

        public override void StateExit()
        {
        }

        public override void StateFixedUpdate()
        {
            delay -= Time.deltaTime;
            if (delay < 0.0f)
            {
                for (int ix = 0; ix < self.FieldInfo.Width; ix++)
                {
                    var col = self.MyPieceField[ix];
                    var nullCount = col.Count(piece => piece == null);
                    if (nullCount > 0)
                    {
                        for (int iy = 0; iy < col.Length; iy++)
                        {
                            var targetPiece = self.MyPieceField[ix, iy];
                            if(targetPiece != null)
                            {
                                if (targetPiece.MyIndex.Item2 > iy)
                                {
                                    self.RepositionPiece(targetPiece, (ix, iy), PieceRepositionType.Refill);
                                    self.repositionedPieceQueue.Enqueue(targetPiece);
                                }
                            }
                        }

                        var nullYpos = self.MyPieceField.GetNullYPos(ix);
                        while (nullYpos != -1)
                        {
                            var newPiece = self.GetUseablePiece();
                            var except = new PieceType[] { PieceType.None, PieceType.Vbomb, PieceType.Hbomb, PieceType.Block };
                            newPiece.MyType = Utility.PickRandom(Utility.GetEnumArray(except));
                            newPiece.MyIndex = (ix, nullYpos);
                            self.MyPieceField[ix, nullYpos] = newPiece;

                            var pos = self.GetPiecePosition(newPiece.MyIndex) + (Vector3Int.up * 300);
                            newPiece.transform.position = pos;

                            nullYpos = self.MyPieceField.GetNullYPos(ix);
                            if (nullYpos == -1)
                            {
                                self.RepositionPiece(newPiece, newPiece.MyIndex, PieceRepositionType.Refill, ChangeToMatchable);
                            }
                            else
                            {
                                self.RepositionPiece(newPiece, newPiece.MyIndex, PieceRepositionType.Refill);
                            }
                            self.repositionedPieceQueue.Enqueue(newPiece);
                        }
                    }
                }
            }
        }

        public override void StateUpdate()
        {
        }

        private void ChangeToMatchable()
        {
            stateManager.ChangeState<PieceManagerPopMatchableState>();
        }
    }
}