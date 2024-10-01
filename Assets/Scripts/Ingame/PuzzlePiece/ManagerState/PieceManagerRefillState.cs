using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerRefillState : BaseFSM<PuzzlePieceManager>
    {
        private float delay;
        public override void StateEnter()
        {
            self.repositionedPieceQueue.Clear();
            delay = 0.5f;
        }

        public override void StateExit()
        {
        }

        public override void StateFixedUpdate()
        {
            delay -= Time.deltaTime;
            if (delay < 0.0f)
            {
                for (int ix = 0; ix < self.MyPieceField.Length; ix++)
                {
                    var col = self.MyPieceField[ix];
                    if (col.Count < self.FieldInfo.Height)
                    {
                        for (int iy = 0; iy < col.Count; iy++)
                        {
                            var targetPiece = self.MyPieceField[ix][iy];
                            if (targetPiece.MyIndex.Item2 > iy)
                            {
                                self.RepositionPiece(targetPiece, (ix, iy));
                                self.repositionedPieceQueue.Enqueue(targetPiece);
                            }
                        }

                        while (col.Count != self.FieldInfo.Height)
                        {
                            var newPiece = self.GetUseablePiece();
                            var except = new PieceType[] { PieceType.None, PieceType.Vbomb, PieceType.Hbomb, PieceType.Block };
                            newPiece.MyType = Utility.PickRandom(Utility.GetEnumArray(except));
                            newPiece.MyIndex = (ix, col.Count);
                            self.MyPieceField[ix].Add(newPiece);

                            var pos = self.GetPiecePosition(newPiece.MyIndex) + (Vector3Int.up * 300);
                            newPiece.transform.position = pos;

                            if (col.Count < self.FieldInfo.Height)
                            {
                                self.RepositionPiece(newPiece, newPiece.MyIndex);
                            }
                            else
                            {
                                self.RepositionPiece(newPiece, newPiece.MyIndex, ChangeToMatchable);
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