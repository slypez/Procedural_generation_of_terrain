using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementControl : MonoBehaviour
{

    [SerializeField] private float displacementAmount;
    [SerializeField] private float speed;
    [SerializeField] private float displacementAmountMax;
    [SerializeField] private float displacementAmountMin;


    MeshRenderer meshRender;

    void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        displacementAmount = Mathf.Lerp(displacementAmount, displacementAmountMin, Time.deltaTime);
        meshRender.material.SetFloat("_Amount", displacementAmount);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            displacementAmount += speed * Time.deltaTime;
        }
        displacementAmount = Mathf.Clamp(displacementAmount, displacementAmountMin, displacementAmountMax);
    }
}
