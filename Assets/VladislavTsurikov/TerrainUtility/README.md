# TerrainUtility

`VladislavTsurikov.TerrainUtility` contains editor-side reusable APIs for:

- creating terrain asset folders
- creating or updating terrain grids in scene
- applying heightmaps to `TerrainData`
- wiring terrain neighbors
- storing generated terrain assets in a flat folder layout similar to `WorldCreatorBridge`

The module is intentionally generic and can be reused by importers such as `GaeaBridge`.
