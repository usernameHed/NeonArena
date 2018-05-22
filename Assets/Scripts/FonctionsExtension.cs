using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public static class FonctionsExtension
{
    #region core script
    /// <summary>
    /// clean les transforms
    /// </summary>
    public static Transform ClearChild(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }
    #endregion
}
