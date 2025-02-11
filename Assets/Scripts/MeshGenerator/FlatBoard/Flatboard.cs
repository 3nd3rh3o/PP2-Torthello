namespace Tortello
{
    public class FlatBoard : Board
    {
        public FlatBoardSettings settings;
        new void OnEnable()
        {
            MeshGenerator = new FlatBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            Graph = new FlatBoardGraph(settings);
            base.OnEnable();
        }
    }
}