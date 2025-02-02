using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AxeSwingScript : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private Tool tool;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {

        if (tool == null)
            tool = GetComponent<Tool>(); // Get the Tool component on this GameObject

        tool.OnToolUsed += AxeSwing;
        Debug.Log($"Axe xonnected");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AxeSwing()
    {
        Debug.Log($"Axe swing");
        anim.Play();

    }
}
