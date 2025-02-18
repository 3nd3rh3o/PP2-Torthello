using UnityEngine;
using UnityEngine.UIElements;

namespace Tortello
{
    public class UIRoot : MonoBehaviour
    {
        private TemplateContainer root;

        public bool quit_b = false;

        public Settings settings;
        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().visualTreeAsset.CloneTree();
            root.Query<Button>().ForEach((button) => button.clickable = new(() => Debug.Log("AAAHHH!!!")));

        }
        void Update()
        {
            
            if (settings.m_fullscreen.IsDirty())
            {
                if (settings.m_fullscreen.GetValue()) Screen.fullScreen = true;
                else Screen.fullScreen = false;
                settings.m_fullscreen.Proccesed();
            }

            if (quit_b) {
                Debug.Log("clicked");
                
            }
        }

    }
}
