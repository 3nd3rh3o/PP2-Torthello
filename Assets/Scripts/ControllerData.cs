using System;
using UnityEngine;

namespace Torthello
{
    [Serializable]
    public class GOHolder
    {
        public GameObject MainMenu;
        public GameObject PauseUI;

        public GameObject Torus;
    }

    [Serializable]
    public class Settings
    {

        [Tooltip("Graphic settings of the game")]
        public GraphicSettings GraphicSettings;
        
    }
    [Serializable]
    public class GraphicSettings
    {
        [SerializeField]
        public ScreenMode displayMode;

        [Serializable]
        public enum ScreenMode
        {
            Windowed,
            Fullscreen
        }
    }
}