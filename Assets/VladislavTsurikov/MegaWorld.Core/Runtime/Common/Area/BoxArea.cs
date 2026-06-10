using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Area
{
    public class BoxArea : Area
    {
        public readonly RayHit RayHit;
        public readonly Terrain TerrainUnder;

        public Vector3 Normal => RayHit?.Normal ?? Vector3.up;
        public Bounds TerrainBounds { get; set; }

        public Vector3 Tangent
        {
            get
            {
                Vector3 referenceAxis = Mathf.Abs(Vector3.Dot(Normal, Vector3.up)) < 0.99f
                    ? Vector3.up
                    : Vector3.right;
                Vector3 tangent = Vector3.Cross(referenceAxis, Normal).normalized;

                return tangent;
            }
        }

        public Vector3 Bitangent => Vector3.Cross(Normal, Tangent).normalized;

        public float BoxSize => LocalSize.x;

        public BoxArea(RayHit rayHit, float size) : base(rayHit.Point, size)
        {
            RayHit = rayHit;
            TerrainUnder = UnityTerrainUtility.GetTerrain(rayHit.Point);
            TerrainBounds = GetTerrainBounds(TerrainUnder);
        }

        public BoxArea(RayHit rayHit, Vector3 size) : base(rayHit.Point, size)
        {
            RayHit = rayHit;
            TerrainUnder = UnityTerrainUtility.GetTerrain(rayHit.Point);
            TerrainBounds = GetTerrainBounds(TerrainUnder);
        }

        public BoxArea(RayHit rayHit, Vector3 size, Terrain terrainUnder) : base(rayHit.Point, size)
        {
            RayHit = rayHit;
            TerrainUnder = terrainUnder != null ? terrainUnder : UnityTerrainUtility.GetTerrain(rayHit.Point);
            TerrainBounds = GetTerrainBounds(TerrainUnder);
        }

        private static Bounds GetTerrainBounds(Terrain terrain)
        {
            if (terrain == null || terrain.terrainData == null)
            {
                return default;
            }

            return new Bounds(terrain.terrainData.bounds.center + terrain.transform.position,
                terrain.terrainData.bounds.size);
        }
    }
}
