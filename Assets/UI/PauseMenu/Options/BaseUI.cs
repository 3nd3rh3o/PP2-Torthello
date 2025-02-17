using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[ExecuteAlways]
public class BaseUI : UIBehaviour
{
    public BoardTileColor settings;


    public new void OnEnable()
    {
        base.OnEnable();
        ConverterGroups.RegisterGlobalConverter((ref float hue) => Color.HSVToRGB(hue, 0.9f, 0.9f));
    }
}
