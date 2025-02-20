using UnityEngine.InputSystem;

namespace Tortello
{
    public class FlatBoard : Board
    {
        public FlatBoardSettings settings;
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new FlatBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            Graph = new FlatBoardGraph(settings);
            inputSystem = new FlatBoardInputSystem(settings, transform, actionMap);
            pawnProccessor = new FlatBoardPawnProccessor(transform, settings);
            base.OnEnable();
        }
    }
}