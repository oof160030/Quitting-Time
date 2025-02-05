using UnityEngine;
using System.Collections;

public class Spawner_SCR : MonoBehaviour
{
    // Items that spawn
    private GameObject E1, E2, E3;
    private int E1Num, E2Num, E3Num;
    private float spawnRate;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Spawnwork(int numToSpawn)
    {
        int X = numToSpawn;

        while(X > 0)
        {
            //Spawn random enemy from selection
            spawnRandom();
            X--;
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void StartSpawning(GameObject Enemy, int N, float rate)
    {
        E1 = Enemy; E2 = null; E3 = null;
        E1Num = N; E2Num = 0; E3Num = 0;
        spawnRate = rate;
        StartCoroutine(Spawnwork(E1Num + E2Num + E3Num));
    }

    public void StartSpawning(GameObject Enemy, int N, GameObject Enemy2, int N2, float rate)
    {
        E1 = Enemy; E2 = Enemy2; E3 = null;
        E1Num = N; E2Num = N2; E3Num = 0;
        spawnRate = rate;
        StartCoroutine(Spawnwork(E1Num + E2Num + E3Num));
    }

    public void StartSpawning(GameObject Enemy, int N, GameObject Enemy2, int N2, GameObject Enemy3, int N3, float rate)
    {
        E1 = Enemy; E2 = Enemy2; E3 = Enemy3;
        E1Num = N; E2Num = N2; E3Num = N3;
        spawnRate = rate;
        StartCoroutine(Spawnwork(E1Num + E2Num + E3Num));
    }

    private void spawnRandom()
    {
        //Add up remaining enemies to spwan of each type
        int C = E1Num + E2Num + E3Num;

        //Generate a random integer based on the number
        int R = Random.Range(0, C) + 1;

        //Subtract E1 Num from the total
        if(R > 0 && E1Num != 0)
        {
            R -= E1Num;
            if(R <= 0)
            {
                //spawn the enemy
                Instantiate(E1, transform.position, Quaternion.identity);
                E1Num--;
            }
        }

        if (R > 0 && E2Num != 0)
        {
            R -= E2Num;
            if (R <= 0)
            {
                //spawn the enemy
                Instantiate(E2, transform.position, Quaternion.identity);
                E2Num--;
            }
        }

        if (R > 0 && E3Num != 0)
        {
            R -= E3Num;
            if (R <= 0)
            {
                //spawn the enemy
                Instantiate(E3, transform.position, Quaternion.identity);
                E3Num--;
            }
        }
    }

    public void AddSpawning(GameObject Enemy, int N, int spawnSlot)
    {
        if (spawnSlot == 2)
        {
            E2 = Enemy; E2Num = N;
        }
        else
        {
            E3 = Enemy; E3Num = N;
        }
    }
}
