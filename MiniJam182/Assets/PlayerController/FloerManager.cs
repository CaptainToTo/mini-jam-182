using TMPro;
using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    public Flower[] flowers;

    public TextMeshProUGUI total;
    public TextMeshProUGUI count;
    public int num;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        total.text = "/" + flowers.Length.ToString();

        foreach (var flower in flowers)
        {
            flower.OnPollinated.AddListener(OnPollinated);
        }

        count.text = "0";
    }

    public void OnPollinated()
    {
        num++;
        count.text = num.ToString();
    }
}
