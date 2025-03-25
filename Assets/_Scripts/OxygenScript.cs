using UnityEngine;
using UnityEngine.UI;

public class OxygenScript : MonoBehaviour
{
    PlayerManager PM;
    Slider oxygenBar => GetComponent<Slider>();
    private ParticleSystem oxygenParticles => oxygenBar.handleRect.GetComponent<ParticleSystem>();

    [SerializeField]
    private float SecondsOfOxygen;
    float oxygenLevel
    {
        get{return oxygenBar.value;}
        set{oxygenBar.value = value;}
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        PM.OnPlayerDie.AddListener(delegate
        {
            oxygenLevel = 0;
            oxygenParticles.Stop();
        });
        GameManager.Instance.GameStart.AddListener(delegate
        {
            SecondsOfOxygen = PM.GoatStats.maxTimeOxygen;
            oxygenLevel = PM.GoatStats.maxTimeOxygen;
            oxygenParticles.Play();
        });
        oxygenParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (PM.IsGameInProgress && !PM.OnDoorThreshold)
        {
            oxygenLevel -= Time.deltaTime / PM.GoatStats.maxTimeOxygen;
            SecondsOfOxygen -= Time.deltaTime;
            if (oxygenBar.value <= 0)
            {
                PM.OnPlayerDie.Invoke();
            }
        }
    }
}
