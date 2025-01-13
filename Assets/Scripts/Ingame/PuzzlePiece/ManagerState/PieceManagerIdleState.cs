using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager 
{
    private Coroutine hintCountDownCoroutine;
    public class PieceManagerIdleState : BaseFSM<PuzzlePieceManager>
    {
        private Vector3 inputPos;
        private Vector3 inputResetPos;
        public override void StateEnter()
        {
            inputResetPos = Vector3.one * -1;
            inputPos = inputResetPos;
            self.SetHintCountDown();
            self.selectedPuzzlePiece = null;
            self.swapTargetPuzzlePiece = null;
            GameManager.Instance.Combo = 0;
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
            if (GameManager.Instance.TimeLimitValue < 0)
            {
                GameManager.Instance.TimeOver();
            }
            if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isPause)
            {
                // 6 = PuzzlePiece
                var pieceObjs = IngameTouchManager.GetMousePointObjects(1 << 6);
                if (pieceObjs != null)
                {
                    var targetPiece = pieceObjs[0].GetComponent<PuzzlePiece>();

                    if (self.selectedPuzzlePiece == null)
                    {
                        inputPos = Camera.main.WorldToScreenPoint(Input.mousePosition);
                        self.selectedPuzzlePiece = targetPiece;
                        self.SelectedIcon.SetActive(true);
                        self.SelectedIcon.transform.position = self.selectedPuzzlePiece.transform.position;
                    }
                    else
                    {
                        if (targetPiece == self.selectedPuzzlePiece)
                        {
                            inputPos = inputResetPos;
                            self.selectedPuzzlePiece = null;
                            self.SelectedIcon.SetActive(false);
                        }
                        else if (self.selectedPuzzlePiece.GetNearPieces().Contains(targetPiece))
                        {
                            self.swapTargetPuzzlePiece = targetPiece;
                            stateManager.ChangeState<PieceManagerSwapPieceState>();
                            return;
                        }
                        else
                        {
                            inputPos = Camera.main.WorldToScreenPoint(Input.mousePosition);
                            self.selectedPuzzlePiece = targetPiece;
                            self.SelectedIcon.transform.position = self.selectedPuzzlePiece.transform.position;
                        }
                    }
                }
                else
                {
                    inputPos = inputResetPos;
                    self.selectedPuzzlePiece = null;
                    self.SelectedIcon.SetActive(false);
                }
            }

            if(Input.GetMouseButton(0))
            {
                if (self.selectedPuzzlePiece != null)
                {
                    var currentPos = Camera.main.WorldToScreenPoint(Input.mousePosition);
                    var delta = inputPos - currentPos;
                    var targetTuple = self.selectedPuzzlePiece.MyIndex;
                    var dotest = true;
                    if (delta.x > self.PieceSize)
                    {
                        targetTuple.Item1 -= 1;
                    }
                    else if (delta.x < -self.PieceSize)
                    {
                        targetTuple.Item1 += 1;
                    }
                    else if (delta.y > self.PieceSize)
                    {
                        targetTuple.Item2 -= 1;
                    }
                    else if (delta.y < -self.PieceSize)
                    {
                        targetTuple.Item2 += 1;
                    }
                    else
                    {
                        dotest = false;
                    }

                    if(dotest)
                    {
                        if(self.IsPlaceAreExist(targetTuple) && !self.IsPlaceEmpty(targetTuple))
                        {
                            self.swapTargetPuzzlePiece = self.MyPieceField[targetTuple.Item1, targetTuple.Item2];
                            stateManager.ChangeState<PieceManagerSwapPieceState>();
                        }
                    }

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
        while (GameManager.Instance.isPause)
        {
            yield return null;
        }

        yield return new WaitForSeconds(4.0f);

        if (myStateController.CurrentState.GetType() == typeof(PieceManagerIdleState))
        {
            HintPieceList.ForEach(piece => piece.MyAnimator.SetBool("Hint", true));
        }
    }
}
