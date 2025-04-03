using System;
using Unity.Properties;
using UnityEngine;
namespace Torthello
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Tortello/Settings")]
    public class Settings : ScriptableObject
    {
        [Range(0f, 10f)] public float sideLength = 1.5f;
        public PawnModel PawnModel = PawnModel.Default;

        public Material Tilematerial;

        public Material HexTilematerial;

        public int offset_Minimap_width = 0;
        public int offset_Minimap_height = 0;

        public int hoveredTile = 0;

        [Tooltip("Used to generate minimap on torus board")] public ComputeShader minimapCS;
        [CreateProperty]public RenderTexture minimapRT;


        [HideInInspector]
        public bool IA = true;
        //used by UI.
        public float Score = 0f;

        //UI asked for game start.
        [HideInInspector]
        public bool startCMD = false;

        //Game state for UI.
        public string turn = "";

        public float rotationOffset = 0f;
        [HideInInspector] public bool rotAnimU = false;
        [HideInInspector] public float rotAnimT = 0f;
        [HideInInspector] public bool rotAnimD = false;

        private float _hue = 0.45f;

        [CreateProperty]
        public float hue
        {
            get => _hue;
            set
            {
                _hue = value;
                Tilematerial.color = Color.HSVToRGB(_hue, 0.7f, 0.7f);
                HexTilematerial.color = Color.HSVToRGB(_hue, 0.7f, 0.7f);
            }
        }

        [CreateProperty]
        public Color color
        {
            get => Color.HSVToRGB(hue, 0.7f, 0.7f);

        }

        [HideInInspector]
        public Parameter<BoardType> type = new(Torthello.BoardType.TwoD);

        [CreateProperty]
        public int BoardType
        {
            get => type.GetValue() switch { Torthello.BoardType.TwoD => 0, Torthello.BoardType.Torus => 1, Torthello.BoardType.TriangularBoard => 2, Torthello.BoardType.TriangularSimpleBoard => 3, _ => throw new NotImplementedException() };
            set => type.SetValue(value switch { 0 => Torthello.BoardType.TwoD, 1 => Torthello.BoardType.Torus, 2 => Torthello.BoardType.TriangularBoard, 3 => Torthello.BoardType.TriangularSimpleBoard, _ => throw new NotImplementedException() });
        }

        [Range(4, 20)]
        public int BoardWidth = 8;
        [Range(4, 20)]
        public int BoardHeight = 8;

        public Parameter<bool> m_MinimapTorus = new(false);

        [CreateProperty] public bool MinimapTorus
        {
            get => m_MinimapTorus.GetValue();
            set => m_MinimapTorus.SetValue(value);
        }

        //default: true
        
        public Parameter<bool> m_fullscreen = new(true);

        [CreateProperty]
        public bool Fullscreen
        {
            get => m_fullscreen.GetValue();
            set => m_fullscreen.SetValue(value);
        }
        [HideInInspector]
        public bool isInGame = false;
        public float CamSentivity = 0.5f;
        
        public float pitch = 0f;
        public float yaw = 120f;
        public float zoom = 15f;
        [HideInInspector]
        public bool rebuildBoardCMD;
        public int Difficulty = 2;

        //NOTE : Need checks

        public PlayerType PlayerNoir = PlayerType.Human;

        public PlayerType PlayerBlanc = PlayerType.MiniMax;


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

    public enum PlayerType
    {
        Human,
        MiniMax
    }

    public enum BoardType
    {
        TwoD,
        TriangularBoard,
        Torus,
        TriangularSimpleBoard
    }
}

