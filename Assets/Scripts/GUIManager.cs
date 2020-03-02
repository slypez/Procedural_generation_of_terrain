using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI algorithmText;
    private MapPreview mapPreview;

    private void Start()
    {
        mapPreview = FindObjectOfType<MapPreview>();
    }

    private void Update()
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        algorithmText.text = "Current algorithm: " + mapPreview.NoiseAlgorithm.ToString();
    }
}
