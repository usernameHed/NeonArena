using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// description
/// <summary>

[System.Serializable]
public class SoundGroup
{
	public AudioClip[] clips;
}

//[RequireComponent(typeof(CircleCollider2D))]
public class SoundManager : MonoBehaviour
{
	private static SoundManager instance;
	public static SoundManager Instance
	{
		get { return instance; }
	}
	[FoldoutGroup("Audios")]
	public SoundGroup TNTSound;

	[FoldoutGroup("Audios")]
	public SoundGroup KatanaReadySound;

	[FoldoutGroup("Audios")]
	public SoundGroup KatanaSound;

	public AudioClip RocketReadySound;

	public AudioClip RocketLaunchSound;

	[FoldoutGroup("Audios")]
	public SoundGroup RocketSound;

	public AudioClip EmptyFlameThrowerSound;

	public AudioClip FlameThrowerSound;

	[FoldoutGroup("Audios")]
	public SoundGroup BlockDeathSound;

	public AudioClip JumperSound;

	public AudioClip DeathSound;

	public AudioClip LandingSound;

	public AudioClip WallJumpSound;

	public AudioClip JumpSound;

	public AudioClip MenuSound;

	[SerializeField]
	private AudioSource soundPlayer;

	public SoundManager()
	{
		instance = this;
	}

	public void PlaySound(SoundGroup soundGroup, float pitch = 1.0F, float pitchVariance = 0.0F, float volume = 1.0F)
	{
		PlaySound (soundGroup.clips[Random.Range(0, soundGroup.clips.Length)], pitch, pitchVariance, volume);
	}

	public void PlaySound(AudioClip audioClip, float pitch = 1.0F, float pitchVariance = 0.0F, float volume = 1.0F)
	{
		AudioSource sp = (AudioSource) Instantiate (soundPlayer, Vector3.zero, Quaternion.identity);
		sp.clip = audioClip;
		sp.volume = volume;
		float pv = pitchVariance / 2.0F;
		sp.pitch = pitch + Random.Range (-pv, pv);

		sp.Play ();
		StartCoroutine (WaitBeforeSoundDestroy(sp));
	}
    
	IEnumerator WaitBeforeSoundDestroy(AudioSource audio)
	{
        yield return new WaitForSeconds (audio.clip.length);
        Destroy (audio.gameObject);
	}
    
}
