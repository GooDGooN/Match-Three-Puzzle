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
        private bool isStateChange;
        public override void StateEnter()
        {
            isStateChange = false;
            self.repositionedPieceQueue.Clear();
            delay = 0.01f;
        }

        public override void StateExit()
        {
        }

        public override void StateFixedUpdate()
        {
            delay -= Time.deltaTime;
            var played = false;
            if (delay < 0.0f)
            {
                if (isStateChange)
                {
                    stateManager.ChangeState<PieceManagerPopMatchedState>();
                    return;
                }

                for (int ix = 0; ix < self.FieldInfo.Width; ix++)
                {
                    if (self.MyPieceField.GetNullYPos(ix) != -1)
                    {
                        for (int iy = 0; iy < self.MyPieceField[ix].Length; iy++)
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
                            var except = new PieceType[] { PieceType.None, PieceType.Block, PieceType.Rainbow };
                            newPiece.MyType = Utility.PickRandom(Utility.GetEnumArray(except));
                            newPiece.MyIndex = (ix, nullYpos);
                            self.MyPieceField[ix, nullYpos] = newPiece;

                            var pos = self.GetPiecePosition(newPiece.MyIndex) + (Vector3Int.up * 300);
                            newPiece.transform.position = pos;

                            nullYpos = self.MyPieceField.GetNullYPos(ix);
                            if (!played)
                            {
                                self.RepositionPiece(newPiece, newPiece.MyIndex, PieceRepositionType.Refill, ChangeToPop);
                                played = true;
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

        private void ChangeToPop()
        {
            isStateChange = true;
            delay = 0.1f;
        }
    }
}