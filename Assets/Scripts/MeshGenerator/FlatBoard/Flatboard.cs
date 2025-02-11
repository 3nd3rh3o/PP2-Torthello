namespace Tortello
{
    public class FlatBoard : Board
    {
        public FlatBoardSettings settings;
        new void OnEnable()
        {
            MeshGenerator = new FlatBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            base.OnEnable();
        }
    }
}