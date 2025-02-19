using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tortello
{
    public class UIRoot : MonoBehaviour
    {
        public VisualTreeAsset MenuAsset;
        private TemplateContainer menu;
        public VisualTreeAsset MenuOptionAsset;
        private TemplateContainer option;
        private UIDocument uiDocument;
        // action a bind aux bouttons
        private readonly Action menu_quit = () => Application.Quit();
        private Action menu_option;
        private Action option_menu;





        public Settings settings;
        void OnEnable()
        {
            menu_option = () =>
            {
                EnableOptionFromMenu();
            };

            option_menu = () =>
            {
                DisableOption();
                EnableMenu();
            };

            uiDocument = GetComponent<UIDocument>();
            menu = MenuAsset.Instantiate();
            option = MenuOptionAsset.Instantiate();


            EnableMenu();
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
            DisableMenu();
        }

        void EnableMenu()
        {
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(menu);

            // register de l'action au boutton Menu_Exit_Button.
            root.Q<Button>("Menu_Exit_button").clicked += menu_quit;
            root.Q<Button>("Menu_Option_button").clicked += menu_option;

        }

        void DisableMenu()
        {
            VisualElement root = uiDocument.rootVisualElement;

            //Unregister, comme ça on peut switch les callbackevent sans problème.
            root.Q<Button>("Menu_Exit_button").clicked -= menu_quit;
            root.Q<Button>("Menu_Option_button").clicked -= menu_option;
            // On desactive l'ui du menu
            root.Remove(menu);
        }

        void EnableOptionFromMenu()
        {
            DisableMenu();
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(option);

            root.Q<Button>("Option_Menu_button").clicked += option_menu;
        }

        void DisableOption()
        {
            VisualElement root = uiDocument.rootVisualElement;
            root.Remove(option);

            //FIXME null error >_<
            root.Q<Button>("Option_Menu_button").clicked -= option_menu;
            EnableMenu();
        }

        
    }
}
