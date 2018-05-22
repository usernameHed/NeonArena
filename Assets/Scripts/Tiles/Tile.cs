using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
	private static Tile[] tiles = new Tile[256];

	//public static Tile GRASS = new Tile(1, TilesPrefabs.Instance.StaticTile, TilesPrefabs.Instance.Grass);
	//public static Tile DIRT = new Tile(2, TilesPrefabs.Instance.StaticTile, TilesPrefabs.Instance.Dirt);
	public static Tile DESTRUCTIBLE = new Tile(1, TilesPrefabs.Instance.StaticTile, TilesPrefabs.Instance.Destructible);
	public static Tile INDESTRUCTIBLE = new Tile(10, TilesPrefabs.Instance.StaticTile, TilesPrefabs.Instance.Indestructible, 0.0F, false);

	public static Tile SPIKES_UP = new Tile(4, TilesPrefabs.Instance.SpikesTile, TilesPrefabs.Instance.Spikes);
	public static Tile SPIKES_LEFT = new Tile(5, TilesPrefabs.Instance.SpikesTile, TilesPrefabs.Instance.Spikes, 90.0F);
	public static Tile SPIKES_DOWN = new Tile(6, TilesPrefabs.Instance.SpikesTile, TilesPrefabs.Instance.Spikes, 180.0F);
	public static Tile SPIKES_RIGHT = new Tile(7, TilesPrefabs.Instance.SpikesTile, TilesPrefabs.Instance.Spikes, 270.0F);

	public static Tile TRAMPOLINE = new Tile (8, TilesPrefabs.Instance.TrampolineTile, TilesPrefabs.Instance.Trampoline);
	public static Tile TNT = new Tile (9, TilesPrefabs.Instance.TNTTile, TilesPrefabs.Instance.TNT);

	public static PlayerSpawn PLAYER_SPAWN = new PlayerSpawn(12);

	private GameObject tilePrefab;
	private Material tileMaterial;
	private float zRotation;
	private bool destructible;

	public Tile(int id, GameObject tilePrefab, Material tileMaterial, float zRotation = 0.0F, bool destructible = true)
	{
		this.tileMaterial = tileMaterial;
		this.tilePrefab = tilePrefab;
		this.zRotation = zRotation;
		this.destructible = destructible;
		tiles [id] = this;
	}

	public virtual void CreateTile (GameObject parent, int x, int y, float zIndex)
	{
		GameObject tile = GameObject.Instantiate (tilePrefab, new Vector3 ((float) x, (float) y, zIndex), Quaternion.Euler(new Vector3(0.0F, 0.0F, zRotation)));
		TilesManager tm = tile.GetComponent<TilesManager> ();
		tm.isDestructible = destructible;
		tile.transform.SetParent(parent.transform);
		tile.GetComponent<Renderer> ().material = tileMaterial;
        //if (tile.INDESTRUCTIBLE)
          //  tile.GetComponent<TilesManager>().isDestructible = false;

    }

	public static Tile GetTile(int id)
	{
		return tiles [id];
	}
}
