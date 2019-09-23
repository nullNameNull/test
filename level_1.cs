using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class level_1 : MonoBehaviour
{
    public GameObject _panel;
    public Text message;
    [SerializeField]
    private string _mess;


    void Start()
    {
        _panel.SetActive(true);
        message.text=_mess;
    }
}
