using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public void playButtonSound()
    {
        FindObjectOfType<AudioManager>().Play("Menus");
    }
}
