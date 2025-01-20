using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Torthello
{
    public class Option : ControlScenario
    {
        private GameObject optionPopUp;
        private InputActionMap actionMap;
        private Action backCMD;

        public void Init(TorethelloController controller)
        {
            optionPopUp = controller.gameObjects.OptionsPopup;


            OptionUI ui = optionPopUp.GetComponent<OptionUI>();
            
            ui.graphicSettings = controller.settings.GraphicSettings;
            ui.Apply = controller.ApplyGraphicSettings;
            ui.Back = controller.OnExitOption;

            actionMap = InputSystem.actions.FindActionMap("Menu");


            backCMD = controller.OnExitOption;

            optionPopUp.SetActive(true);
        }

        public void Reset()
        {
            optionPopUp.SetActive(false);
        }

        public void Update()
        {
#if !UNITY_EDITOR
            if (actionMap.FindAction("Close/Exit").WasReleasedThisFrame())
            {
                backCMD();
            }
#endif
        }
    }
}