using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float fps;
    public TMPro.TextMeshProUGUI FPSCountertext;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetFPS", 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSCountertext.text = "FPS: " + fps.ToString();
    }
}
