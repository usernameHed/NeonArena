using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesPrefabs : MonoBehaviour
{
	// Singleton
	private static TilesPrefabs instance;
	public static TilesPrefabs Instance
	{
		get { return instance; }
	}

	// Prefabs
	public GameObject StaticTile;
	public GameObject SpikesTile;
	public GameObject TrampolineTile;
	public GameObject TNTTile;

	// Materials
	//public Material Grass;
	//public Material Dirt;
	//public Material Wall;
	public Material Destructible;
	public Material Indestructible;
	public Material Spikes;
	public Material Trampoline;
	public Material TNT;

	public TilesPrefabs()
	{
		instance = this;
	}
}
