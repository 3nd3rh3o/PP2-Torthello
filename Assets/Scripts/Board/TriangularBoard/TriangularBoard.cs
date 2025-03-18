using UnityEngine.InputSystem;

namespace Torthello
{
    public class TriangularBoard : Board
    {
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new TriangularBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            Graph = new TriangularBoardGraph(settings);
            inputSystem = new TriangularBoardInputManager(settings, transform, actionMap);
            pawnProccessor = new TriangularBoardPawnProcessor(transform, settings);
            base.OnEnable();
        }
    }
}