using UnityEngine;

public class SnakeScript : Boss
{
    public static SnakeScript SnakeInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!waitComplete) return;
        //Debug.Log("‘Ò‹@o—ˆ‚Ä?");
    }
}
