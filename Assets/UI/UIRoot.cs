using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Tortello
{
    public class UIRoot : MonoBehaviour
    {
        public InputActionAsset actions;
        public VisualTreeAsset MenuAsset;
        private TemplateContainer menu;
        public VisualTreeAsset MenuOptionAsset;
        private TemplateContainer option;
        public VisualTreeAsset NewGameAsset;
        private TemplateContainer newGame;
        public VisualTreeAsset InGameOverlayAsset;
        private TemplateContainer inGameOverlay;

        private UIDocument uiDocument;
        // action a bind aux bouttons
        private readonly Action menu_quit = () => Application.Quit();
        private Action menu_option;
        private Action back_to_menu;
        private Action menu_newGame;
        private Action newGame_twoPlayer;


        private int state = 0;


        public Settings settings;
        void OnEnable()
        {
            menu_option = () =>
            {
                Disable(state);
                EnableOptionFromMenu();
            };

            back_to_menu = () =>
            {
                Disable(state);
                EnableMenu();
            };

            menu_newGame = () =>
            {
                Disable(state);
                EnableNewGame();
            };

            newGame_twoPlayer = () =>
            {
                Disable(state);
                EnableInGameOverlay();
                settings.startCMD = true;
                
            };
            
            uiDocument = GetComponent<UIDocument>();
            menu = MenuAsset.Instantiate();
            option = MenuOptionAsset.Instantiate();
            newGame = NewGameAsset.Instantiate();
            inGameOverlay = InGameOverlayAsset.Instantiate();
            actions.FindActionMap("InGame", false).Enable();
            

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



            if (actions.FindActionMap("InGame", false).FindAction("Esc").WasReleasedThisFrame())
            {
                //in main menu
                if (state == 0)
                {
                    Application.Quit();
                }
                //in option(Menu) or new game
                else if (state == 1 || state == 2)
                {
                    back_to_menu();
                }
                //in game
                else if (state == 3)
                {

                }
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
                    case 3:
                        DisableInGameOverlay();
                        break;
                }

            } catch
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

            root.Q<Button>("Option_Menu_button").clicked += back_to_menu;
        }

        void DisableOption()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Q<Button>("Option_Menu_button").clicked -= back_to_menu;

            root.Remove(option);
        }


        void EnableNewGame()
        {
            state = 2;

            VisualElement root = uiDocument.rootVisualElement;

            root.Add(newGame);

            root.Q<Button>("NewGame_Menu_button").clicked += back_to_menu;
            root.Q<Button>("NewGame_TwoPlayer_button").clicked += newGame_twoPlayer;
        }

        void DisableNewGame()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Q<Button>("NewGame_Menu_button").clicked -= back_to_menu;
            root.Q<Button>("NewGame_TwoPlayer_button").clicked -= newGame_twoPlayer;

            root.Remove(newGame);
        }

        void EnableInGameOverlay()
        {
            state = 3;

            VisualElement root = uiDocument.rootVisualElement;

            root.Add(inGameOverlay);
        }

        void DisableInGameOverlay()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Remove(inGameOverlay);
        }

    }
}
