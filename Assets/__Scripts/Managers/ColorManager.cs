using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorManager : MonoBehaviour
{
   public static ColorManager SharedInstance;

   public Color selectedColor = Color.blue;
   public Color black { get; } = Color.black;

   private void Awake()
   {
      SharedInstance = this;
   }
}
