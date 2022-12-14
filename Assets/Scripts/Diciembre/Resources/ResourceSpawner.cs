using System.Collections.Generic;
using System.Linq; //para usar Any en list.
using UnityEditor;
using UnityEngine;

namespace Diciembre
{
    public class ResourceSpawner : MonoBehaviour
    {
        #region EXPOSED_FIELD
        [SerializeField] private GameObject resourcePrefab;
        [SerializeField] private CentroUrbano townCenter;
        [SerializeField] private Transform resourceConteiner;

        [SerializeField] private Vector3 SpawnPos;

        [SerializeField] private int maxResources;
        #endregion

        #region PRIVATE_FIELDS
        private static List<Resource> resources = new List<Resource>();
        #endregion
        public static List<Resource> Resources { get => resources; }

        #region PUBLIC_METHODS
        public void SpawnResourceRandom()
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

            SpawnResource(new Vector3(randPos.x, randPos.y));
        }

        public void SpawnResourceAtPosition()
        {
            if (!(maxResources >= resources.Count))
                return;

            Vector2Int randPos = new Vector2Int((int)SpawnPos.x,(int)SpawnPos.y);
            int index = NodeUtils.PositionToIndex(randPos);
            if (!CanSpawn(index, randPos))
                return;

            SpawnResource(new Vector3(randPos.x, randPos.y));
        }
        public static Resource GetAnyResource()
        {
            if (resources.Count == 0)
                return null;
            return resources[Random.Range(0, resources.Count)];
        }

        public static Resource GetCloserResource(Vector3 agentPos)
        {
            if (resources.Count == 0)
                return null;
            return VoronoiController.GetMineCloser(agentPos);

        }
        #endregion

        #region PRIVATE_METHODS

        private void SpawnResource(Vector3 pos)
        {
            GameObject resourceGO = Instantiate(resourcePrefab, pos, Quaternion.identity, resourceConteiner);
            Resource resource = resourceGO.GetComponent<Resource>();
            resource.OnEmpty += DeleteResourceReference;
            resources.Add(resource);
            VoronoiController.SetVoronoi(resources);
        }
        private void DeleteResourceReference(Resource resource)
        {
            resources.Remove(resource);
            Destroy(resource.gameObject);
            VoronoiController.SetVoronoi(resources);
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
            if (invalidPosition(randPos))
                return false;
            return true;
        }
        private bool invalidPosition(Vector2Int randpos)
        {
            if (randpos.x <= 0 || randpos.y <= 0 || randpos.x == Main.MapSize.x||randpos.y ==Main.MapSize.y)
                return true;
            return false;
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
    }

    [CustomEditor(typeof(ResourceSpawner))]
    public class ResourceSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ResourceSpawner level = (ResourceSpawner)target;

                if (GUILayout.Button("SpawnAt"))
                    level.SpawnResourceAtPosition();
        }
    }
}
