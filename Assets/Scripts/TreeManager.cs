using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    
    [SerializeField] private GameObject[] treeTypes;
    // Start is called before the first frame update
    void Start()
    {
		TerrainData terrain = Terrain.activeTerrain.terrainData;
//		
//		ArrayList instances = new ArrayList();
//		
		foreach (TreeInstance tree in terrain.treeInstances) {
//			float distance = Vector3.Distance(Vector3.Scale(tree.position, terrain.size) + Terrain.activeTerrain.transform.position, transform.position);
//			if (distance<BlastRange) {
//				// the tree is in range - destroy it
//				GameObject dead = Instantiate(DeadReplace, Vector3.Scale(tree.position, terrain.size) + Terrain.activeTerrain.transform.position, Quaternion.identity) as GameObject;
//				dead.GetComponent<Rigidbody>().maxAngularVelocity = 1;
//				dead.GetComponent<Rigidbody>().AddExplosionForce(BlastForce, transform.position, BlastRange*5, 0.0f);
//			} else {
//				// tree is out of range - keep it
//				instances.Add(tree);
//				
//			}
            GameObject newTree = Instantiate(treeTypes[tree.prototypeIndex], Vector3.Scale(tree.position, terrain.size) + Terrain.activeTerrain.transform.position, Quaternion.Euler(0.0f, Mathf.Rad2Deg * tree.rotation, 0.0f));
            newTree.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            
		}
//		terrain.treeInstances = (TreeInstance[])instances.ToArray(typeof(TreeInstance));
                                                
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }
}
    