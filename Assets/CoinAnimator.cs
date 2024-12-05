using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    public Animator coin;

    void Update()
    {
        if (GameManager.Instance != null && coin != null)
        {
            coin.SetBool("Wonder", GameManager.Instance.currentWonderEffect != GameManager.WonderEffect.None);
        }
        else if (coin != null)
        {
            coin.SetBool("Wonder", true);
        }
    }
}
