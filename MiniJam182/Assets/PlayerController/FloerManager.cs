using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlowerManager : MonoBehaviour
{
    public Flower[] flowers;

    public TextMeshProUGUI total;
    public TextMeshProUGUI count;
    public int num;

    public Volume vol;
    public Vignette cone;
    public Light sun;

    public float time;

    public float sunIntense1;

    public float coneIntense;

    public Color nightColor;

    public Material nightTime;

    public Transform endSpot;

    public bool running = true;

    public Frog[] frogs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vol?.profile.TryGet(out cone);
        total.text = "/" + flowers.Length.ToString();

        foreach (var flower in flowers)
        {
            flower.OnPollinated.AddListener(OnPollinated);
        }

        count.text = "0";

        RenderSettings.skybox.SetFloat("_Blend", 1f);

        black.gameObject.SetActive(false);

        foreach (var f in frogs)
            f.gameObject.SetActive(false);
    }

    public void OnPollinated()
    {
        num++;
        count.text = num.ToString();

        if (testing || num == flowers.Length)
        {
            running = false;
            GetComponent<CameraFollow>().enabled = false;
            StopAllCoroutines();
            StartCoroutine(ToEnd());
        }
        else if (num == 1)
        {
            StopAllCoroutines();
            StartCoroutine(ToNight1());
        }
        else if (num == 2)
        {
            StopAllCoroutines();
            StartCoroutine(ToNight2());
        }
        else if (num == 3)
        {
            StopAllCoroutines();
            StartCoroutine(ToNight3());
        }
    }

    IEnumerator ToNight1()
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            float t = timer / time;
            sun.intensity = Mathf.Lerp(1f, sunIntense1, t);
            yield return null;
        }
    }

    IEnumerator ToNight2()
    {
        float timer = 0;
        float start = sun.intensity;
        while (timer < time)
        {
            timer += Time.deltaTime;
            float t = timer / time;
            sun.intensity = Mathf.Lerp(start, 0, t);
            RenderSettings.skybox.SetFloat("_Blend", Mathf.Lerp(1, 0.5f, t));
            yield return null;
        }
    }

    IEnumerator ToNight3()
    {
        SoundManager.instance.ToNight(time);

        float timer = 0;
        float start = RenderSettings.skybox.GetFloat("_Blend");
        while (timer < time)
        {
            timer += Time.deltaTime;
            float t = timer / time;
            cone.intensity.Override(Mathf.Lerp(0, coneIntense, t));
            RenderSettings.skybox.SetFloat("_Blend", Mathf.Lerp(start, 0, t));
            yield return null;
        }
        foreach (var f in frogs)
            f.gameObject.SetActive(true);
    }

    public void Die(bool immediate = false)
    {
        if (!running) return;
        running = false;
        GetComponent<CameraFollow>().enabled = false;
        if (immediate)
        {
            StartCoroutine(DieNow());
            return;
        }
        StartCoroutine(DieTween());
    }

    public Image black;
    public float wait;
    public float fade;

    public GameObject endScreen;

    public bool testing;

    IEnumerator DieNow()
    {
        black.gameObject.SetActive(true);
        black.color = Color.black;
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene("ButterflyScene");
    }

    IEnumerator DieTween()
    {
        yield return new WaitForSeconds(wait);

        float timer = 0;
        black.gameObject.SetActive(true);
        Color target = new Color(0, 0, 0, 0);
        while (timer < fade)
        {
            timer += Time.deltaTime;
            float t = timer / fade;
            black.color = Color.Lerp(target, Color.black, t);
            yield return null;
        }

        SceneManager.LoadScene("ButterflyScene");
    }

    IEnumerator ToEnd()
    {
        foreach (var f in frogs)
            f.enabled = false;
        yield return new WaitForSeconds(wait);

        SoundManager.instance.ToDay(fade);

        float timer = 0;
        black.gameObject.SetActive(true);
        Color target = new Color(0, 0, 0, 0);
        while (timer < fade)
        {
            timer += Time.deltaTime;
            float t = timer / fade;
            black.color = Color.Lerp(target, Color.black, t);
            yield return null;
        }

        RenderSettings.skybox.SetFloat("_Blend", 1f);
        cone.intensity.Override(0);
        sun.intensity = 1f;
        transform.position = endSpot.position;
        transform.forward = endSpot.forward;
        total.gameObject.SetActive(false);
        count.gameObject.SetActive(false);
        endScreen.SetActive(true);
        
        timer = 0;
        while (timer < fade)
        {
            timer += Time.deltaTime;
            float t = timer / fade;
            black.color = Color.Lerp(Color.black, target, t);
            yield return null;
        }
        black.gameObject.SetActive(false);
    }
}
