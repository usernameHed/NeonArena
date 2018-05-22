using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : Tile
{
	public PlayerSpawn(int id) : base(id, null, null, 0.0F)
	{
		
	}

	public override void CreateTile (GameObject parent, int x, int y, float zIndex)
	{
		MapManager.Instance.SpawnPositions.Add (new Vector3(x + 0.5F, y + 0.5F, 0.0F));
	}
}
