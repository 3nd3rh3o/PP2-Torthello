using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Torthello
{
    public class TorBoard : Board
    {
        public InputActionAsset actionMap;
        private IPlayerAI aiPlayerNoir;
        private IPlayerAI aiPlayerBlanc;

        new void OnEnable()
        {
            MeshGenerator = new TorBoardMeshGenerator(settings);
            MaterialHandler = new TorBoardMaterialHandler(settings);
            Graph = new TorBoardGraph(settings);
            inputSystem = new TorBoardInputSystem(settings, transform, actionMap, (TorBoardMeshGenerator)MeshGenerator);
            pawnProccessor = new TorBoardPawnProcessor(transform, settings, (TorBoardMeshGenerator)MeshGenerator);
            aiPlayerNoir = new PlayerMiniMax(Graph, Couleur.Noir);
            aiPlayerBlanc = new PlayerMiniMax(Graph, Couleur.Blanc);
            base.OnEnable();
        }
    }
}
