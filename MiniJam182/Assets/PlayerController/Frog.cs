using System.Collections;
using UnityEngine;

public class Frog : MonoBehaviour
{

    public Butterfly player;

    public Transform[] points;

    public Transform curPoint;

    public float teleportDist;
    public float teleportCheckFreq;

    float checkTimer;

    public float eatDist;
    public float eatSpeed;
    public float pullSpeed;

    public Transform upperJaw;
    public Transform lowerJaw;
    public float jawTween;
    public float upperJawAngle;
    public float lowerJawAngle;

    float upperJawOrigin;
    float lowerJawOrigin;

    public float DistToPlayer()
    {
        var diff = transform.position - player.transform.position;
        Vector2 plane = new Vector2(diff.x, diff.y);
        return plane.magnitude;
    }

    public float DistToPlayer(Transform t)
    {
        var diff = t.position - player.transform.position;
        Vector2 plane = new Vector2(diff.x, diff.y);
        return plane.magnitude;
    }

    public Transform GetTeleportPoint()
    {
        Transform best = curPoint;
        float bestDist = DistToPlayer(best);
        foreach (var p in points)
        {
            if (p == best) continue;
            float dist = DistToPlayer(p);
            if (dist > teleportDist && dist < bestDist && p.childCount == 0)
            {
                best = p;
                bestDist = dist;
            }
        }
        return best;
    }

    public void TeleportTo(Transform t)
    {
        transform.position = t.position;
        transform.up = t.up;
        curPoint = t;
        transform.SetParent(curPoint);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkTimer = teleportCheckFreq;
        if (curPoint == null)
            curPoint = points[0];
        TeleportTo(curPoint);
        upperJawOrigin = upperJaw.localEulerAngles.x;
        lowerJawOrigin = lowerJaw.localEulerAngles.x;
    }

    bool isEating = false;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy == false) return;

        if (!isEating && checkTimer <= 0)
        {
            var dist = DistToPlayer();
            if (dist < eatDist)
            {
                isEating = true;
                StartCoroutine(Eat());
            }
            else if (dist > teleportDist)
            {
                var t = GetTeleportPoint();
                if (t != curPoint)
                    TeleportTo(t);
            }
            checkTimer = teleportCheckFreq;
        }
        else
        {
            checkTimer -= Time.deltaTime;
        }
    }

    public LineRenderer tongue;
    public Transform mouth;

    public float catchDist;

    bool escaped = false;

    IEnumerator Eat()
    {
        // open jaw and shoot tongue
        float jawTimer = 0;
        float tongueTimer = 0;
        escaped = false;

        tongue.gameObject.SetActive(true);
        tongue.SetPosition(0, mouth.position);
        tongue.SetPosition(1, mouth.position);

        Vector3 target = player.transform.position;
        var upperJawRot = Quaternion.Euler(new Vector3(upperJawAngle, upperJaw.localEulerAngles.y, upperJaw.localEulerAngles.z));
        var upperJawStart = Quaternion.Euler(new Vector3(upperJawOrigin, upperJaw.localEulerAngles.y, upperJaw.localEulerAngles.z));
        var lowerJawRot = Quaternion.Euler(new Vector3(lowerJawAngle, lowerJaw.localEulerAngles.y, lowerJaw.localEulerAngles.z));
        var lowerJawStart = Quaternion.Euler(new Vector3(lowerJawOrigin, lowerJaw.localEulerAngles.y, lowerJaw.localEulerAngles.z));

        while (jawTimer < jawTween)
        {
            jawTimer += Time.deltaTime;
            tongueTimer += Time.deltaTime;
            float t = jawTimer / jawTween;
            float j = tongueTimer / eatSpeed;
            lowerJaw.localRotation = Quaternion.Slerp(lowerJawStart, lowerJawRot, t);
            upperJaw.localRotation = Quaternion.Slerp(upperJawStart, upperJawRot, t);
            tongue.SetPosition(1, Vector3.Lerp(mouth.position, target, j));
            target = player.transform.position;
            yield return null;
        }

        while (tongueTimer < eatSpeed)
        {
            tongueTimer += Time.deltaTime;
            float j = tongueTimer / eatSpeed;
            Vector3 nextPos = Vector3.Lerp(mouth.position, target, j);
            tongue.SetPosition(1, nextPos);
            if ((player.transform.position - nextPos).magnitude < catchDist)
            {
                break;
            }
            yield return null;
        }

        bool isCaught = false;
        float returnSpeed = eatSpeed;
        Vector3 returnPos = mouth.position;
        if ((player.transform.position - target).magnitude < catchDist && !player.isCaptured)
        {
            returnPos = new Vector3(mouth.position.x, mouth.position.y, target.z);
            returnSpeed = pullSpeed;
            isCaught = true;
            player.Capture(this);
        }


        tongueTimer = 0;
        while (tongueTimer < returnSpeed)
        {
            tongueTimer += Time.deltaTime;
            float t = tongueTimer / returnSpeed;
            Vector3 nextPos = Vector3.Lerp(target, returnPos, t);
            tongue.SetPosition(1, nextPos);
            if (isCaught)
            {
                player.transform.position = nextPos;
            }
            if (escaped)
            {
                returnPos = mouth.position;
                returnSpeed = eatSpeed;
                isCaught = false;
                escaped = false;
            }
            yield return null;
        }
        tongue.gameObject.SetActive(false);

        if (isCaught)
        {
            Camera.main.GetComponent<FlowerManager>().Die(true);
        }

        jawTimer = 0;
        while (jawTimer < jawTween)
        {
            jawTimer += Time.deltaTime;
            float t = jawTimer / jawTween;
            lowerJaw.localRotation = Quaternion.Slerp(lowerJawRot, lowerJawStart, t);
            upperJaw.localRotation = Quaternion.Slerp(upperJawRot, upperJawStart, t);
            yield return null;
        }
        isEating = false;
        checkTimer = teleportCheckFreq;
    }

    public void Escaped()
    {
        escaped = true;
    }
}
