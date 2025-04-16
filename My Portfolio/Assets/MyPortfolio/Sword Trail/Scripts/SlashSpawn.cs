using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashSpawn : MonoBehaviour
{
    public List<Slash> slashes;
    public float initialDelay = 2f; // Delay before starting the first attack sequence
    private bool attacking;
    
    // Start is called before the first frame update
    void Start()
    {
        DisableSlashes();
        StartCoroutine(StartSlashSequence());
    }

    // Coroutine to handle the initial delay and then continuously start the slash attack sequence
    IEnumerator StartSlashSequence()
    {
        yield return new WaitForSeconds(initialDelay);
        
        while (true)
        {
            if (!attacking) 
            {
                attacking = true;
                yield return StartCoroutine(SlashAttack());
            }

            // Optional: Add a delay between consecutive attack sequences
            yield return new WaitForSeconds(1);
        }
    }

    // Coroutine to handle the slash attack sequence
    IEnumerator SlashAttack()
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }

        yield return new WaitForSeconds(1);
        DisableSlashes();
        attacking = false;
    }

    // Method to disable all slash objects
    void DisableSlashes()
    {
        for (int i = 0; i < slashes.Count; i++)
        {
            slashes[i].slashObj.SetActive(false);
        }
    }
}

[System.Serializable]
public class Slash
{
    public GameObject slashObj;
    public float delay;
}
