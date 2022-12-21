using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    public Text dialogText;
    public float charactersPerSecond = 25.0f;
    public float timeToWaitAfterText = 1.0f;

    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject movementSelect;
    [SerializeField] private GameObject movementDesc;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> movementTexts;

    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    
    [SerializeField] private Color selectedColor = Color.blue;
    
    public IEnumerator SetDialog(string message)
    {
        dialogText.text = "";
        foreach (var c in message)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }

        yield return new WaitForSeconds(timeToWaitAfterText);
    }

    public void ToggleDialogText(bool activated)
    {
        dialogText.enabled = activated;
    }

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleMovements(bool activated)
    {
        movementSelect.SetActive(activated);
        movementDesc.SetActive(activated);
    }

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = i == selectedAction ? selectedColor : Color.black;
        }
    }

    public void SetPokemonMovements(List<Move> moves)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                movementTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                movementTexts[i].text = "---";
            }
        } 
    }
    
    public void SelectMovement(int selectedMovement, Move move)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].color = i == selectedMovement ? selectedColor : Color.black;
        }
        
        ppText.text = $"PP {move.Pp}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }

}
