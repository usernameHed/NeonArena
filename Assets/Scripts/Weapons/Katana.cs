using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class Katana : Weapon
{
    [FoldoutGroup("Gameplay")] [Tooltip("la vitesse de turnRate du weapon")] [SerializeField] private float turnRate = 2000f;
    [FoldoutGroup("Gameplay")] [Tooltip("Speed du player when dashing")] [SerializeField] private float dashSpeed = 30f;
    [FoldoutGroup("Gameplay")] [Tooltip("temps de dash...")] [SerializeField] private float dashTime = 0.5f;
    [FoldoutGroup("Gameplay")] [Tooltip("forward du swouch")] [SerializeField] private float forwardSwouch = 1f;
    [FoldoutGroup("Gameplay")] [Tooltip("mini temps avant que le swouch tue")] [SerializeField] private float timeSwouchKill = 0.2f;
    [FoldoutGroup("Gameplay")] [Tooltip("range du katana")] [SerializeField] private float rangeKatana = 2f;

    [SerializeField]
    private GameObject katanaBeforeSwouchPrefab;
    [SerializeField]
	private GameObject katanaSwouchPrefab;
    [SerializeField]
    private GameObject overlapDestroyPrefabs;


    [SerializeField]
	private float offset;

    private GameObject katanaSwouchTmp; //sauvegarde du prefabs swouch

    public bool isDashing = false;

	protected override void Shoot(float rotation)
    {
		if (isDashing)
			return;

        PC.animatorPlayer.SetBool("Fire", true);

        SoundManager.Instance.PlaySound (SoundManager.Instance.KatanaSound);

		isDashing = true;
		PC.stopMove = true;

		PC.rigidbodyPlayer.velocity = Vector3.zero;
		PC.rigidbodyPlayer.useGravity = false;
		PC.rigidbodyPlayer.AddForce(transform.up * dashSpeed, ForceMode.Impulse);

        Invoke("StartHit", dashTime);

        katanaSwouchTmp = Instantiate (katanaBeforeSwouchPrefab, transform.position + (transform.up), transform.rotation, transform);
        katanaSwouchTmp.GetComponent<TriggerKatana>().setActive(this);
    }

    /// <summary>
    /// le katana viens de finir sa tragectoire
    /// </summary>
    public void StartHit()
    {
        CancelInvoke();
        Destroy(katanaSwouchTmp);

        GameObject katanaSwouch = Instantiate(katanaSwouchPrefab, transform.position + (transform.up * forwardSwouch), transform.rotation, transform);
		katanaSwouch.transform.rotation = Quaternion.Euler (new Vector3(0.0F, 0.0F, transform.rotation.eulerAngles.z + 90.0F));

		GameObject overlapDestroy = Instantiate(overlapDestroyPrefabs, transform.position + (transform.up * forwardSwouch), transform.rotation);

        OverlapDestroy OD = overlapDestroy.GetComponent<OverlapDestroy>();
        OD.startKill(rangeKatana, rangeKatana, PC.gameObject, timeSwouchKill, PC.gameObject);

		PC.stopMove = false;
		isDashing = false;

		PC.rigidbodyPlayer.velocity = Vector3.zero;
		PC.rigidbodyPlayer.useGravity = true;
    }

    /// <summary>
    /// visé avec le joueur
    /// </summary>
    public override void Dir(float horizMove, float vertiMove)
    {
        if (isDashing)
            return;

        float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, vertiMove * turnRate * Time.deltaTime);

        Quaternion _targetRotation = Quaternion.identity;

        _targetRotation = Quaternion.Euler(0f, 0f, -heading * Mathf.Rad2Deg);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnRate * Time.deltaTime);
    }
}
