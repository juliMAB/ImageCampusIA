using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AldeanoData : MonoBehaviour
{
    private string[] names = { "Kapena Kapono", "Mele Mikala", "Apikalia Lani", "Kanani Iakopa", "Ekewaka Noa" };
    [SerializeField] TMPro.TextMeshProUGUI NombreBichito;
    
    public void Init(int id)
    {
        gameObject.name = names[0] +" "+ id.ToString();
        NombreBichito.text = names[0] + " " + id.ToString();
    }
}
