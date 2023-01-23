using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{

    [SerializeField] private Dialog dialog;
    
    public void Interact()
    {
        DialogManager.SharedInstance.ShowDialog(dialog);
    }
    
}
