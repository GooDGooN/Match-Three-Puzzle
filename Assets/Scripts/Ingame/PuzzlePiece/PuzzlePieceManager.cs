using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceManager : MonoBehaviour
{
    public PuzzlePiece[,] PuzzlePieces;
    public GameObject PuzzlePieceContainer;

    private void Awake()
    {
        PuzzlePieces = new PuzzlePiece[GameManager.PieceFieldWidth, GameManager.PieceFieldHeight];
        PuzzlePieceContainer = new GameObject("PuzzlePieceContainer");
        PuzzlePieceContainer.transform.parent = transform;
    } 

    // Update is called once per frame
    private void Update()
    {
        
    }
}
