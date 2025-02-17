using UnityEngine;
using UnityEngine.InputSystem;

namespace Tortello
{
    public class ToreBoard : Board
    {
        public ToreBoardSettings settings;
        public InputActionAsset actionMap;
        new void OnEnable()
        {
            MeshGenerator = new ToreBoardMeshGenerator(settings);
            MaterialHandler = new ToreBoardMaterialHandler(settings);
            Graph = new ToreBoardGraph(settings);
            inputSystem = new ToreBoardInputSystem(settings, transform, actionMap);
            pawnProccessor = new ToreBoardPawnProccessor(transform, settings);
            base.OnEnable();
        }
    }
}