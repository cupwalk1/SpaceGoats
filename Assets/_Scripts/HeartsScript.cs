using System.Collections;
using UnityEngine;

public class HeartsScript : MonoBehaviour
{
    
    public GoatStats goatStats;
    public PlayerManager playerManager;
    public GameObject heartPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < goatStats.maxGoatHealth; i++)
        {
            Instantiate(heartPrefab, transform);
        }
        playerManager.OnTakeDamage.AddListener(UpdateHearts);
    }

    private void UpdateHearts()
    {
        StartCoroutine(RemoveHeart());
    }

    private IEnumerator RemoveHeart()
    {
        while (transform.childCount > playerManager.Health)
        {
            Debug.Log($"{playerManager.Health}");
            Destroy(transform.GetChild(0).gameObject);
            yield return new WaitForEndOfFrame();
        }
    }
}
