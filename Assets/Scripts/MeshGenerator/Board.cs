using UnityEngine;

namespace Tortello
{
    /// <summary>
    /// Doit contenir un générateur de forme, un générateur de materiau, un gestionnaire d'input et un graphe.
    /// </summary>
    
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Board : MonoBehaviour
    {
        /// <summary>
        /// Ce qui génère la forme du plateau.
        /// </summary>
        public MeshGenerator MeshGenerator;


        /// <summary>
        /// Ce qui défini le renderer du plateau.
        /// </summary>
        public MaterialHandler MaterialHandler;


        /// <summary>
        /// Appelé lorsque le GO ou le script est activé.
        /// </summary>
        void OnEnable()
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.UpdateMesh(mF);
            MaterialHandler.UpdateRenderer(mR);
        }

        /// <summary>
        /// Appelé à chaque frame.
        /// </summary>
        void Update()
        {

        }
        
        /// <summary>
        /// Appelé toute les Time.fixedDeltaTime(intervale constant).
        /// </summary>
        void FixedUpdate()
        {

        }

        /// <summary>
        /// Appelé lorsque le GO ou le script est désactivé.
        /// </summary>
        void OnDisable()
        {
            
        }
    }
}