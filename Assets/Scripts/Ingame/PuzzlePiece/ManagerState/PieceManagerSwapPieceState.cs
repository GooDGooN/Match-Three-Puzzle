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

            self.RepositionPiece(self.swapTargetPuzzlePiece, selectedIndex);
            self.RepositionPiece(self.selectedPuzzlePiece, targetIndex, CheckAtferSwap);
        }

        public override void StateExit()
        {
            self.SelectedIcon.SetActive(false);
            self.swapTargetPuzzlePiece = null;
            self.selectedPuzzlePiece = null;
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
        }

        private void CheckAtferSwap()
        {
            var matchableList = self.GetMatchablePieces(self.selectedPuzzlePiece).ToList();
            matchableList.AddRange(self.GetMatchablePieces(self.swapTargetPuzzlePiece));
            if (matchableList.Count > 0)
            {
                foreach (var piece in matchableList)
                {
                    self.PieceField[piece.MyIndex.Item1].Remove(piece);
                    piece.MyIndex = (-1, -1);
                    piece.transform.position = new Vector2(0, -500.0f);
                    // Change to Refill State
                }
            }
            else
            {
                self.RepositionPiece(self.swapTargetPuzzlePiece, targetIndex);
                self.RepositionPiece(self.selectedPuzzlePiece, selectedIndex, GoBackAtferSwap);
            }
        }
        private void GoBackAtferSwap()
        {
            stateManager.ChangeState<PieceManagerIdleState>();
        }

    }

}