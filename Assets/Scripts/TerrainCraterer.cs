using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCraterer : MonoBehaviour
{
    public static TerrainCraterer Instance {get ; private set;}
    [SerializeField] Terrain terrain;
    private TerrainData td;
    private Vector3 terPosition;
    private float[,] orig_heights;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this) {
            Debug.LogError("Only one terrain craterer please");
            Destroy(gameObject);
        }
        Instance = this;
        if (terrain == null) terrain = Terrain.activeTerrain;
        td = terrain.terrainData; 
        terPosition =  terrain.transform.position;
        orig_heights = td.GetHeights(0, 0, td.heightmapResolution ,td.heightmapResolution);
    }
    private Vector3 ConvertWorldCor2TerrCor(Vector3 wordCor)
    {
        Vector3 vecRet = new Vector3();
        vecRet.x = ((wordCor.x - terPosition.x) / td.size.x);
        vecRet.z = ((wordCor.z - terPosition.z) / td.size.z);
        return vecRet;
    }
    private (int, int) TerrainTextureFromLocalCoordinates(Vector3 localCoordinates) {
        float relativeTerCoordX = td.heightmapResolution * localCoordinates.x;
        float relativeTerCoordZ = td.heightmapResolution * localCoordinates.z;
        return (Mathf.FloorToInt(relativeTerCoordX), Mathf.FloorToInt(relativeTerCoordZ));
    }
    public void MakeCraterAtWorld(Vector3 world_position) {
        Vector3 local = ConvertWorldCor2TerrCor(world_position);
        (int hitPointTerX, int hitPointTerZ) = TerrainTextureFromLocalCoordinates(local);

        if (hitPointTerX < 0 || hitPointTerX >= td.heightmapResolution || hitPointTerZ < 0 || hitPointTerZ >= td.heightmapResolution) {
            Debug.LogError("Hit at a point off the terrain : " + hitPointTerX + " " + hitPointTerZ);
            Debug.LogError("Size is " + td.size);
            Debug.LogError("resolution is " + td.heightmapResolution);
            return;
        }
        //float[,] heights = td.GetHeights(0, 0, td.heightmapResolution ,td.heightmapResolution);
        float[,] heights = td.GetHeights(hitPointTerX, hitPointTerZ, 1, 1);
         
        //heights[ hitPointTerZ, hitPointTerX ] = heights[ hitPointTerZ, hitPointTerX ] * 0.95f;
        heights[0,0] = heights[0,0] * 0.95f;
        //td.SetHeights( 0, 0, heights );
        td.SetHeights( hitPointTerX, hitPointTerZ, heights );
    }
    private void OnApplicationQuit() {
        td.SetHeights( 0, 0, orig_heights);
    }
}
