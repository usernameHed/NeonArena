using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReader
{
	public static MapData LoadMap(TextAsset mapJson)
	{
		//Parse json into class
		return JsonUtility.FromJson<MapData>(mapJson.text);
	}
}

[System.Serializable]
public class MapData
{
	public int width;
	public int height;
	public MapLayer[] layers;
}

[System.Serializable]
public class MapLayer
{
	public int[] data;
	public string name;
}