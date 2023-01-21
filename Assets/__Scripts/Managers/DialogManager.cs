using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
   public static DialogManager SharedInstance;
   
   [SerializeField] private GameObject dialogBox;
   [SerializeField] private Text dialogText;

   [SerializeField] private int charactersPerSecond = 25;

   public event Action OnDialogStart, OnDialogFinish;
   private int currentLine = 0;
   private Dialog actualDialog;
   private bool isWriting = false;

   private void Awake()
   {
      if (SharedInstance == null)
      {
         SharedInstance = this;
      }
   }

   public void ShowDialog(Dialog dialog)
   {
      OnDialogStart?.Invoke();
      dialogBox.SetActive(true);
      actualDialog = dialog;
      StartCoroutine(SetDialog(dialog.Lines[0]));
   }
   
   public IEnumerator SetDialog(string message)
   {
      isWriting = true;
      dialogText.text = "";
      foreach (var c in message)
      {
         dialogText.text += c;
         yield return new WaitForSeconds(1 / charactersPerSecond);
      }

      isWriting = false;
   }

   public void HandleUpdate()
   {
      if (Input.GetKeyDown(KeyCode.Return) && !isWriting)
      {
         currentLine++;
         if (currentLine >= actualDialog.Lines.Count)
         {
            dialogBox.SetActive(false);
            currentLine = 0;
            OnDialogFinish?.Invoke();
         }
         else
         {
            StartCoroutine(SetDialog(actualDialog.Lines[currentLine]));
         }
      }
   }
}
