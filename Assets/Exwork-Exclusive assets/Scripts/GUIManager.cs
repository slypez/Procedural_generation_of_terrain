using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI algorithmText;
    [SerializeField] private HeightMapSettings heightMapSettings;

    private void Update()
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        algorithmText.text = "Current algorithm: " + heightMapSettings.noiseSettings.noiseAlgorithm.ToString();
    }
}
