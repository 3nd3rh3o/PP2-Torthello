using System;
using Torthello;
using UnityEngine;
using UnityEngine.UIElements;

public class OptionUI : MonoBehaviour
{
    public Action Back;
    public Action Apply;

    public GraphicSettings graphicSettings;



    private EnumField displayModeField;
    private Button ApplyButton;
    private Button BackButton;


    private EventCallback<ChangeEvent<Enum>> displayModeChanged;
    private EventCallback<MouseUpEvent> ApplyEvent;
    private EventCallback<MouseUpEvent> BackEvent;


    void OnEnable()
    {
        displayModeChanged = (evt) => {
            graphicSettings.displayMode = (GraphicSettings.ScreenMode)evt.newValue;
        };
        ApplyEvent = (evt) => Apply();
        BackEvent = (evt) => Back();

        UIDocument uiDoc = GetComponent<UIDocument>();
        VisualElement root = uiDoc.rootVisualElement;

        displayModeField = root.Q<EnumField>("display-mode");
        displayModeField.SetValueWithoutNotify(graphicSettings.displayMode);
        ApplyButton = root.Q<Button>("apply");
        BackButton = root.Q<Button>("back");

        RegisterCallbackEvents();
    }


    void OnDisable()
    {
        UnregisterCallbackEvents();
    }


    void RegisterCallbackEvents()
    {
        displayModeField.RegisterCallback(displayModeChanged);
        ApplyButton.RegisterCallback(ApplyEvent);
        BackButton.RegisterCallback(BackEvent);
    }

    void UnregisterCallbackEvents()
    {
        displayModeField.UnregisterCallback(displayModeChanged);
        ApplyButton.UnregisterCallback(ApplyEvent);
        BackButton.UnregisterCallback(BackEvent);
    }
}
