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
        //‘Ò‹@ˆ—Š®—¹‚ð‘Ò‚Â
        if (!waitComplete) return;

        //HP‚ÅðŒ•ªŠò
        if (ratioHP >= 50)
        {
            //HP50%ˆÈã‚Ìˆ—
        }
        else
        {
            //HP50%ˆÈ‰º‚Ìˆ—
        }


    }
}
