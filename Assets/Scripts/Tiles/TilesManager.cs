using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class TilesManager : MonoBehaviour
{
    #region variable
    [FoldoutGroup("Gameplay")] [Tooltip("le material du background")]
    public Material matBackground;

    [FoldoutGroup("Gameplay")] [Tooltip("la bloc est-il destructible ")]
    public bool isDestructible = false;

    [FoldoutGroup("Gameplay")] [Tooltip("temps avant de respawn")]
    public float timeBeforeRespawn = 10f;

    [FoldoutGroup("Gameplay")] [Tooltip("warning time")]
    public float timeWarningRespawn = 10f;

    private Material matDefault;

    private float timeToGo;

    [FoldoutGroup("Debug")] [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)]
    private float timeOpti = 0.1f;

    private bool isActive = true;

    [FoldoutGroup("Debug")] [Tooltip("prefabs de la particule de mort")]
    public GameObject prefabsDeadObject;

    [FoldoutGroup("Debug")] [Tooltip("prefabs objet warning")]
    public GameObject prefabsWarningObject;

    [FoldoutGroup("Debug")] [Tooltip("prefabs objet warning")]
    public GameObject prefabsOverlapsDestroy;

    private GameObject warningParticle;

    #endregion

    #region  initialisation
    //initialisation
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                                   //setup le temps
        matDefault = GetComponent<MeshRenderer>().material;
        startEverything();
    }
    #endregion

    #region core script

    /// <summary>
    /// reset tout
    /// </summary>
    private void startEverything()
    {
        isActive = true;
        gameObject.GetComponent<MeshRenderer>().material = matDefault;
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    /// <summary>
    /// désactive, et attent avant de respawn
    /// </summary>
    /// <returns></returns>
    private IEnumerator desactiveAndWaitbeforeReset()
    {
        GameObject deadParticle = Instantiate(prefabsDeadObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        gameObject.GetComponent<MeshRenderer>().material = matBackground;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        //wait X seconde
        yield return new WaitForSeconds(timeBeforeRespawn);

        GameObject overlap = Instantiate(prefabsOverlapsDestroy, gameObject.transform.position, Quaternion.identity) as GameObject;
        overlap.GetComponent<OverlapDestroy>().destroyTiles = false;
        overlap.GetComponent<OverlapDestroy>().startKill(0.5f, 0.5f, null, 0, null);

        Destroy(warningParticle);
        startEverything();
    }

    /// <summary>
    /// désactive, et attent avant de respawn
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitForCreateWarning()
    {
        //wait X seconde
        yield return new WaitForSeconds(timeWarningRespawn);
        warningParticle = Instantiate(prefabsWarningObject, gameObject.transform.position, Quaternion.identity) as GameObject;
    }
    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void Update()
    {
        if (Time.fixedTime >= timeToGo)                                         //effectué à chaque opti frame
        {

            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public virtual bool destroyThis()
    {
        if (!isDestructible || !isActive)
            return false;
        isActive = false;

        StartCoroutine(desactiveAndWaitbeforeReset());    //respawn la tiles
        StartCoroutine(waitForCreateWarning());    //respawn la tiles

		return true;
    }
    #endregion
}
