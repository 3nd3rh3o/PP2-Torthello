namespace Tortello
{
    public class FlatBoard : Board
    {
        new void OnEnable()
        {
            MeshGenerator = new FlatBoardMeshGenerator();
            MaterialHandler = new FlatBoardMaterialHandler();
            base.OnEnable();
        }
    }
}