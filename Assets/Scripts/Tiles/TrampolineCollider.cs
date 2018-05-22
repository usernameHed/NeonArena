using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineCollider : MonoBehaviour
{
	[SerializeField]
	private float jumpForce = 20.0F;

	// Trampoline jump
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player"))
		{
			SoundManager.Instance.PlaySound (SoundManager.Instance.JumperSound);
			Rigidbody playerBody = other.GetComponent<Rigidbody> ();
            Vector3 posBump = new Vector3(transform.position.x, other.transform.position.y + 1f, 0);
            Instantiate(other.GetComponent<PlayerController>().prefabsBump, posBump, Quaternion.identity);
            //playerBody.velocity = new Vector3 (playerBody.velocity.x, jumpForce, playerBody.velocity.z);
            playerBody.velocity = new Vector3(playerBody.velocity.x, 0, 0);
            playerBody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
		}
	}
}
