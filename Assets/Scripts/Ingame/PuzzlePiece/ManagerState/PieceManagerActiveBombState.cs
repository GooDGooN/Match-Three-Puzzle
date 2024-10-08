
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    private List<PuzzlePiece> removeTargetList = new();
    public class PieceManagerActiveBombState : BaseFSM<PuzzlePieceManager>
    {
        private int count;
        private float delay;
        public override void StateEnter()
        {
            count = 0;
            delay = 0.25f;

            self.removeTargetList.Clear();
/*            !!TEST!! self.StartActiveBomb(self.selectedPuzzlePiece);*/
        }

        public override void StateExit()
        {
        }

        public override void StateFixedUpdate()
        {
        }

        public override void StateUpdate()
        {
            delay -= Time.deltaTime;
            if(count < self.removeTargetList.Count)
            {
                count = self.removeTargetList.Count;
                delay = 0.15f;
            }

            if(delay < 0)
            {
                foreach(PuzzlePiece piece in self.removeTargetList)
                {
                    if(piece.MyIndex != (-1, -1))
                    {
                        piece.RemoveSelf();
                    }
                }
                stateManager.ChangeState<PieceManagerRefillState>();
            }
        }
    }

/*    !!TEST!!
 *    private void StartActiveBomb(PuzzlePiece target)
    {
        StartCoroutine(ActiveBomb(target));
    }

    private IEnumerator ActiveBomb(PuzzlePiece targetPiece)
    {
        var addTuple = (targetPiece.MyType == PieceType.Vbomb) ? (0, 1) : (1, 0);
        var targetTuple = targetPiece.MyIndex;

        var increasedTuple = (targetTuple.Item1 + addTuple.Item1, targetTuple.Item2 + addTuple.Item2);
        var decreasedTuple = (targetTuple.Item1 - addTuple.Item1, targetTuple.Item2 - addTuple.Item2);
        var repeatTime = 0;

        if (targetPiece.MyType == PieceType.Vbomb)
        {
            repeatTime = targetTuple.Item2 > FieldInfo.Height / 2 ? targetTuple.Item2 : FieldInfo.Height - targetTuple.Item2;
        }
        else
        {
            repeatTime = targetTuple.Item1 > FieldInfo.Width / 2 ? targetTuple.Item1 : FieldInfo.Width - targetTuple.Item1;
        }
        var delayTime = 0.15f / repeatTime;
        AddTargetToRemoveList(targetPiece);

        while (repeatTime-- > 0)
        {   
            // check right or up
            if (IsPlaceAreExist(increasedTuple))
            {
                if(!IsPlaceEmpty(increasedTuple))
                {
                    var target = MyPieceField[increasedTuple.Item1, increasedTuple.Item2];
                    var targetType = target.MyType;
                    if (targetPiece.MyType == targetType)
                    {
                        AddTargetToRemoveList(target);
                    }
                    else if (targetType == PieceType.Vbomb || targetType == PieceType.Hbomb)
                    {
                        if(target.MyIndex != (-1, -1))
                        {
                            StartActiveBomb(target);
                        }
                    }
                    else
                    {
                        AddTargetToRemoveList(target);
                    }
                }
                increasedTuple = (increasedTuple.Item1 + addTuple.Item1, increasedTuple.Item2 + addTuple.Item2);
            }

            // check left or down
            if (IsPlaceAreExist(decreasedTuple))
            {
                if(!IsPlaceEmpty(decreasedTuple))
                {
                    var target = MyPieceField[decreasedTuple.Item1, decreasedTuple.Item2];
                    var targetType = target.MyType;
                    if (targetPiece.MyType == targetType)
                    {
                        AddTargetToRemoveList(target);
                    }
                    else if (targetType == PieceType.Vbomb || targetType == PieceType.Hbomb)
                    {
                        if (target.MyIndex != (-1, -1))
                        {
                            StartActiveBomb(target);
                        }
                    }
                    else
                    {
                        AddTargetToRemoveList(target);
                    }
                }
                decreasedTuple = (decreasedTuple.Item1 - addTuple.Item1, decreasedTuple.Item2 - addTuple.Item2);
            }
            yield return new WaitForSeconds(delayTime);
        }
        targetPiece = null;

        void AddTargetToRemoveList(PuzzlePiece target)
        {
            target.transform.position = new Vector2(0, -500.0f);
            removeTargetList.Add(target);
        }
    }*/
}
