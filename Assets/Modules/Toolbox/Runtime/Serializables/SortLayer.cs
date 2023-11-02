using System;
using UnityEngine;

/// <summary>
/// Use this, if you want a selectable dropdown for Unity's SortingLayers in the Inspector.
/// </summary>
[Serializable]
public class SortLayer
{
    [SerializeField] private string _name;
    [SerializeField] private int _id;

    public string Name => _name;
    public int Id => _id;
}