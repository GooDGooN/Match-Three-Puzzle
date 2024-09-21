using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class PuzzlePieceManager
{
    public class PieceManagerCheckMatchableState : BaseFSM<PuzzlePieceManager>
    {
        private float delay;
        private bool isRefill;
        public override void StateEnter()
        {
            delay = 0.1f;
            isRefill = false;

        }

        public override void StateExit()
        {

        }

        public override void StateFixedUpdate()
        {
            delay -= Time.deltaTime;
            if(delay < 0.0f)
            {
                while (self.repositionedPieceQueue.Count > 0)
                {
                    var target = self.repositionedPieceQueue.Dequeue();
                    if (target.MyIndex != (-1, -1))
                    {
                        var matchables = self.GetMatchablePieces(target);
                        if (matchables.Length > 0)
                        {
                            foreach (var piece in matchables)
                            {
                                if(piece.MyIndex != (-1, -1))
                                {
                                    self.PieceField[piece.MyIndex.Item1].Remove(piece);
                                    piece.MyIndex = (-1, -1);
                                    piece.transform.position = new Vector2(0, -500.0f);
                                }
                                isRefill = true;
                            }
                        }
                    }
                }

                if (isRefill)
                {
                    stateManager.ChangeState<PieceManagerRefillState>();
                }
                else
                {
                    stateManager.ChangeState<PieceManagerIdleState>();
                }
            }
        }

        public override void StateUpdate()
        {
        }
    }

}