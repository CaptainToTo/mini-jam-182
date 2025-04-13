using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Camera.main.GetComponent<FlowerManager>().Die();
        }
    }

    
}
