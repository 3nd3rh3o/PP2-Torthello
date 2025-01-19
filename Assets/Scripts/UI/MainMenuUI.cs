using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public Action StartGame;
    public Action Options;
    public Action Quit;
    

    private Button StartButton;
    private Button OptionButton;
    private Button QuitButton;

    private EventCallback<MouseUpEvent> StartGameEvent;
    private EventCallback<MouseUpEvent> OptionEvent;
    private EventCallback<MouseUpEvent> QuitGameEvent;

    private void OnEnable()
    {
        StartGameEvent = (evt) => StartGame();
        OptionEvent = (evt) => Options();
        QuitGameEvent = (evt) => Quit();


        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;


        //Get ref to buttons
        StartButton = root.Q<Button>("start");
        OptionButton = root.Q<Button>("option");
        QuitButton = root.Q<Button>("quit");

        RegisterCallbackEvents();
    }

    private void OnDisable()
    {
        UnregisterCallbackEvents();
    }

    private void RegisterCallbackEvents()
    {
        StartButton.RegisterCallback(StartGameEvent);
        OptionButton.RegisterCallback(OptionEvent);
        QuitButton.RegisterCallback(QuitGameEvent);

    }

    private void UnregisterCallbackEvents()
    {
        StartButton.UnregisterCallback(StartGameEvent);
        OptionButton.UnregisterCallback(OptionEvent);
        QuitButton.UnregisterCallback(QuitGameEvent);
    }
}
