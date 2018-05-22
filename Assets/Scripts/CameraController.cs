using System;
using UnityEngine;
using System.Collections.Generic;

/*
edit:
https://www.draw.io/?state=%7B%22ids%22:%5B%220Byzet-SVq6ipWGFSX1RhNlhHeGc%22%5D,%22action%22:%22open%22,%22userId%22:%22113268299787013782381%22%7D#G0Byzet-SVq6ipWGFSX1RhNlhHeGc
see:
https://drive.google.com/file/d/0Byzet-SVq6ipVVEzQWNLUnRTbHM/view
*/

/// <summary>
/// Gère la caméra, plusieurs facteurs influent:
/// - les joueurs
/// - les objets à focus
/// La caméra a pour but de se placer à la position m_DesiredPosition en smooth(lissé).
/// Cette position correspond au barycentre de la liste d'objet Targets.
/// </summary>

public class CameraController : MonoBehaviour
{
    [Range(0, 5f)]    public float m_DampTime = 0.2f;           //Temps d'attente de la caméra pour changer de position
    [Range(0, 150f)]    public float minZoom = 4.0f;             //Zoom minimum
    [Range(5, 150f)]    public float maxZoom = 15.0f;           //Zoom maximum - étape 1
    [Range(5, 150f)]    public float maxZoomMax = 15.0f;        //Zoom maximum au-delà du maximum de base (si le zoom a besoin d'être agrandi)
    [Range(5, 150f)]    public float maxOfMaxMax = 30.0f;       //Zoom Max que le maxZoomMax peut aller
    public float basicZoom = 6.0f;                              //Zoom de base quand il n'y a qu'une seule Target
    [Range(0, 50f)]    public float EdgeMarge = 4f;             //Espace entre les cibles et les bords de la map
    [Range(0, 10f)]    public float reduceWhenTooBig = 0f;      //Zoom quand celui-ci est trop grand
    [Range(0, 10f)]    public float addWhenTooSmall = 0f;       //Dezoom quand celui-ci est trop petit (lorsque l'une des targets touche le mur !)
    [Range(0, 50f)]    public float dezoomCoef = 4;             //Dezoom quand une cible est hors de la map (la caméra a besoin de dézoomer !)
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;           //Optimisation des fps
    [Range(0, 100f)]    public float margeErrorAddCam = 1f;   //Marge de distance avant d'ajouter à la caméra

    [Space(10)]
    public Transform alternativeFocus;                          //Target alternative à focus s'il n'y en a aucune

    [Space(10)]
    [Header("Debug")]
    public List<GameObject> Targets = new List<GameObject>();           //Toutes les Targets, joueur inclus
    public GameObject walls;                                            //Référence aux murs de la caméra
    public bool showWalls = false;                                      //Variable qui définit si l'on doit afficher ou non les murs
    public bool isOnAlternativeFocus = false;                           //défini si la caméra est arrivé sur son focus alternatif
    public bool isOnFocusQueen = false;                                 //le jeu est fini, la caméra se trouve sur la reine (en multi)
    public TimeWithNoEffect TWNE;                                  //temps au début du jeu qui focus sur la queen
   


    /// <summary>
    /// 
    /// </summary>

    private Vector3 m_MoveVelocity;                 //Vitesse de référence, qu'est-ce qu'elle fout là ???
    public  Vector3 m_DesiredPosition;              //La position où la caméra est en train de se déplacer.
    private float timeToGo;                         //Variable d'optimisation
    private float timeZoom;                         //Variable d'optimisation
    public float timeZoomOpti = 2f;                         //Variable d'optimisation

    private GameObject gameController;              //Référence du GameController
    private bool stopMoving = false;                //arrete de bouger la camera !
    private float saveDistanceWhenAdding = -1;      //sauvegarde la distance courante juste avant d'ajouter à la caméra

    /// <summary>
    /// Initialise le gameController
    /// </summary>
    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        TWNE = gameObject.GetComponent<TimeWithNoEffect>();
    }

    /// <summary>
    /// Initialise l'optimisation et le maxZoomMax au zoom
    /// </summary>
    void Start()
    {
        maxZoomMax = maxZoom;
        timeToGo = Time.fixedTime + timeOpti;
        timeZoom = Time.fixedTime - 1;
        saveDistanceWhenAdding = -1;
    }

    /// <summary>
    /// Ajoute un objet la liste Targets pour la caméra
    /// </summary>
    public void addToCam(GameObject other)
    {
        if (Targets.IndexOf(other) < 0)                         //Si l'objet n'est pas déjà dans l'array !
        {
            stopMoving = false;
            Targets.Add(other);                                 //Ajoute un objet
        }
            
    }

    /// <summary>
    /// Supprime un objet s'il se trouve dans la liste
    /// </summary>
    /// <param name="other">objet à supprimer de la liste</param>
    public void deleteToCam(GameObject other)
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].GetInstanceID() == other.GetInstanceID())
            {
                Targets.RemoveAt(i);
                return;
            }                
        }
    }

    /// <summary>
    /// Garde le maxZoomMax toujours supérieur ou égal au zoom
    /// </summary>
    void Zoom()
    {
        if (maxZoomMax < maxZoom)
        {
            maxZoomMax = maxZoom;
        }

        /*if (maxZoomMax > maxOfMaxMax)
        {
            maxZoomMax = maxOfMaxMax;
            gameController.GetComponent<DistanceForWalls>().distance += maxOfMaxMax;
        }*/
            
    }

    /// <summary>
    /// Actualise les listes de la caméra en supprimant les cases vides
    /// (Si un objet a été détruit, la case devient vide)
    /// </summary>
    private void ActualizeTarget()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            //Si la target est désactivée ou inexistante, supprimer la case du tableau
            if (!Targets[i] || !Targets[i].gameObject.activeSelf)
                Targets.Remove(Targets[i]);
        }
    }

    /// <summary>
    /// clear la liste des targets
    /// </summary>
    public void clearTarget()
    {
        Debug.Log("ici clean target");
        Targets.Clear();
    }

    /// <summary>
    /// Définit la position où la caméra doit se rendre
    /// </summary>
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();                            //Position finale
        int numTargets = 0;                                            //Compte le nombre de targets de la caméra
        float minX = 0;                                                //Définit le min/max en X et Y
        float maxX = 0;
        float minY = 0;
        float maxY = 0;
        float biggestDist = 0.0f;

        //Boucle sur chaque target
        for (int i = 0; i < Targets.Count; i++)
        {
            GameObject tmpTarget = Targets[i];
            if (!tmpTarget || !tmpTarget.activeSelf)       //Si elle est inactive, passer à la suivante
                continue;

            //si c'est un player, prendre son pointSmooth !
            //if (tmpTarget.CompareTag("Player"))
              //  tmpTarget = tmpTarget.GetComponent<PlayerController>().pointCamSmooth.gameObject;

            if (tmpTarget.transform.position == Vector3.zero)
            {
                Debug.Log("oko totototo");
            }
            if (i == 0)                                                 //Si c'est le début de boucle,
            {                                                           //Définit min/max à la première target
                minX = maxX = tmpTarget.transform.position.x;          //  en x
                minY = maxY = tmpTarget.transform.position.y;          //  en y
            }
            else                                                        //Sinon, agrandit/diminue selon la target courante pour définir un carré qui englobe toutes les targets
            {
                minX = (tmpTarget.transform.position.x < minX) ? tmpTarget.transform.position.x : minX;
                maxX = (tmpTarget.transform.position.x > maxX) ? tmpTarget.transform.position.x : maxX;
                minY = (tmpTarget.transform.position.y < minY) ? tmpTarget.transform.position.y : minY;
                maxY = (tmpTarget.transform.position.y > maxY) ? tmpTarget.transform.position.y : maxY;
            }

            numTargets++;                                               //Ajoute le compteur pour connaître le nombre de targets actives
        }

        //S'il y a plus d'une target, définit le milieu par rapport aux min/max !
        if (numTargets > 0)
        {
            averagePos.x = (minX + maxX) / 2;
            averagePos.y = (minY + maxY) / 2;
        }
        //S'il n'y a aucune target... sélectionne le focus alternatif !
        if (Targets.Count == 0)
        {
            if (alternativeFocus)
                averagePos = alternativeFocus.position;
            else
            {
                stopMoving = true;
                return;
            }
        }
        else
        {
            stopMoving = false;
        }


        //Ajoute au zoom biggestDist
        //(Vous vous souvenez, c'est juste en haut, si une target s'est retrouvée hors de l'écran)
        maxZoomMax += biggestDist;

        //Définit la distance à mesurer
        float dist = Mathf.Max(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY));
        averagePos.z = (Targets.Count > 1) ? -Mathf.Min(Mathf.Max(minZoom, dist + EdgeMarge), Mathf.Max(maxZoom, maxZoomMax)) : -basicZoom;
        
         //Finalement, change la cible pour la caméra
        m_DesiredPosition = averagePos;
    }

    /// <summary>
    /// N'est jamais appelé pour l'instant
    /// Initialise la position de la caméra
    /// </summary>
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();                                      // Trouve le point cible pour la caméra
        transform.position = m_DesiredPosition;
    }

    /// <summary>
    /// renvoi true si la caméra se trouve sur sa position visé
    /// </summary>
    /// <returns></returns>
    bool camIsOnFocus(Transform desiredPos)
    {
        if (transform.position.x > desiredPos.position.x - 1
                && transform.position.x < desiredPos.position.x + 1
                && transform.position.y > desiredPos.position.y - 1
                && transform.position.y < desiredPos.position.y + 1)
        {
            return (true);
        }
        return (false);
    }

    private void Update()
    {
        if (/*Time.fixedTime >= timeToGo && */!stopMoving)
        {
            Zoom();
            ActualizeTarget();
            FindAveragePosition();
            //timeToGo = Time.fixedTime + timeOpti;
        } 
    }

    /// <summary>
    /// Bouge la caméra lentement (smoothy) vers la position souhaitée
    /// </summary>
    private void LateUpdate()
    {
        if (stopMoving)
            return;
        //bouger vers la position souhaité
        //transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        transform.position = Vector3.Lerp(transform.position, m_DesiredPosition, m_DampTime);
        //transform.position = 
    }
}