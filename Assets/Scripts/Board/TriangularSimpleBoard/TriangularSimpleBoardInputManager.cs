using UnityEngine;
using UnityEngine.InputSystem;
namespace Torthello
{
    public class TriangularSimpleBoardInputManager : TriangularBoardInputManager
    {
        public TriangularSimpleBoardInputManager(Settings settings, Transform boardTransform, InputActionAsset actionMap) : base(settings, boardTransform, actionMap)
        {
        }
    }
}