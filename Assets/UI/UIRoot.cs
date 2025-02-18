using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tortello
{
    public class UIRoot : MonoBehaviour
    {
        private UIDocument uiDocument;
        private Button button;
        // action a bind a un boutton.
        private readonly Action quit = () => Application.Quit();

        public Settings settings;
        void OnEnable()
        {
            uiDocument = GetComponent<UIDocument>();

            VisualElement root = uiDocument.rootVisualElement;
            // register de l'action au boutton Menu_Exit_Button.
            root.Q<Button>("Menu_Exit_button").clicked += quit;
        }
        void Update()
        {

            if (settings.m_fullscreen.IsDirty())
            {
                if (settings.m_fullscreen.GetValue()) Screen.fullScreen = true;
                else Screen.fullScreen = false;
                settings.m_fullscreen.Proccesed();
            }
        }

        void OnDisable()
        {
            VisualElement root = uiDocument.rootVisualElement;
            //Unregister, comme ça on peut switch les callbackevent sans problème.
            root.Q<Button>("Menu_Exit_button").clicked -= quit;
        }
    }
}
