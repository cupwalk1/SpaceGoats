using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [FormerlySerializedAs("magCoefficient")] [SerializeField] private float magnitude = 1f;
    [SerializeField] private float shakeDuration = 0.2f;
    void Start()
    {
        transform.position = player.transform.position + offset;
        player.GetComponent<PlayerManager>().OnTakeDamage.AddListener(Shake);
        GetComponent<AudioSource>().volume = FindFirstObjectByType<SoundManager>().GetComponent<AudioSource>().volume;
    }
    
    void FixedUpdate()
    {
        if (!player.GetComponent<PlayerManager>().ShouldMoveCamera) return;
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
    
    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Vector3 desiredPosition = player.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x + x, smoothedPosition.y + y, smoothedPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
