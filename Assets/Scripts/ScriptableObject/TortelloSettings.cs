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


        [HideInInspector]
        public bool IA = true;
        //used by UI.
        public float Score = 0f;

        //UI asked for game start.
        [HideInInspector]
        public bool startCMD = false;

        //Game state for UI.
        public string turn = "";



        private float _hue = 0.45f;

        [CreateProperty]
        public float hue
        {
            get => _hue;
            set
            {
                _hue = value;
                Tilematerial.color = Color.HSVToRGB(_hue, 0.7f, 0.7f);
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
            get => type.GetValue() switch { Torthello.BoardType.TwoD => 0, Torthello.BoardType.Torus => 1, _ => throw new NotImplementedException() };
            set => type.SetValue(value switch { 0 => Torthello.BoardType.TwoD, 1 => Torthello.BoardType.Torus, _ => throw new NotImplementedException() });
        }

        [Range(4, 20)]
        public int BoardWidth = 8;
        [Range(4, 20)]
        public int BoardHeight = 8;

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
        Torus
    }
}

