namespace Tortello
{
    public class FlatBoard : Board
    {
        public FlatBoardSettings settings;
        new void OnEnable()
        {
            MeshGenerator = new FlatBoardMeshGenerator(settings);
            MaterialHandler = new FlatBoardMaterialHandler(settings);
            inputSystem = new FlatBoardInputSystem(settings, transform);
            base.OnEnable();
        }
    }
}