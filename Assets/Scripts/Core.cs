using UnityEngine;

public class Core : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            UIManager.Instance.SubtractTime(5);
        }
    }

}