using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Torthello
{
    public class CyclesInputSystem : FlatBoardInputSystem
    {
        private float2[] nodes;
        public CyclesInputSystem(Settings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {

        }

        public override int GetTileHoveredID()
        {
            if (!Camera.main || !Application.isFocused) return -1;
            Vector2 mousePos = new(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            for (int i = 0; i < settings.BoardHeight; i++)
            {
                if ((mousePos - new Vector2(nodes[i].x, nodes[i].y)).sqrMagnitude <= 0.01f)
                {
                    previousHoveredTileID = i;
                    return i;
                }
            }
            previousHoveredTileID = -1;
            return -1;
        }

        public override void Init()
        {
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            int lCycle = settings.BoardHeight;
            nodes = new float2[lCycle];
            const float RADIUS = 0.3f;
            float THETA = 2f * Mathf.PI / lCycle;
            for (int i = 0; i < lCycle; i++)
            {
                nodes[i] = new float2(0.5f + RADIUS * Mathf.Cos(i * THETA), 0.5f + RADIUS * Mathf.Sin(i * THETA));
            }
            actionMap.FindActionMap("InGame", false).Enable();
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            previousSideLength = settings.sideLength;
            previousHoveredTileID = -1;
            Camera.main.transform.position = Quaternion.Euler(0f, settings.yaw, 0f) * Quaternion.Euler(0f, 0f, -settings.pitch) * (boardTransform.position - new Vector3(settings.zoom, 0f, 0f));
            Camera.main.transform.LookAt(boardTransform.position);
        }
    }
}