using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    Transform P;
    private MGR SMGR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        P = GameObject.FindGameObjectWithTag("Player").transform;
        SMGR = MGR.SMGR;
        SMGR.Cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
            transform.position = new Vector3(P.position.x, P.position.y, -10f);
    }
}
