using Unity.Properties;
using UnityEngine;
namespace Tortello
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Tortello/Settings")]
    public class Settings : ScriptableObject
    {
        [Range(0f,10f)] public float sideLength = 1f;
        public PawnModel PawnModel = PawnModel.Default;

        public Material Tilematerial;
        
        private float _hue = 0f;
        
        [CreateProperty]
        public float hue {
            get => _hue;
            set {
                _hue = value;
                Tilematerial.color = Color.HSVToRGB(_hue, 0.7f, 0.7f);
            }
        } 


        [CreateProperty]
        public Color color
        {
            get => Color.HSVToRGB(hue, 0.7f, 0.7f);
            
        }

        [Range(1, 20)]
        public int BoardWidth = 8;
        [Range(1, 20)]
        public int BoardHeight = 8;

        //default: true
        public Parameter<bool> m_fullscreen = new(true);

        [CreateProperty]
        public bool Fullscreen
        {
            get => m_fullscreen.GetValue();
            set => m_fullscreen.SetValue(value);
        }

        public bool isInGame = false;
    }
    public struct Parameter<T>
    {
        private T value;
        bool dirty;

        public Parameter(T v)
        {
            value = v;
            dirty = false;
        }

        public readonly T GetValue() => value;
        public void SetValue(T val)
        {
            value = val;
            dirty = true;
        }
        public readonly bool IsDirty() => dirty;
        public void Proccesed() => dirty = false;
    }
}

