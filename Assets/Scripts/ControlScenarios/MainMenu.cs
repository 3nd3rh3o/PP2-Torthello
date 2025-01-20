using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Torthello
{
    public class MainMenuControl : ControlScenario
    {
        private GameObject torus;
        private GameObject ui;
        private InputActionMap actionMap;

        private Action quitCmd;


        public void Init(TorethelloController controller)
        {
            //Take needed GO
            ui = controller.gameObjects.MainMenu;



            //Hook up UI buttons
            MainMenuUI menu = ui.GetComponent<MainMenuUI>();
            menu.StartGame = controller.StartGame;
            menu.Options = controller.DrawOptions;
            menu.Quit = controller.CloseGame;


            //hook up input system
            actionMap = InputSystem.actions.FindActionMap("Menu");

            //hook up refs
            quitCmd = controller.CloseGame;

            //Enable GO
            ui.SetActive(true);

        }
        public void Update()
        {
#if !UNITY_EDITOR
            if (actionMap.FindAction("Close/Exit").WasReleasedThisFrame())
            {
                quitCmd();
            }
#endif
        }

        public void Reset()
        {
            ui.SetActive(false);
        }
    }
}