using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnGameStart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
