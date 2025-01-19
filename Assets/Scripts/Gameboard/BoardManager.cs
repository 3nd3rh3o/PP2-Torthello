namespace Board
{
    using System;
    using Unity.Mathematics;
    using UnityEngine;

    public class Manager
    {
        private Settings settings;
        private GameManager parent;
        private CombineInstance[] combines;
        private Mesh EvenMesh;
        private Mesh OddMesh;
        private Mesh HoverMesh;

        private Material[] normal;
        private Material[] normalAndHover;

        public Manager(Settings settings, GameManager parent)
        {
            this.settings = settings;
            this.parent = parent;
            normal = new Material[]{settings.EvenMat, settings.OddMat};
            normalAndHover = new Material[]{settings.EvenMat, settings.OddMat, settings.HoverMat};
        }



        //Call this ONCE when the board params should be loaded
        public void Setup()
        {
            //Setup gameObject
            parent.GetComponent<MeshFilter>().mesh = new();
            parent.GetComponent<MeshRenderer>().sharedMaterials = normal;
            //Setup sub-meshes
            EvenMesh = new();
            OddMesh = new();
            HoverMesh = new();
            combines = new CombineInstance[3];
            combines[0].mesh = EvenMesh;
            combines[1].mesh = OddMesh;
            combines[2].mesh = HoverMesh;

        }

        //Call this to kill the board
        public void Discard()
        {
            //Destroy meshes and combine
            
            EvenMesh = null;
            OddMesh = null;
            HoverMesh = null;

            combines = null;


            parent.GetComponent<MeshFilter>().mesh.Clear();
            parent.GetComponent<MeshFilter>().mesh = null;
        }

        //Draw the tiles
        public void DrawBase()
        {
            //Draw parts and update rendered mesh
            EvenMesh = TorusMeshGenerator.GenMeshPair(EvenMesh, settings.radius, settings.sectionRadius, settings.numberOfSection * 2, settings.pointsPerSection * 2);
            OddMesh = TorusMeshGenerator.GenMeshImpair(OddMesh, settings.radius, settings.sectionRadius, settings.numberOfSection * 2, settings.pointsPerSection * 2);
            
            parent.GetComponent<MeshFilter>().sharedMesh.Clear();
            parent.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combines, false, false, false);
        }

        public void DrawEffect()
        {
            if (!settings.enableHoverEffect || parent.tileHovered.Equals(new(-1, -1)))
            {
                HoverMesh.Clear();
                parent.GetComponent<MeshRenderer>().sharedMaterials = normal;
            }
            else
            {
                HoverMesh = TorusMeshGenerator.GenMeshOfTileByIndex(HoverMesh, parent.tileHovered.x, parent.tileHovered.y, settings.radius, settings.sectionRadius, settings.numberOfSection * 2, settings.pointsPerSection * 2);
                parent.GetComponent<MeshRenderer>().sharedMaterials = normalAndHover;
            }
            
            parent.GetComponent<MeshFilter>().sharedMesh.Clear();
            parent.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combines, false, false, false);
        }
    }



    [Serializable]
    public class Settings
    {
        public float radius = 1f;
        public float sectionRadius = 0.5f;
        [Range(1, 50)]
        public int numberOfSection = 4;
        [Range(1, 50)]
        public int pointsPerSection = 4;


        public Material EvenMat;
        public Material OddMat;
        public Material HoverMat;

        [HideInInspector]
        public bool enableHoverEffect = false;
    }
}