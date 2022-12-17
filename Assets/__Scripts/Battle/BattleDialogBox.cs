using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    public Text dialogText;
    public float charactersPerSecond = 25.0f;

    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject movementSelect;
    [SerializeField] private GameObject movementDesc;

    public IEnumerator SetDialog(string message)
    {
        dialogText.text = "";
        foreach (var c in message)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
    }

}
