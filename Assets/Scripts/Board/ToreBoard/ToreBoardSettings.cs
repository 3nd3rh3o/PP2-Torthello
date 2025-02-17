using UnityEngine;
namespace Tortello
{
    [CreateAssetMenu(fileName = "ToreBoardSettings", menuName = "Tortello/ToreBoardSettings")]
    public class ToreBoardSettings : ScriptableObject
    {
        [Range(4, 20)] public int BoardWidth = 8;

        [Range(4, 20)] public int BoardHeight = 8;

        [Range(0f,10f)] public float sideLength = 1f;
        public PawnModel PawnModel = PawnModel.Default;

        public Material Tilematerial;
    }

}

