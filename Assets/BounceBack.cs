using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBack : MonoBehaviour
{
	public void Bounce(float force, float time)
	{
		if (gameObject.activeSelf)
		{
			StartCoroutine (BounceAnimation (force, time));
		}
	}

	IEnumerator BounceAnimation(float force, float time)
	{
		transform.localScale = Vector3.one;

		float timeMultiplier = 1.0F / time;
		float timer = time;
		while(timer > 0.0F)
		{
			transform.localScale = Vector3.one + Vector3.one * Easing.Back.In(timer * timeMultiplier) * force;
			timer -= Time.deltaTime;
			yield return null;
		}

		transform.localScale = Vector3.one;
	}
}
