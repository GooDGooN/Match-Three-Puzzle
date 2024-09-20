using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PuzzlePieceManager 
{
    public class PieceManagerIdleState : BaseFSM<PuzzlePieceManager>
    {
        public override void StateEnter()
        {
            self.Controllable = false;
        }

        public override void StateExit()
        {
            self.Controllable = true;
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            if (Input.GetMouseButtonDown(0) && !self.Controllable)
            {
                var pieceObjs = IngameTouchManager.GetMousePointObjects(1 << 6);
                if (pieceObjs != null)
                {
                    var targetPiece = pieceObjs[0].GetComponent<PuzzlePiece>();
                    if (self.selectedPuzzlePiece == null)
                    {
                        self.selectedPuzzlePiece = targetPiece;
                        self.SelectedIcon.SetActive(true);
                        self.SelectedIcon.transform.position = self.selectedPuzzlePiece.transform.position;
                    }
                    else
                    {
                        self.swapTargetPuzzlePiece = targetPiece;
                        stateManager.ChangeState<PieceManagerSwapPieceState>();
                    }
                }

            }
        }
    }
}
