using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GmPhatFinding : MonoBehaviour
{
    public GameObject aldenito;

    public Transform casa;

    public Transform casa2;


    public void SpawnAldeanito()
    {
        GameObject sp = Instantiate(aldenito, null);
        MineroPath mineroPath = sp.GetComponent<MineroPath>();
        //mineroPath.placeToGo = casa2;
        //mineroPath.placeToBack = casa;
    }
}
