using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{
	// Destroy player on touch
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player"))
		{
			other.gameObject.GetComponent<PlayerController> ().RpcKill();
		}
	}
}
