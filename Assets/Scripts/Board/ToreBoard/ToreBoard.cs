using UnityEngine.InputSystem;

namespace Tortello
{
    public class ToreBoard : Board
    {
        public FlatBoardSettings settings;
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new ToreBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            Graph = new ToreBoardGraph(settings);
            inputSystem = new ToreBoardInputManager(settings, transform, actionMap);
            pawnProccessor = new ToreBoardPawnProcessor(transform, settings);
            base.OnEnable();
        }
    }
}