#if UNITY_EDITOR
#if UNDO_RENDERER_STACK
using System.Collections.Generic;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;

namespace VladislavTsurikov.Undo.Editor.TerrainObjectRenderer
{
    public class CreatedTerrainObject : UndoRecord
    {
        private readonly List<TerrainObjectInstance> _instanceList = new();

        public CreatedTerrainObject(TerrainObjectInstance terrainObject) => _instanceList.Add(terrainObject);

        public override void Merge(UndoRecord record)
        {
            if (record is not CreatedTerrainObject undo)
            {
                return;
            }

            _instanceList.AddRange(undo._instanceList);
        }

        public override void Undo()
        {
            foreach (TerrainObjectInstance terrainObject in _instanceList)
            {
                terrainObject.Destroy();
            }
        }
    }
}
#endif
#endif
