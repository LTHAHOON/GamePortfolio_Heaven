using UnityEngine;
using UnityEngine.UI;

public class FramePerSecond : MonoBehaviour
{
    [SerializeField]
    private float _updateInterval = 0.1f;

    private float accum = 0.0f;
    private int frames = 0;
    private float timeLeft;

    [SerializeField]
    private Text textDisplay;
    void Start()
    {
        timeLeft = _updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if(timeLeft <= 0.0f)
        {
            textDisplay.text = string.Concat("FPS: ", (accum / frames).ToString("###"));
            timeLeft = _updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    private void OnValidate()
    {
        textDisplay.text = "FPS: N/A";       
    }
}
