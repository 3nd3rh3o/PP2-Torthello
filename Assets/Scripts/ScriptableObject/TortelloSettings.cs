using Unity.Properties;
using UnityEngine;
namespace Tortello
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Tortello/Settings")]
    public class Settings : ScriptableObject
    {
        [SerializeField]
        public float hue = 0f;

        [CreateProperty]
        public Color color
        {
            get => Color.HSVToRGB(hue, 0.7f, 0.7f);
            set
            {
                Color.RGBToHSV(value, out hue, out _, out _);
            }
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

