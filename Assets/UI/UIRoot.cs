using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Torthello
{
    public class UIRoot : MonoBehaviour
    {
        public GameObject gameBoard;
        public InputActionAsset actions;
        public VisualTreeAsset MenuAsset;
        private TemplateContainer menu;
        public VisualTreeAsset MenuOptionAsset;
        private TemplateContainer menuOption;
        public VisualTreeAsset NewGameAsset;
        private TemplateContainer newGame;
        public VisualTreeAsset InGameOverlayAsset;
        private TemplateContainer inGameOverlay;
        public VisualTreeAsset PauseUIAsset;
        private TemplateContainer pauseUI;

        public VisualTreeAsset PauseOptionUIAsset;
        private TemplateContainer pauseOptionUI;

        private UIDocument uiDocument;
        // action a bind aux bouttons
        private readonly Action menu_quit = () => Application.Quit();
        private Action menu_option;
        private Action back_to_menu;
        private Action menu_newGame;
        private Action newGame_twoPlayer;
        private Action newGame_BOT;
        private Action pause_Resume;
        private Action pause_Menu;
        private Action pause_Option;
        private Action pause_option_back;

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
                settings.IA = false;
                settings.startCMD = true;

            };
            newGame_BOT = () =>
            {
                Disable(state);
                EnableInGameOverlay();
                settings.IA = true;
                settings.startCMD = true;
            };
            pause_Resume = () =>
            {
                Disable(state);
                settings.isInGame = true;
                EnableInGameOverlay();
            };
            pause_Menu = () =>
            {
                Disable(state);
                settings.isInGame = false;
                settings.rebuildBoardCMD = true;
                EnableMenu();
            };
            pause_Option = () =>
            {
                Disable(state);
                EnablePauseOption();
            };
            pause_option_back = () =>
            {
                Disable(state);
                EnablePauseMenu();
            };

            uiDocument = GetComponent<UIDocument>();
            menu = MenuAsset.Instantiate();
            menuOption = MenuOptionAsset.Instantiate();
            newGame = NewGameAsset.Instantiate();
            inGameOverlay = InGameOverlayAsset.Instantiate();
            pauseUI = PauseUIAsset.Instantiate();
            pauseOptionUI = PauseOptionUIAsset.Instantiate();
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
            if (settings.m_fullscreen.GetValue()) Screen.fullScreen = true;
            else Screen.fullScreen = false;
            settings.m_fullscreen.Proccesed();
            // creation gameboard
            gameBoard.SetActive(false);
            FlatBoard f = gameBoard.AddComponent<FlatBoard>();
            f.settings = settings;
            f.actionMap = actions;
            gameBoard.SetActive(true);

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


            if (settings.type.IsDirty())
            {
                gameBoard.SetActive(false);
                if (settings.type.GetValue() == BoardType.TwoD)
                {
                    RemoveBoard();
                    FlatBoard f = gameBoard.AddComponent<FlatBoard>();
                    f.settings = settings;
                    f.actionMap = actions;
                }
                else if (settings.type.GetValue() == BoardType.Torus)
                {
                    RemoveBoard();
                    ToreBoard t = gameBoard.AddComponent<ToreBoard>();
                    t.settings = settings;
                    t.actionMap = actions;
                }
                else if(settings.type.GetValue() == BoardType.TriangularBoard)
                {
                    RemoveBoard();
                    TriangularBoard t = gameBoard.AddComponent<TriangularBoard>();
                    t.settings = settings;
                    t.actionMap = actions;
                }
                else{
                    RemoveBoard();
                    TriangularSimpleBoard t = gameBoard.AddComponent<TriangularSimpleBoard>();
                    t.settings = settings;
                    t.actionMap = actions;
                }
                settings.type.Proccesed();
                gameBoard.SetActive(true);

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
                    Disable(state);
                    settings.isInGame = false;
                    EnablePauseMenu();
                }
                else if (state == 4)
                {
                    pause_Resume();
                }
                else if (state == 5)
                {
                    pause_option_back();
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
                    case 4:
                        DisablePauseMenu();
                        break;
                    case 5:
                        DisablePauseOption();
                        break;
                }

            }
            catch
            {

            }
        }

        void EnableMenu()
        {
            state = 0;
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(menu);
            settings.pitch = 120f;
            settings.yaw = 0f;

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
            root.Add(menuOption);

            root.Q<Button>("Option_Menu_button").clicked += back_to_menu;
        }

        void DisableOption()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Q<Button>("Option_Menu_button").clicked -= back_to_menu;

            root.Remove(menuOption);
        }


        void EnableNewGame()
        {
            state = 2;

            VisualElement root = uiDocument.rootVisualElement;

            root.Add(newGame);

            root.Q<Button>("NewGame_Menu_button").clicked += back_to_menu;
            root.Q<Button>("NewGame_TwoPlayer_button").clicked += newGame_twoPlayer;
            root.Q<Button>("NewGame_BOT_button").clicked += newGame_BOT;
        }

        void DisableNewGame()
        {
            VisualElement root = uiDocument.rootVisualElement;

            root.Q<Button>("NewGame_Menu_button").clicked -= back_to_menu;
            root.Q<Button>("NewGame_TwoPlayer_button").clicked -= newGame_twoPlayer;
            root.Q<Button>("NewGame_BOT_button").clicked -= newGame_BOT;

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

        void EnablePauseMenu()
        {
            state = 4;
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(pauseUI);
            root.Q<Button>("Pause_Resume_button").clicked += pause_Resume;
            root.Q<Button>("Pause_Option_button").clicked += pause_Option;
            root.Q<Button>("Pause_Menu_button").clicked += pause_Menu;
        }

        void DisablePauseMenu()
        {
            VisualElement root = uiDocument.rootVisualElement;
            root.Q<Button>("Pause_Resume_button").clicked -= pause_Resume;
            root.Q<Button>("Pause_Option_button").clicked -= pause_Option;
            root.Q<Button>("Pause_Menu_button").clicked -= pause_Menu;
            root.Remove(pauseUI);
        }
        void EnablePauseOption()
        {
            state = 5;
            VisualElement root = uiDocument.rootVisualElement;
            root.Add(pauseOptionUI);
            root.Q<Button>("Option_Pause_button").clicked += pause_option_back;
        }
        void DisablePauseOption()
        {
            VisualElement root = uiDocument.rootVisualElement;
            root.Q<Button>("Option_Pause_button").clicked -= pause_option_back;
            root.Remove(pauseOptionUI);
        }


        void RemoveBoard()
        {
            try
            {
#if UNITY_EDITOR
                DestroyImmediate(gameBoard.GetComponent<FlatBoard>());
#else
                Destroy(gameBoard.GetComponent<FlatBoard>());
#endif
            }
            catch { }
            try
            {
#if UNITY_EDITOR
                DestroyImmediate(gameBoard.GetComponent<ToreBoard>());
#else
                Destroy(gameBoard.GetComponent<ToreBoard>());
#endif
            }
            catch { }
            try
            {
#if UNITY_EDITOR
                DestroyImmediate(gameBoard.GetComponent<TriangularBoard>());
#else
                Destroy(gameBoard.GetComponent<TriangularBoard>());
#endif
            }
            catch { }
        }
    }
}
