using UnityEngine.InputSystem;

namespace Torthello
{
    public class TriangularSimpleBoard : Board
    {
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new TriangularSimpleBoardMeshGenerator(settings);
            MaterialHandler = new TriangularSimpleBoardMatHandler(settings);
            Graph = new TriangularSimpleBoardGraph(settings);
            inputSystem = new TriangularSimpleBoardInputManager(settings, transform, actionMap);
            pawnProccessor = new TriangularSimpleBoardPawnProcessor(transform, settings);
            base.OnEnable();
        }
    }
}