using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceIn : MonoBehaviour
{
	public delegate void BounceFinished();

	[SerializeField]
	private float timeToWait;

	void OnEnable()
	{
		StartCoroutine (BounceAnimation(true));	
	}

	public IEnumerator BounceAnimation(bool growing, BounceFinished callBack = null)
	{
		transform.localScale = Vector3.zero;
		if (timeToWait > 0)
		{
			yield return new WaitForSecondsRealtime (timeToWait);
		}

		float startTime = Time.realtimeSinceStartup;
		float time = 0.0F;

		while (time <= 1.0F)
		{
			time += (Time.realtimeSinceStartup - startTime);
			float scaleValue = Easing.Back.Out (Mathf.Max((growing ? time : 1.0F - time) / 2.0F, 0.0F));
			transform.localScale = new Vector3(scaleValue, scaleValue, 0.0F);

			yield return null;
		}

		transform.localScale = growing ? Vector3.one : Vector3.zero;

		if (callBack != null)
		{
			callBack ();
		}
	}
}
