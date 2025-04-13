using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Flower : MonoBehaviour
{
    public UnityEvent OnPollinated;

    public float disappearDur;
    public float maxSize;
    public AnimationCurve tween;
    public GameObject particles;

    bool collected = false;

    void Start()
    {
        maxSize = transform.localScale.x;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !collected)
        {
            OnPollinated.Invoke();
            collected = true;
            SoundManager.instance.GetFlower();
            StartCoroutine(Tween());
        }
    }

    IEnumerator Tween()
    {
        float timer = 0;

        while (timer < disappearDur)
        {
            timer += Time.deltaTime;
            float t = timer / disappearDur;
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one * maxSize, tween.Evaluate(t));
            yield return null;
        }

        if (particles)
        {
            var p = Instantiate(particles);
            p.transform.position = transform.position;
        }
        gameObject.SetActive(false);
    }
}
