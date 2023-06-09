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
    private float [,,] orig_textureAlphas;
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
        orig_textureAlphas = td.GetAlphamaps(0,0, td.alphamapWidth, td.alphamapHeight);
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
    private (int, int) TerrainAlphasFromLocalCoordinates(Vector3 localCoordinates) {
        float relativeTerCoordX = td.alphamapWidth * localCoordinates.x;
        float relativeTerCoordZ = td.alphamapHeight * localCoordinates.z;
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
        (int hitPointAlphX, int hitPointAlphZ) = TerrainAlphasFromLocalCoordinates(local);
        if (hitPointAlphX < 1 || hitPointAlphX >= td.alphamapWidth-1 || hitPointAlphZ < 1 || hitPointAlphZ >= td.alphamapHeight -1) {
            Debug.LogError("Hit at a point off the terrain : " + hitPointAlphX + " " + hitPointAlphZ);
            Debug.LogError("Size is " + td.alphamapWidth + " " + td.alphamapHeight);
            Debug.LogError("resolution is " + td.alphamapResolution);
            return;
        }
        float[,,] maps = td.GetAlphamaps(hitPointAlphX-1, hitPointAlphZ-1, 3, 3);
        for (int i = 0; i < maps.GetLength(0); i++)
        {
            for (int j = 0; j < maps.GetLength(1); j++) {
                maps[i,j,0] = 0.0f;
                maps[i,j,1] = 0.0f;
                maps[i,j,2] = 0.0f;
                maps[i,j,3] = 1.0f;  //Burned crater color
            }
        }

        td.SetAlphamaps(hitPointAlphX-1, hitPointAlphZ-1, maps);

    }
    private void OnApplicationQuit() {
        td.SetHeights( 0, 0, orig_heights);
        td.SetAlphamaps(0,0,orig_textureAlphas);
    }
}
