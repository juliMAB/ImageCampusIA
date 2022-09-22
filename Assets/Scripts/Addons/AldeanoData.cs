using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AldeanoData : MonoBehaviour
{
    private string[] names = { "Kapena Kapono", "Mele Mikala", "Apikalia Lani", "Kanani Iakopa", "Ekewaka Noa" };
    [SerializeField] TMPro.TextMeshProUGUI tag;
    
    public void Init(int id)
    {
        gameObject.name = names[0] +" "+ id.ToString();
        tag.text = names[0] + " " + id.ToString();
    }
}
