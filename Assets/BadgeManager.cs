using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeManager : MonoBehaviour //if you're planning on adding more badge slots, duplicate badge2 with ctrl + d, go to PlayerController, do the same, you'll need to update the EquipBadge and DoesHaveBadge functions to include the new slots. at the end you'll need to add it to the menu
{
    public PlayerController.WonderBadge badge1;
    public PlayerController.WonderBadge badge2;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        if(FindObjectOfType<BadgeManager>() != this)
        {
            Destroy(gameObject);
        }
    }
}
