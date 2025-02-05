using UnityEngine;

public class MouseTracking : MonoBehaviour
{

    private MGR SMGR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SMGR = MGR.SMGR;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = SMGR.GetMousePos();
    }
}
