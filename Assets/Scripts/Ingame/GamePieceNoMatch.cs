using System.Collections;
using UnityEngine;

public class GamePieceNoMatch : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.isPause = true;
        StartCoroutine(SetFree());
    }

    private IEnumerator SetFree()
    {
        var manager = GameManager.Instance;
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
        manager.TimeLimitValue += 0.1f;
        manager.isPause = false;
    }
}
