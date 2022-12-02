using System.Collections.Generic;
using System.Linq; //para usar Any en list.
using UnityEngine;
using UnityEngine.UI;

namespace Diciembre
{
    public class ResourceSpawner : MonoBehaviour
    {
        #region EXPOSED_FIELD
        [SerializeField] private Button SpawnResourceButton;

        [SerializeField] private GameObject resourcePrefab;
        [SerializeField] private CentroUrbano townCenter;
        [SerializeField] private Transform resourceConteiner;

        [SerializeField] private int maxResources;
        #endregion

        #region PRIVATE_FIELDS
        private List<Resource> resources = new List<Resource>();
        #endregion

        #region PUBLIC_METHODS
        private void SpawnResource()
        {
            if (!(maxResources >= resources.Count))
                return;

            Vector2Int randPos = Vector2Int.zero;
            int index = 0;
            int iterations = 0;

            do
            {
                iterations++;
                if (iterations > 100)
                {
                    Debug.Log("maximo iteraciones");
                    return;
                }
                randPos = new Vector2Int(Random.Range(0, Main.MapSize.x+1), Random.Range(0, Main.MapSize.y+1));
                index = NodeUtils.PositionToIndex(randPos);
            } while (!CanSpawn(index,randPos));

            GameObject resourceGO = Instantiate(resourcePrefab, new Vector3(randPos.x,randPos.y), Quaternion.identity, resourceConteiner);
            Resource agent = resourceGO.GetComponent<Resource>();
            agent.OnEmpty += DeleteResourceReference;
            resources.Add(agent);
        }
        #endregion

        #region PRIVATE_METHODS
        private void DeleteResourceReference(Resource resource)
        {
            resources.Remove(resource);
            Destroy(resource.gameObject);
        }
        private bool CanSpawn(int index, Vector2Int randPos)
        {
            if (invalidIndex(index))
                return false;
            if (invalidWeight(index))
                return false;
            if (hasResource(randPos))
                return false;
            if (SamePosAsTown(randPos))
                return false;
            return true;
        }
        private bool invalidIndex(int index)
        {
            return index == -1;
        }
        private bool invalidWeight(int index)
        {
            return Main.mainMap[index].weight != (int)TILE_TYPE.GRASS;
        }
        private bool hasResource(Vector2Int randPos)
        {
            return resources.Any(p => new Vector2Int(Mathf.RoundToInt(p.transform.position.x), Mathf.RoundToInt(p.transform.position.y)) == randPos);
        }
        private bool SamePosAsTown(Vector2Int randPos)
        {
            return randPos == new Vector2Int(Mathf.RoundToInt(townCenter.transform.position.x), Mathf.RoundToInt(townCenter.transform.position.y));
        }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            SpawnResourceButton.onClick.AddListener(SpawnResource);
        }
        #endregion
    }
}
