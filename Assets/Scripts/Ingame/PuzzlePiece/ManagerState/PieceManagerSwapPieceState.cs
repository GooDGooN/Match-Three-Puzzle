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
            if(self.selectedPuzzlePiece.MyType == PieceType.Rainbow ^ self.swapTargetPuzzlePiece.MyType == PieceType.Rainbow)
            {
                stateManager.ChangeState<PieceManagerPopMatchedState>();
                return;
            }

            if (self.GetMatchablePieces(self.selectedPuzzlePiece).Length > 0)
            {
                self.repositionedPieceQueue.Enqueue(self.selectedPuzzlePiece);
            }

            if (self.GetMatchablePieces(self.swapTargetPuzzlePiece).Length > 0)
            {
                self.repositionedPieceQueue.Enqueue(self.swapTargetPuzzlePiece);
            }

            if (self.repositionedPieceQueue.Count > 0)
            {
                stateManager.ChangeState<PieceManagerPopMatchedState>();
            }
            else
            {
                self.RepositionPiece(self.swapTargetPuzzlePiece, targetIndex, PieceRepositionType.Swap);
                self.RepositionPiece(self.selectedPuzzlePiece, selectedIndex, PieceRepositionType.Swap, GoBackAtferSwap);
            }
        }
        private void GoBackAtferSwap()
        {
            stateManager.ChangeState<PieceManagerIdleState>();
        }

    }

}