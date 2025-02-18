using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIRoot : UIBehaviour
{
    private VisualElement root;
    protected override void Start()
    {
        base.Start();
        root = GetComponent<UIDocument>().rootVisualElement;   
    }
}
