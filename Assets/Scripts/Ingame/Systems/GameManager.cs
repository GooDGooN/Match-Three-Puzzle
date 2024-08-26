using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int PieceFieldWidth = 10;
    public const int PieceFieldHeight = 10;
    public static readonly Vector3Int PieceFieldSize = new Vector3Int(10, 10);
    public const int PieceSize = 36;
    public const int PieceTypeAmount = 7;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
