using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAnimController : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private Tool tool;
    [SerializeField]  ProceduralWeaponAnimator procAnim;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {

        if (tool == null)
            tool = GetComponent<Tool>(); // Get the Tool component on this GameObject

        tool.OnToolUsed += AxeSwing;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AxeSwing()
    {
        procAnim.StartSwing();

     //   anim.Stop();

        //  anim.Play();

    }
    private void OnDestroy()
    {
        tool.OnToolUsed -= AxeSwing;
    }
}
