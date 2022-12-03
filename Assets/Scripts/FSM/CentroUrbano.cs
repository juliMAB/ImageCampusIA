using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM
{

    //public class CentroUrbano : MonoBehaviour
    //{
    //    [SerializeField] private GameObject m_aldeano;
    //    [SerializeField] private List<Mine> m_minitas;
    //    [SerializeField] private GameObject m_pfMina;
    //
    //    private List<Aldeano> aldeanos = new List<Aldeano>();
    //
    //    [SerializeField] private int maxMinitas = 5;
    //
    //    [SerializeField] private Rect m_Limites;
    //
    //    [SerializeField] private float m_gold;
    //
    //    public float Gold { get => m_gold; set => m_gold = value; }
    //
    //    public void SpawnAldeano()
    //    {
    //        GameObject aldeanito = Instantiate(m_aldeano, transform.position, Quaternion.identity, transform);
    //        Aldeano aldeanoCs = aldeanito.GetComponent<Aldeano>();
    //        aldeanos.Add(aldeanoCs);
    //        aldeanoCs.Init(this);
    //    }
    //    public Mine GetAnyMina()
    //    {
    //        if (m_minitas.Count > 0)
    //            return m_minitas[Random.Range(0, m_minitas.Count)];
    //        return null;
    //    }
    //
    //    public void CreateMine()
    //    {
    //        if (maxMinitas == m_minitas.Count)
    //            return;
    //        Vector2Int randPos = Vector2Int.zero;
    //        int aux = 0;
    //        do
    //        {
    //            aux++;
    //            if (aux > 100)
    //            {
    //                Debug.Log("maximo iteraciones");
    //                continue;
    //            }
    //            randPos = new Vector2Int(Random.Range((int)m_Limites.xMin, (int)m_Limites.xMax + 1), Random.Range((int)m_Limites.yMin, (int)m_Limites.yMax));
    //        } while (m_minitas.Any(p => !ComparePositions(randPos, p.transform.position)) && ComparePositions(randPos, transform.position, 2.0f));
    //        Mine mine = Instantiate(m_pfMina, new Vector3(randPos.x, randPos.y), Quaternion.identity, transform).GetComponent<Mine>();
    //        mine.OnEmpty += DeleteMineReference;
    //        m_minitas.Add(mine);
    //    }
    //    private bool ComparePositions(Vector2Int vec2I, Vector3 vec3)
    //    {
    //        return (vec3.x + 0.25f > vec2I.x && vec3.x - 0.25f < vec2I.x) &&
    //               (vec3.y + 0.25f > vec2I.y && vec3.y - 0.25f < vec2I.y);
    //
    //    }
    //    private bool ComparePositions(Vector2Int vec2I, Vector3 vec3, float distance)
    //    {
    //        return (vec3.x + distance > vec2I.x && vec3.x - distance < vec2I.x) &&
    //               (vec3.y + distance > vec2I.y && vec3.y - distance < vec2I.y);
    //
    //    }
    //
    //    private void DeleteMineReference(Mine mine)
    //    {
    //        m_minitas.Remove(mine);
    //        Destroy(mine.gameObject);
    //    }
    //    public void Descansar()
    //    {
    //        //for (int i = 0; i < aldeanos.Count; i++)
    //        //{
    //        //    aldeanos[i].SetFlag(Flags.OnTired);
    //        //}
    //    }
    //    public void Alerta()
    //    {
    //        //for (int i = 0; i < aldeanos.Count; i++)
    //        //{
    //        //    aldeanos[i].SetFlag(Flags.OnAlert);
    //        //}
    //    }
    //
    //}

}