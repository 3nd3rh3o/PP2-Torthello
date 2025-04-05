using UnityEngine.InputSystem;

namespace Torthello
{
    public class CyclesBoard : Board
    {
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new CyclesMeshGenerator(settings);
            MaterialHandler = new CycleMaterialHandler(settings);
            Graph = new CyclesBoardGraph(settings);
            inputSystem = new FlatBoardInputSystem(settings, transform, actionMap);
            pawnProccessor = new CyclesPawnProcessor(transform, settings);

            base.OnEnable();
        }
    }
}