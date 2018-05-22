using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	private static MapManager instance;
	public static MapManager Instance
	{
		get { return instance; }
	}

	[SerializeField]
	private TextAsset mapToLoad;

	[SerializeField]
	private GameObject mapBorder;

	public int width;
	public int height;
	private int[] foreground;
	private int[] background;
	private int spawnRobin = 0;
	private List<Vector3> spawnPositions;
	public List<Vector3> SpawnPositions
	{
		get { return spawnPositions; }
	}

	MapManager()
	{
		instance = this;
	}

	void Start()
	{
        Debug.Log("start mapmanager");
		LoadMap ();
	}

	// Load map and apply it
	public void LoadMap()
	{
		// Load map data from JSON
		MapData data = MapReader.LoadMap (mapToLoad);
		width = data.width;
		height = data.height;

		bool shallSwap = !data.layers [0].name.Equals ("Foreground");
		foreground = shallSwap ? data.layers [1].data : data.layers [0].data;
		background = shallSwap ? data.layers [0].data : data.layers [1].data;

		spawnPositions = new List<Vector3> ();
		CreateMap ();
		spawnRobin = Random.Range (0, spawnPositions.Count);
	}

	// Prepare new map
	void CreateMap()
	{
		// Destroy remaining instances
		/*for (int i = 0; i < transform.childCount; i++)
		{
            Destroy (transform.GetChild (i));
		}*/

		CreateTerrain (foreground, 0.0F);
		CreateTerrain (background, 1.0F);
		AddMapBorders ();
	}

	// Place game object
	void CreateTerrain(int[] layer, float zIndex)
	{
		for (int i = 0; i < layer.Length; i++)
		{
			int x = i % width;
			int y = i / width;

			int tileId = layer [i];

			if (tileId > 0)
			{

				Tile tile = Tile.GetTile (tileId);
				if (tile != null)
				{
					tile.CreateTile (gameObject, x, -y + height - 1, zIndex);
				}
			}
		}
	}

	// Add map collision borders
	void AddMapBorders()
	{
		BoxCollider bottomCollider = Instantiate (mapBorder, new Vector3(width / 2.0F - 0.5F, -0.5F, 0.0F), Quaternion.identity).GetComponent<BoxCollider>();
		BoxCollider topCollider = Instantiate (mapBorder, new Vector3(width / 2.0F - 0.5F, height - 0.5F, 0.0F), Quaternion.identity).GetComponent<BoxCollider>();
		BoxCollider leftCollider = Instantiate (mapBorder, new Vector3(-0.5F, height / 2.0F - 0.5F, 0.0F), Quaternion.identity).GetComponent<BoxCollider>();
		BoxCollider rightCollider = Instantiate (mapBorder, new Vector3(width - 0.5F, height / 2.0F - 0.5F, 0.0F), Quaternion.identity).GetComponent<BoxCollider>();

		// Set center
		bottomCollider.center = new Vector3 (0.0F, -0.5F, 0.0F);
		topCollider.center = new Vector3 (0.0F, 0.5F, 0.0F);
		leftCollider.center = new Vector3 (-0.5F, 0.0F, 0.0F);
		rightCollider.center = new Vector3 (0.5F, 0.0F, 0.0F);

		// Set collision size
		topCollider.size = bottomCollider.size = new Vector3 (width + 2.0F, 1.0F, 1.0F);
		leftCollider.size = rightCollider.size = new Vector3 (1.0F, height + 2.0F, 1.0F);
	}

	public Vector3 GetRandomSpawnPoint()
	{
		int spawnPoint = spawnRobin;
		spawnRobin = (spawnRobin + 1) % spawnPositions.Count;
		return spawnPositions[spawnPoint];
	}
}
