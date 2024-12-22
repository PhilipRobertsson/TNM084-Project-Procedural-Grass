using UnityEngine;

public class LevelGeneration : MonoBehaviour{
    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;
    [SerializeField]
    private GameObject tilePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        GenerateMap();
    }

    void GenerateMap(){
        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // for each Tile, instantiate a Tile in the correct position
        for (int iXT= 0; iXT < mapWidthInTiles; iXT++){
            for (int iZT = 0; iZT < mapDepthInTiles; iZT++){
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + iXT * tileWidth,
                  this.gameObject.transform.position.y,
                  this.gameObject.transform.position.z + iZT * tileDepth);
                // instantiate a new Tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
            }
        }
    }
}
