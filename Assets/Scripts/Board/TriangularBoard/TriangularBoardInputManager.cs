using UnityEngine;
using UnityEngine.InputSystem;
namespace Torthello
{
    public class TriangularBoardInputManager : FlatBoardInputSystem
    {
        public TriangularBoardInputManager(Settings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {
        }
    }
}