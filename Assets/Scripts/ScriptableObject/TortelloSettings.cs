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
        
    }
}

