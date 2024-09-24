using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerCheckMatchableState : BaseFSM<PuzzlePieceManager>
    {
        private float delay;
        private bool isRefill;
        private Queue<PuzzlePiece> removePieceQueue;
        public override void StateEnter()
        {
            removePieceQueue = new();
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
                    var matchables = self.GetMatchablePieces(target);
                    if (matchables.Length > 0)
                    {
                        isRefill = true;
                        foreach (var piece in matchables)
                        {
                            removePieceQueue.Enqueue(piece);
                            piece.transform.position = new Vector2(0, -500.0f);
                        }
                    }
                }

                while(removePieceQueue.Count > 0)
                {
                    var piece = removePieceQueue.Dequeue();
                    if (piece.MyIndex != (-1, -1))
                    {
                        self.PieceField[piece.MyIndex.Item1].Remove(piece);
                        piece.MyIndex = (-1, -1);
                    }
                }

                if (isRefill)
                {
                    stateManager.ChangeState<PieceManagerRefillState>();
                    return;
                }
                stateManager.ChangeState<PieceManagerCheckAllFieldState>();
            }
        }

        public override void StateUpdate()
        {
        }
    }

}