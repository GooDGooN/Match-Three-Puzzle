using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
    public PuzzlePiece[,] PuzzlePieces;

    private void Awake()
    {
        PuzzlePieces = new PuzzlePiece[GameManager.PieceFieldSize.x, GameManager.PieceFieldSize.y];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
