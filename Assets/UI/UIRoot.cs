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
        public VisualTreeAsset NewGameAsset;
        private TemplateContainer newGame;
        private UIDocument uiDocument;
        // action a bind aux bouttons
        private readonly Action menu_quit = () => Application.Quit();
        private Action menu_option;
        private Action option_back;
        private Action menu_newGame;
        private Action newGame_back;


        private int state = 0;


        public Settings settings;
        void OnEnable()
        {
            menu_option = () =>
            {
                Disable(state);
                EnableOptionFromMenu();
            };

            option_back = () =>
            {
                Disable(state);
                EnableMenu();
            };

            menu_newGame = () =>
            {
                Disable(state);
                EnableNewGame();
            };

            newGame_back = () =>
            {
                Disable(state);
                EnableMenu();
            };

            uiDocument = GetComponent<UIDocument>();
            menu = MenuAsset.Instantiate();
            option = MenuOptionAsset.Instantiate();
            newGame = NewGameAsset.Instantiate();


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
            Disable(state);
        }

        void Disable(int state)
        {
            // we expect an exception when exiting playmode in editor.
            // UI destroyed before manual cleaning. (not a problem)
            try
            {
                switch (state)
                {
                    case 0:
                        DisableMenu();
                        break;
                    case 1:
                        DisableOption();
                        break;
                    case 2:
                        DisableNewGame();
                        break;
                }

            } catch (ArgumentNullException)
            {
                
            }
        }

        void EnableMenu()
        {
            state = 0;
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(menu);

            // register de l'action au boutton Menu_Exit_Button.
            root.Q<Button>("Menu_Exit_button").clicked += menu_quit;
            root.Q<Button>("Menu_Option_button").clicked += menu_option;
            root.Q<Button>("Menu_NewGame_button").clicked += menu_newGame;

        }

        void DisableMenu()
        {
            VisualElement root = uiDocument.rootVisualElement;

            //Unregister, comme ça on peut switch les callbackevent sans problème.
            root.Q<Button>("Menu_Exit_button").clicked -= menu_quit;
            root.Q<Button>("Menu_Option_button").clicked -= menu_option;
            root.Q<Button>("Menu_NewGame_button").clicked -= menu_newGame;
            // On desactive l'ui du menu
            root.Remove(menu);
        }

        void EnableOptionFromMenu()
        {
            state = 1;
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(option);

            root.Q<Button>("Option_Menu_button").clicked += option_back;
        }

        void DisableOption()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Q<Button>("Option_Menu_button").clicked -= option_back;

            root.Remove(option);
        }


        void EnableNewGame()
        {
            state = 2;

            VisualElement root = uiDocument.rootVisualElement;

            root.Add(newGame);

            root.Q<Button>("NewGame_Menu_button").clicked += newGame_back;
        }

        void DisableNewGame()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Q<Button>("NewGame_Menu_button").clicked -= newGame_back;

            root.Remove(newGame);
        }


    }
}
