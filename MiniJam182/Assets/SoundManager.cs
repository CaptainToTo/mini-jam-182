using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioSource dayMusic;
    public AudioSource dayAmb;
    public AudioSource nightMusic;
    public AudioSource nightAmb;

    float nightMusicVol;
    float dayMusicVol;

    public GameObject flap;
    AudioSource[] flaps;

    public AudioSource getFlower;
    public float getFlowerNightPitch;

    public GameObject caughtSounds;
    public float caughtSoundsEase;
    public float toCalm;

    bool isNight = false;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nightMusicVol = nightMusic.volume;
        dayMusicVol = dayMusic.volume;
        dayMusic.Play();
        dayAmb.Play();
        nightMusic.Stop();

        flaps = flap.GetComponents<AudioSource>();
    }

    public void GetFlower()
    {
        if (isNight)
            getFlower.pitch = getFlowerNightPitch;
        getFlower.Play();
    }

    public void Flap()
    {
        flaps[Random.Range(0, flaps.Length)].Play();
    }

    public void ToNight(float dur)
    {
        nightMusic.volume = 0;
        nightAmb.volume = 0;
        isNight = true;
        nightMusic.Play();
        nightAmb.Play();
        frogInSight.Play();
        StartCoroutine(ToNightTween(dur));
    }

    IEnumerator ToNightTween(float dur)
    {
        float timer = 0;
        float start = dayMusic.volume;
        while (timer < dur)
        {
            timer += Time.deltaTime;
            float t = timer / dur;
            dayMusic.volume = Mathf.Lerp(start, 0f, t);
            dayAmb.volume = Mathf.Lerp(start, 0, t);
            nightMusic.volume = Mathf.Lerp(0, nightMusicVol, t);
            nightAmb.volume = Mathf.Lerp(0, nightMusicVol, t);
            yield return null;
        }
        dayMusic.Stop();
        dayAmb.Stop();
    }

    public void ToDay(float dur)
    {
        isNight = false;
        dayMusic.volume = 0;
        dayAmb.volume = 0;
        dayMusic.Play();
        dayAmb.Play();
        StartCoroutine(ToDayTween(dur));
    }

    IEnumerator ToDayTween(float dur)
    {
        float timer = 0;
        float start = nightMusic.volume;
        float frogStart = frogInSight.volume;
        while (timer < dur)
        {
            timer += Time.deltaTime;
            float t = timer / dur;
            nightMusic.volume = Mathf.Lerp(start, 0f, t);
            nightAmb.volume = Mathf.Lerp(start, 0, t);
            frogInSight.volume = Mathf.Lerp(frogStart, 0, t);
            dayMusic.volume = Mathf.Lerp(0, dayMusicVol, t);
            dayAmb.volume = Mathf.Lerp(0, dayMusicVol, t);
            yield return null;
        }
        nightMusic.Stop();
        nightAmb.Stop();
        frogInSight.Stop();
    }

    public AudioSource tongueSound;

    public void Tongue()
    {
        tongueSound.Play();
    }

    public AudioSource water;

    public void Water()
    {
        water.Play();
    }

    Coroutine caughtSoundsTween;
    public void Caught()
    {
        if (caughtSoundsTween != null)
            StopCoroutine(caughtSoundsTween);
        caughtSoundsTween = StartCoroutine(CaughtTween());
    }

    IEnumerator CaughtTween()
    {
        var sounds = caughtSounds.GetComponents<AudioSource>();
        foreach (var s in sounds)
        {
            s.Play();
            s.volume = 0;
        }
        float timer = 0;
        while (timer < caughtSoundsEase)
        {
            timer += Time.deltaTime;
            float v = Mathf.Lerp(0, 1, timer / caughtSoundsEase);
            foreach (var s in sounds)
                s.volume = v;
            yield return null;
        }
    }

    public void Release()
    {
        if (caughtSoundsTween != null)
            StopCoroutine(caughtSoundsTween);
        caughtSoundsTween = StartCoroutine(ReleaseTween());
    }

    IEnumerator ReleaseTween()
    {
        var sounds = caughtSounds.GetComponents<AudioSource>();
        sounds[1].Stop();

        float timer = 0;
        while (timer < toCalm)
        {
            timer += Time.deltaTime;
            float v = Mathf.Lerp(1, 0, timer / toCalm);
            sounds[0].volume = v;
            yield return null;
        }
        foreach (var s in sounds)
        {
            s.Stop();
            s.volume = 0;
        }
    }

    public AudioSource eaten;

    public void Eaten()
    {
        if (caughtSoundsTween != null)
            StopCoroutine(caughtSoundsTween);
        caughtSounds.SetActive(false);
        nightMusic.Stop();
        dayMusic.Stop();
        eaten.Play();
    }

    public AudioSource frogInSight;
    public float silence;
    public float max;

    public Frog[] frogs;

    void Update()
    {
        if (!isNight) return;

        float minDist = silence;
        foreach (var f in frogs)
        {
            float dist = f.DistToPlayer();
            if (dist < minDist)
                minDist = dist;
        }
        frogInSight.volume = Mathf.Lerp(1, 0, (minDist - max) / (silence - max));
    }
}
