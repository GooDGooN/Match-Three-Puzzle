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
            self.selectedPuzzlePiece = null;
            self.swapTargetPuzzlePiece = null;
        }

        public override void StateExit()
        {
            self.SetHintCountDown(true);
            self.SelectedIcon.SetActive(false);
            self.HintPieceList.ForEach(piece => piece.MyAnimator.SetBool("Hint", false));
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 6 = PuzzlePiece
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
                        if (targetPiece == self.selectedPuzzlePiece)
                        {
                            self.selectedPuzzlePiece = null;
                            self.SelectedIcon.SetActive(false);
                        }
                        else if (self.selectedPuzzlePiece.GetNearPieces().Contains(targetPiece))
                        {
                            self.swapTargetPuzzlePiece = targetPiece;
                            stateManager.ChangeState<PieceManagerSwapPieceState>();
                        }
                        else
                        {
                            self.selectedPuzzlePiece = targetPiece;
                            self.SelectedIcon.transform.position = self.selectedPuzzlePiece.transform.position;
                        }
                    }
                }
                else
                {
                    self.selectedPuzzlePiece = null;
                    self.SelectedIcon.SetActive(false);
                }

            }
        }
    }
    private void SetHintCountDown(bool stopCoroutine = false)
    {
        if (stopCoroutine && hintCountDownCoroutine != null)
        {
            StopCoroutine(hintCountDownCoroutine);
            return;
        }
        hintCountDownCoroutine = StartCoroutine(HintCountDown());
    }
    private IEnumerator HintCountDown()
    {
        yield return new WaitForSeconds(4.0f);
        if (myStateController.CurrentState.GetType() == typeof(PieceManagerIdleState))
        {
            HintPieceList.ForEach(piece => piece.MyAnimator.SetBool("Hint", true));
        }
    }
}
