
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PuzzlePieceManager
{
    public class PieceManagerPopRainbowState : BaseFSM<PuzzlePieceManager>
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

}
