using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI algorithmText;
    private MapGenerator mapGenerator;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    private void Update()
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        algorithmText.text = "Current algorithm: " + mapGenerator.NoiseAlgorithm.ToString();
    }
}
