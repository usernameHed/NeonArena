using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class PanelPlayer : MonoBehaviour
{
    #region variable
    [FoldoutGroup("Gameplay")] [Tooltip("id du weapon")] [SerializeField] private int weaponId = 0;
    [FoldoutGroup("Gameplay")] [Tooltip("weapon")] [SerializeField] private Image weaponImage;
    [FoldoutGroup("Gameplay")] [Tooltip("weapon")] [SerializeField] private TextMeshProUGUI textScore;

    [FoldoutGroup("Debug")] [Tooltip("id du panel")] public int idPanel = 1;
    [FoldoutGroup("Debug")] [Tooltip("sprites de katana")] public Sprite katana;
    [FoldoutGroup("Debug")] [Tooltip("sprites des rocketJump")] public Sprite rocket;
    [FoldoutGroup("Debug")] [Tooltip("sprites des rocketJump")] public Sprite flame;

	public PlayerController PC;

    private float timeToGo;

    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)]  private float timeOpti = 0.1f;

    #endregion

    #region  initialisation
    //references
    private void Awake()
    {
        //gameObject.SetActive(false);
    }

    //initialisation
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                                   //setup le temps
    }
    #endregion

    #region core script
    /// <summary>
    /// change l'id du weapons
    /// </summary>
    /// <param name="id"></param>
    public void changeWeaponId(int id)
    {
        weaponId = id;
        if (weaponId == 0)
            weaponImage.sprite = katana;
        else if (weaponId == 1)
            weaponImage.sprite = flame;
        else if (weaponId == 2)
            weaponImage.sprite = rocket;

    }

    public void changeScorePlayer(int score)
    {
        textScore.text = score.ToString();
		textScore.GetComponent<BounceBack> ().Bounce (5.0F, 0.5F);
    }
    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void Update()
    {
		if(PC != null && PC.weaponScript != null)
			weaponImage.fillAmount = PC.weaponScript.WeaponPercent ();
        if (Time.fixedTime >= timeToGo)                                         //effectué à chaque opti frame
        {

            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
        Destroy(gameObject);
    }
    #endregion
}
