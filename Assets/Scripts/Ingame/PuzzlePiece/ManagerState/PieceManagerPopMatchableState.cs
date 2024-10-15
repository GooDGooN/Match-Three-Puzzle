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
            self.stateChangeDelay = 0.1f;
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
                            Debug.Log("rainbow");
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

    private void StartActiveBomb(PuzzlePiece target)
    {
        switch(target.MySubType)
        {
            case PieceSubType.Vbomb:
                StartCoroutine(ActiveVerticalBomb(target));
                break;
            case PieceSubType.Hbomb:
                StartCoroutine(ActiveHorizontalBomb(target));
                break;
            case PieceSubType.CrossBomb:
                StartCoroutine(ActiveVerticalBomb(target));
                StartCoroutine(ActiveHorizontalBomb(target));
                break;
        }
        stateChangeDelay = 0.16f;
        target.RemoveSelf();
    }

    private IEnumerator ActiveHorizontalBomb(PuzzlePiece targetPiece)
    {
        var targetTuple = targetPiece.MyIndex;

        var increasedTuple = (targetTuple.Item1 + 1, targetTuple.Item2);
        var decreasedTuple = (targetTuple.Item1 - 1, targetTuple.Item2);
        var repeatTime = targetTuple.Item1 > FieldInfo.Width / 2 ? targetTuple.Item1 : FieldInfo.Width - targetTuple.Item1;
        var delayTime = 0.15f / repeatTime;
        while (repeatTime-- > 0)
        {
            // check right
            if (IsPlaceAreExist(increasedTuple) && !IsPlaceEmpty(increasedTuple))
            {
                var target = MyPieceField[increasedTuple.Item1, increasedTuple.Item2];
                if (target.MyIndex == increasedTuple)
                {
                    var targetSubType = target.MySubType;
                    if (targetSubType == PieceSubType.Vbomb)
                    {
                        if (target.MyIndex != (-1, -1))
                        {
                            StartActiveBomb(target);
                        }
                    }
                    else
                    {
                        target.RemoveSelf();
                    }
                }
                increasedTuple = (increasedTuple.Item1 + 1, increasedTuple.Item2);
            }

            // check left
            if (IsPlaceAreExist(decreasedTuple) && !IsPlaceEmpty(decreasedTuple))
            {
                var target = MyPieceField[decreasedTuple.Item1, decreasedTuple.Item2];
                if (target.MyIndex == decreasedTuple)
                {
                    var targetSubType = target.MySubType;
                    if (targetSubType == PieceSubType.Vbomb)
                    {
                        if (target.MyIndex != (-1, -1))
                        {
                            StartActiveBomb(target);
                        }
                    }
                    else
                    {
                        target.RemoveSelf();
                    }
                }
                decreasedTuple = (decreasedTuple.Item1 - 1, decreasedTuple.Item2);
            }
            yield return new WaitForSeconds(delayTime);
        }
    }

    private IEnumerator ActiveVerticalBomb(PuzzlePiece targetPiece)
    {
        var targetTuple = targetPiece.MyIndex;

        var increasedTuple = (targetTuple.Item1, targetTuple.Item2);
        var decreasedTuple = (targetTuple.Item1, targetTuple.Item2 - 1);
        var repeatTime = targetTuple.Item2 > FieldInfo.Height / 2 ? targetTuple.Item2 : FieldInfo.Height - targetTuple.Item2;
        var delayTime = 0.15f / repeatTime;
        while (repeatTime-- > 0)
        {
            // check up
            if (IsPlaceAreExist(increasedTuple) && !IsPlaceEmpty(increasedTuple))
            {
                var target = MyPieceField[increasedTuple.Item1, increasedTuple.Item2];
                if (target.MyIndex == increasedTuple)
                {
                    var targetSubType = target.MySubType;
                    if (targetSubType == PieceSubType.Hbomb)
                    {
                        if (target.MyIndex != (-1, -1))
                        {
                            StartActiveBomb(target);
                        }
                    }
                    else
                    {
                        target.RemoveSelf();
                    }
                }
            }

            // check down
            if (IsPlaceAreExist(decreasedTuple) && !IsPlaceEmpty(decreasedTuple))
            {
                var target = MyPieceField[decreasedTuple.Item1, decreasedTuple.Item2];
                if (target.MyIndex == decreasedTuple)
                {
                    var targetSubType = target.MySubType;
                    if (targetSubType == PieceSubType.Hbomb)
                    {
                        if (target.MyIndex != (-1, -1))
                        {
                            StartActiveBomb(target);
                        }
                    }
                    else
                    {
                        target.RemoveSelf();
                    }
                }
                decreasedTuple = (decreasedTuple.Item1, decreasedTuple.Item2 - 1);
                increasedTuple = (decreasedTuple.Item1, decreasedTuple.Item2 + 1);
            }
            yield return new WaitForSeconds(delayTime);
        }
    }
}