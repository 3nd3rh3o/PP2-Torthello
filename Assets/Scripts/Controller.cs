using System;
using UnityEngine;

namespace Torthello
{
    public class TorethelloController : MonoBehaviour
    {
        [Tooltip("Various game-objects the controller need access to")]
        public GOHolder gameObjects;
        [Tooltip("Settings of the app and rendering params")]
        public Settings settings;

        private ControlScenario old;
        private ControlScenario activeScenario;

        

        void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            // To have a custom pointer ^^
            //Cursor.SetCursor(null, new(), CursorMode.ForceSoftware);
            Cursor.visible = true;

            




            //Show main menu
            activeScenario = new MainMenuControl();
            activeScenario.Init(this);
        }

        void Update()
        {
            activeScenario?.Update();
        }

        
        public void StartGame()
        {
            Debug.Log("yay!");
        }

        //Hooks
        public void ToMainMenu()
        {
            activeScenario.Reset();
            activeScenario = null;
            activeScenario = new MainMenuControl();
            activeScenario.Init(this);
        }

        public void DrawOptions()
        {
            old = activeScenario;
            activeScenario = new Option();
            activeScenario.Init(this);
        }

        public void OnExitOption()
        {
            activeScenario.Reset();
            activeScenario = null;
            activeScenario = old;
            old = null;
        }

        public void CloseGame()
        {
            activeScenario.Reset();
            activeScenario=null;
            Application.Quit();
            Debug.Log("Game closed!");
        }


        public void ApplyGraphicSettings()
        {
            //Apply graphic settings
            if (settings.GraphicSettings.displayMode == GraphicSettings.ScreenMode.Fullscreen)
            {
                Screen.fullScreen = true;
            } else {
                Screen.fullScreen = false;
            }
        }
    }

    public interface ControlScenario
    {
        void Init(TorethelloController controller);
        void Update();
        void Reset();
    }
}