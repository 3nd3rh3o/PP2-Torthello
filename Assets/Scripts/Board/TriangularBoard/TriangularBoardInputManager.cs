using UnityEngine;
using UnityEngine.InputSystem;
namespace Torthello
{
    public class TriangularBoardInputManager : FlatBoardInputSystem
    {
        public TriangularBoardInputManager(Settings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {
        }

        public override void Init()
        {
            actionMap.FindActionMap("InGame", false).Enable();
            previousWidth = settings.BoardWidth;
            previousHeight = settings.BoardHeight;
            previousSideLength = settings.sideLength;
            
            
        }

        

        public override int GetTileHoveredID()
        {
        }
}