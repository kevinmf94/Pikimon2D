using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers SharedInstance;

    [SerializeField]
    private LayerMask solidObjectLayers, pokemonZone, interactableLayer;

    public LayerMask SolidObjectsLayer => solidObjectLayers;
    public LayerMask PokemonLayer => pokemonZone;
    public LayerMask InteractableLayer => interactableLayer;

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
    }
}
