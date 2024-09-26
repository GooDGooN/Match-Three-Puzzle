using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager 
{
    private Coroutine hintCountDownCoroutine;
    public class PieceManagerIdleState : BaseFSM<PuzzlePieceManager>
    {
        public override void StateEnter()
        {
            self.SetHintCountDown();
        }

        public override void StateExit()
        {
            self.SetHintCountDown(true);
            self.HintPieceList.ForEach(piece => piece.MyAnimator.SetBool("Hint", false));
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
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
                        if (self.selectedPuzzlePiece.GetNearPieces().Contains(targetPiece))
                        {
                            self.swapTargetPuzzlePiece = targetPiece;
                            stateManager.ChangeState<PieceManagerSwapPieceState>();
                        }
                        else
                        {
                            self.selectedPuzzlePiece = null;
                            self.SelectedIcon.SetActive(false);
                        }
                    }
                }

            }
        }
    }
}
