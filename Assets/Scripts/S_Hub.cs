using UnityEngine;

public class S_Hub : MonoBehaviour
{
    public Spawner_SCR[] X;

    private void Start()
    {
        MGR.SMGR.ReceiveSpanwers(X);
    }

    public Spawner_SCR[] ReturnSpawners()
    {
        return X;
    }
}
