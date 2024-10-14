using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerSwapPieceState : BaseFSM<PuzzlePieceManager>
    {
        private Vector3 selectedPos;
        private Vector3 targetPos;
        private (int, int) selectedIndex;
        private (int, int) targetIndex;

        public override void StateEnter()
        {
            selectedPos = self.selectedPuzzlePiece.transform.position;
            targetPos = self.swapTargetPuzzlePiece.transform.position;
            selectedIndex = self.selectedPuzzlePiece.MyIndex;
            targetIndex = self.swapTargetPuzzlePiece.MyIndex;

            self.RepositionPiece(self.swapTargetPuzzlePiece, selectedIndex, PieceRepositionType.Swap);
            self.RepositionPiece(self.selectedPuzzlePiece, targetIndex, PieceRepositionType.Swap, CheckAtferSwap);
        }

        public override void StateExit()
        {
            self.SelectedIcon.SetActive(false);
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
        }

        private void CheckAtferSwap()
        {
            var selected = self.selectedPuzzlePiece;
            var swapTarget = self.swapTargetPuzzlePiece;

            if (self.GetMatchablePieces(self.selectedPuzzlePiece).Length > 0)
            {
                self.repositionedPieceQueue.Enqueue(self.selectedPuzzlePiece);
            }
            else
            {
                self.selectedPuzzlePiece = null;
            }

            if (self.GetMatchablePieces(self.swapTargetPuzzlePiece).Length > 0)
            {
                self.repositionedPieceQueue.Enqueue(self.swapTargetPuzzlePiece);
            }
            else
            {
                self.swapTargetPuzzlePiece = null;
            }

            if(self.repositionedPieceQueue.Count > 0)
            {
                stateManager.ChangeState<PieceManagerPopMatchableState>();
            }
            else
            {
                self.RepositionPiece(swapTarget, targetIndex, PieceRepositionType.Swap);
                self.RepositionPiece(selected, selectedIndex, PieceRepositionType.Swap, GoBackAtferSwap);
            }
        }
        private void GoBackAtferSwap()
        {
            stateManager.ChangeState<PieceManagerIdleState>();
        }

    }

}