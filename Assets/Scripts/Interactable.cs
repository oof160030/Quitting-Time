using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private bool touching;

    public UnityEvent Interacted;
    private MGR SMGR;

    private void Start()
    {
        SMGR = MGR.SMGR;

        SMGR.AddInvocation("TO_TEMPLE", Interacted);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            touching = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            touching = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && touching)
            Interacted.Invoke();
    }
}
