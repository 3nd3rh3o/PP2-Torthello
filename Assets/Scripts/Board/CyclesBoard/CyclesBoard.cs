using UnityEngine.InputSystem;

namespace Torthello
{
    public class CyclesBoard : Board
    {
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new FlatBoardMeshGenerator(settings);
            MaterialHandler = new CycleMaterialHandler(settings);
            Graph = new CyclesBoardGraph(settings);
            inputSystem = new FlatBoardInputSystem(settings, transform, actionMap);
            pawnProccessor = new FlatBoardPawnProccessor(transform, settings);

            base.OnEnable();
        }
    }
}