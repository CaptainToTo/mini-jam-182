using UnityEngine;
using UnityEngine.Events;

public class Flower : MonoBehaviour
{
    public UnityEvent OnPollinated;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            OnPollinated.Invoke();
            gameObject.SetActive(false);
        }
    }
}
