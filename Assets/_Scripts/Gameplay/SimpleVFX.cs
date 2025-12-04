using UnityEngine;

public class SimpleVFX : MonoBehaviour
{
    public GameObject VFX;
    GameObject instance;

    void Start()
    {
        instance = Instantiate(VFX, transform);
    }

    private void OnDestroy()
    {
        Destroy(instance);
    }
}
