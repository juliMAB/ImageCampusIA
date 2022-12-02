using UnityEngine;

public abstract class Tile : MonoBehaviour
{

    #region PUBLIC_FIELDS

    public Vector2Int pos = new Vector2Int();

    #endregion

    #region EXPOSED_FIELD
    [SerializeField] private bool _isWalkable;
    #endregion


    #region PUBLIC_METHODS
    public virtual void Init(int x, int y)
    {
        pos = new Vector2Int(x, y);
    }
    #endregion
}