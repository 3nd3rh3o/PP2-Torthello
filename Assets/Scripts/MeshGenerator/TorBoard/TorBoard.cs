using UnityEngine;
using UnityEngine.InputSystem;

namespace Torthello
{
    public class TorBoard : Board
    {
        public TorBoardSettings settings;
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new TorBoardMeshGenerator(settings);
            MaterialHandler = new TorBoardMaterialHandler(settings);
            Graph = new TorBoardGraph(settings);
            inputSystem = new TorBoardInputSystem(settings, transform, actionMap);
            pawnProccessor = new TorBoardPawnProcessor(transform, settings);
            base.OnEnable();
        }
    }
}
