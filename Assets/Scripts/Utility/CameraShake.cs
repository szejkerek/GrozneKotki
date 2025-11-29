using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    Vector3 originalPos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [ContextMenu("Shake")]
    public void Test()
    {
        Instance.Shake(0.2f, 0.3f);
    }
    
    public void Shake(float duration, float strength)
    {
        StopAllCoroutines();
        originalPos = transform.localPosition;

        StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            Vector3 offset = Random.insideUnitSphere * strength;
            offset.z = 0f;

            transform.localPosition = originalPos + offset;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}