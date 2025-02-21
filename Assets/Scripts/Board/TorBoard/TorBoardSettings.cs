using UnityEngine;

namespace Torthello{

    [CreateAssetMenu(fileName = "TorBoardSettings", menuName = "Tortello/TorBoardSettings")]
    public class TorBoardSettings : ScriptableObject
    {
        [Range(4, 20)] public int BoardSize = 8;
        [Range(0f, 10f)] public float SideLength = 1f; // pas  utilisé pour le tor, il faut modifier le meshGenerator si on veut s'en servir pour modifier la taille du tor
        public Material TileMaterial;
        public PawnModel PawnModel = PawnModel.Default;

        public PlayerType PlayerNoir = PlayerType.Human;

        public PlayerType PlayerBlanc = PlayerType.MiniMax;

    }

    public enum PlayerType
    {
        Human,
        MiniMax
    }

}