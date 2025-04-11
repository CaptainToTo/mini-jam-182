using UnityEngine;

public class InputTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Debug.Log("hello world");
    }
}
