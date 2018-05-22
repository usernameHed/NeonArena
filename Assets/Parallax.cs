using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	private float height;

	// Use this for initialization
	void Start ()
	{
		Renderer render = GetComponent<Renderer> ();
		height = render.bounds.extents.y * 2.0F;
	}

	// Update is called once per frame
	void Update ()
	{
		float frustumHeight = 2.0f * Mathf.Abs(Camera.main.transform.position.z - transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float scale = frustumHeight / height;
		transform.localScale = new Vector3(scale, scale, 0.0F);
		transform.position = new Vector3 (MapManager.Instance.width / 2.0F - 0.5F, Camera.main.transform.position.y, transform.position.z);
	}
}
