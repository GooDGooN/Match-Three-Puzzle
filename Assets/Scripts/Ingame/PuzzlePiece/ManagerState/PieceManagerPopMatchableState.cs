using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    private List<PuzzlePiece> removeTargetList = new();
    private float stateChangeDelay;
    public class PieceManagerPopMatchableState : BaseFSM<PuzzlePieceManager>
    {
        private bool isRefill;
        private List<PuzzlePiece[]> removePiecesList;
        public override void StateEnter()
        {
            removePiecesList = new();
            self.stateChangeDelay = 0.0f;
            isRefill = false;

            while (self.repositionedPieceQueue.Count > 0)
            {
                var target = self.repositionedPieceQueue.Dequeue();
                var matchables = self.GetMatchablePieces(target);
                if (matchables.Length > 0)
                {
                    removePiecesList.Add(matchables);
                }
            }

            removePiecesList = removePiecesList.OrderBy(arr => arr.Length).ToList();
            PuzzlePiece specialPiece = null;

            foreach (var matchables in removePiecesList)
            {

                specialPiece = null;
                var matchLength = matchables.Length;
                if (matchLength > 3 && matchables.Count(arr => arr.MyIndex == (-1, -1)) == 0)
                {
                    // set the special piece
                    if (matchables.Contains(self.selectedPuzzlePiece))
                    {
                        specialPiece = self.selectedPuzzlePiece;
                    }
                    else if (matchables.Contains(self.swapTargetPuzzlePiece))
                    {
                        specialPiece = self.swapTargetPuzzlePiece;
                    }
                    else
                    {
                        specialPiece = matchables[Random.Range(0, matchables.Length)];
                    }

                    // set special piece type
                    if (matchLength >= 6)
                    {
                        specialPiece.TargetChangeType = PieceType.Rainbow;
                    }
                    else if (matchLength == 5)
                    {
                        if (matchables.Count(piece => piece.MyIndex.Item1 == matchables[0].MyIndex.Item1) == 5 ||
                            matchables.Count(piece => piece.MyIndex.Item2 == matchables[0].MyIndex.Item2) == 5)
                        {
                            specialPiece.TargetChangeType = PieceType.Rainbow;
                        }
                        else
                        {
                            specialPiece.TargetChangeSubType = PieceSubType.CrossBomb;
                        }
                    }
                    else if (matchLength == 4)
                    {
                        if (matchables[0].MyIndex.Item1 == matchables[1].MyIndex.Item1)
                        {
                            specialPiece.TargetChangeSubType = PieceSubType.Hbomb;
                        }
                        else if (matchables[0].MyIndex.Item2 == matchables[1].MyIndex.Item2)
                        {
                            specialPiece.TargetChangeSubType = PieceSubType.Vbomb;
                        }
                    }
                    specialPiece.TargetChangeType = specialPiece.MyType;
                }

                specialPiece?.MyAnimator.SetTrigger("ChangeType");
                foreach (var piece in matchables)
                {
                    if (piece != specialPiece && piece.MyIndex != (-1, -1))
                    {
                        isRefill = true;
                        piece.RemoveSelf();
                    }
                }
            }
        }

        public override void StateExit()
        {
            self.swapTargetPuzzlePiece = null;
            self.selectedPuzzlePiece = null;
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            self.stateChangeDelay -= Time.deltaTime;
            if (self.stateChangeDelay < 0.0f)
            {
                if (isRefill)
                {
                    stateManager.ChangeState<PieceManagerRefillState>();
                    return;
                }
                stateManager.ChangeState<PieceManagerFindMatchableState>();
            }
        }
    }

        {
        }
    }

}