
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerActiveBombState : BaseFSM<PuzzlePieceManager>
    {
        public override void StateEnter()
        {
            self.StartBombardment();
        }

        public override void StateExit()
        {
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
        }
    }

    private void StartBombardment()
    {
        StartCoroutine(Bombardment());
    }

    private IEnumerator Bombardment()
    {
        var addTuple = (selectedPuzzlePiece.MyType == PieceType.Vbomb) ? (0, 1) : (1, 0);
        var pieceTuple = selectedPuzzlePiece.MyIndex;
        
        selectedPuzzlePiece.RemoveSelf();

        var increasedTuple = (pieceTuple.Item1 + addTuple.Item1, pieceTuple.Item2);
        var decreasedTuple = (pieceTuple.Item1 - addTuple.Item1, pieceTuple.Item2 - addTuple.Item2);
        var breakable = false;
        var repeatTime = 0;

        if (selectedPuzzlePiece.MyType == PieceType.Vbomb)
        {
            repeatTime = pieceTuple.Item2 > FieldInfo.Height / 2 ? pieceTuple.Item2 : FieldInfo.Height - pieceTuple.Item2;
        }
        else
        {
            repeatTime = pieceTuple.Item1 > FieldInfo.Width / 2 ? pieceTuple.Item1 : FieldInfo.Width - pieceTuple.Item1;
        }
        var delayTime = 0.25f / repeatTime;

        while (!breakable)
        {
            breakable = true;
            yield return new WaitForSeconds(delayTime);
            if (IsPlaceAreExist(increasedTuple) && !IsPlaceEmpty(increasedTuple))
            {
                PieceField[increasedTuple.Item1][increasedTuple.Item2].RemoveSelf();
                increasedTuple = (increasedTuple.Item1 + addTuple.Item1, increasedTuple.Item2);
                breakable = false;
            }
            if (IsPlaceAreExist(decreasedTuple) && !IsPlaceEmpty(decreasedTuple))
            {
                PieceField[decreasedTuple.Item1][decreasedTuple.Item2].RemoveSelf();
                decreasedTuple = (decreasedTuple.Item1 - addTuple.Item1, decreasedTuple.Item2 - addTuple.Item2);
                if (selectedPuzzlePiece.MyType == PieceType.Vbomb)
                {
                    increasedTuple.Item2 -= 1;
                }
                breakable = false;
            }
        }
        selectedPuzzlePiece = null;
        myStateController.ChangeState<PieceManagerRefillState>();
    }
}
