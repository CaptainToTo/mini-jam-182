using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Image sprite;
    public float wait;
    public float fade;

    bool done = false;

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.P)) && !done)
        {
            done = true;
            StartCoroutine(Close());
        }
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(wait);

        float timer = 0;
        Color target = new Color(1, 1, 1, 0);
        while (timer < fade)
        {
            timer += Time.deltaTime;
            float t = timer / fade;
            sprite.color = Color.Lerp(Color.white, target, t);
            yield return null;
        }
        sprite.gameObject.SetActive(false);
    }
}
