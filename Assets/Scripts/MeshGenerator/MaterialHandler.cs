using UnityEngine;

namespace Tortello
{
    public interface MaterialHandler
    {
        public void UpdateRenderer(MeshRenderer renderer);

        public void Destroy(MeshRenderer renderer);
    }
}