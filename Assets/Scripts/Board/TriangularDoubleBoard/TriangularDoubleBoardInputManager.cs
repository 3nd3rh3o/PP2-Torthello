using UnityEngine;
using UnityEngine.InputSystem;
namespace Torthello
{
    public class TriangularDoubleBoardInputManager : InputManager
    {
        public TriangularDoubleBoardInputManager(Settings settings, Transform transform, InputActionAsset actionMap) : base(settings, transform, actionMap)
        {
        }
    }
}