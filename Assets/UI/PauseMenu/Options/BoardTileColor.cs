using Unity.Properties;
using UnityEngine;



[CreateAssetMenu(fileName = "BoardTileColor", menuName = "Scriptable Objects/BoardTileColor")]
public class BoardTileColor : ScriptableObject
{
    [SerializeField]
    public float hue = 0f;

    [CreateProperty]
    public Color color
    {
        get => HueToColor(hue);
        set => hue = hue;
    }

    public static Color HueToColor(float hue)
    {
        return Color.HSVToRGB(hue, 0.9f, 0.9f);
    }
    
}

