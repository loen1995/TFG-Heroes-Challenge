using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeForm : MonoBehaviour
{

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject loginPanel;
    [Tooltip("The Ui Panel to register the user")]
    public GameObject registerPanel;

    public void changeToRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        FindObjectOfType<AudioManager>().Play("Menus");
    }

    public void changeToLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        FindObjectOfType<AudioManager>().Play("Menus");
    }


}
