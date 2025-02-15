using UnityEngine;

namespace Tortello{

    [CreateAssetMenu(fileName = "TorBoardSettings", menuName = "Tortello/TorBoardSettings")]
    public class TorBoardSettings : ScriptableObject
    {
        [Range(4, 20)] public int BoardSize = 8;
        [Range(0f, 10f)] public float SideLength = 1f;
        public Material TileMaterial;
        public PawnModel PawnModel = PawnModel.Default;
    }
}