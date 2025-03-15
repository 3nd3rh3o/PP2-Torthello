using UnityEngine.InputSystem;

namespace Torthello
{
        public class TriangularDoubleBoard : Board
    {
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new ToreDoubleBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            Graph = new ToreDoubleBoardGraph(settings);
            inputSystem = new ToreDoubleBoardInputManager(settings, transform, actionMap);
            pawnProccessor = new ToreDoubleBoardPawnProcessor(transform, settings);
            base.OnEnable();
        }
    }
}