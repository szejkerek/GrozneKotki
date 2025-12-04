using DG.Tweening;
using UnityEngine;

public class Core : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            GameplayManager.Instance.TimeBar.SubtractTimeUI(2, byEnemy: true);
            other.gameObject.GetComponent<Enemy>().Kill();
        }
    }

}