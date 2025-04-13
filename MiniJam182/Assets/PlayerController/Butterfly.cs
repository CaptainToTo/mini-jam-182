using System.Collections;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public Rigidbody Rb;

    public float rotForce;
    public float force;

    public float maxSpeed;
    public float maxRotSpeed;
    public float lift;

    void Start()
    {
        Rb.maxLinearVelocity = maxSpeed;
        Rb.maxAngularVelocity = maxRotSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rb.AddRelativeTorque(new Vector3(rotForce, 0, 0), ForceMode.Impulse);
            Rb.AddForce((transform.forward + (Vector3.up * lift)).normalized * force, ForceMode.Impulse);

            if (leftFlap != null)
                StopCoroutine(leftFlap);
            leftFlap = StartCoroutine(WingFlap(leftWing));

            SoundManager.instance.Flap();

            if (isCaptured)
            {
                captured--;
                if (captured <= 0)
                {
                    isCaptured = false;
                    captor.Escaped();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Rb.AddRelativeTorque(new Vector3(-rotForce, 0, 0), ForceMode.Impulse);
            Rb.AddForce((transform.forward + (Vector3.up * lift)).normalized * force, ForceMode.Impulse);

            if (rightFlap != null)
                StopCoroutine(rightFlap);
            rightFlap = StartCoroutine(WingFlap(rightWing));

            SoundManager.instance.Flap();

            if (isCaptured)
            {
                captured--;
                if (captured <= 0)
                {
                    isCaptured = false;
                    captor.Escaped();
                }
            }
        }
    }

    public Transform leftWing;
    public Transform rightWing;
    public AnimationCurve flapTween;
    public float maxRot;
    public float minRot;
    public float flapDur;

    Coroutine leftFlap;
    Coroutine rightFlap;

    IEnumerator WingFlap(Transform wing)
    {
        float timer = 0;
        while (timer < flapDur)
        {
            timer += Time.deltaTime;
            float t = timer / flapDur;
            wing.localEulerAngles = new Vector3(0, Mathf.Lerp(minRot, maxRot, flapTween.Evaluate(t)), 0);
            yield return null;
        }
        wing.localEulerAngles = Vector3.zero;
    }

    public float wallForce;
    public float minSpinout;
    public float maxSpinout;

    public void HitWall(Vector3 hitPos, Vector3 normal)
    {
        Rb.AddRelativeTorque(new Vector3(Random.Range(minSpinout, maxSpinout), 0, 0), ForceMode.Impulse);
        Rb.AddForce(normal * wallForce, ForceMode.Impulse);
    }

    public int capturedHealth;
    public float capturedMult;

    int timesCaptured = 0;

    int captured;

    public bool isCaptured;
    Frog captor;

    public void Capture(Frog f)
    {
        captured = capturedHealth + (int)(timesCaptured * capturedMult);
        timesCaptured++;
        isCaptured = true;
        captor = f;
    }
}
