using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Weapon : MonoBehaviour
{
    [FoldoutGroup("Debug")] [Tooltip("ref sur playerController")] public PlayerController PC;
	[FoldoutGroup("Gameplay")] [Tooltip("ref sur playerController")] public float cooldown;
	private float nextShoot;

	void LateUpdate()
	{
		if (nextShoot > 0)
		{
			nextShoot -= Time.deltaTime;
		}
	}

	public void TryShoot(float rotation)
	{
		if (nextShoot <= 0)
		{
			nextShoot = cooldown;
			Shoot (rotation);
		}
	}

	abstract protected void Shoot (float rotation);

    abstract public void Dir(float horizMove, float vertiMove);

	public virtual void OnShootRelease(){}

	public virtual float WeaponPercent()
	{
		return Mathf.Clamp((cooldown - nextShoot) / cooldown, 0.0F, 1.0F);
	}

}
