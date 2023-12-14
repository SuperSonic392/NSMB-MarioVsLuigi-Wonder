using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    public PlayerController.wonderBadge badge1;
    public PlayerController.wonderBadge badge2;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
