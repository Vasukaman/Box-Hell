using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAnimController : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private Tool tool;
    [SerializeField]  ProceduralWeaponAnimator procAnim;
    [SerializeField] private bool useProcAnim;
    [SerializeField] private bool restartAnimOnClick;

    private void Awake()
    {
        tool.OnToolThrown += OnToolThrown;
        tool.OnToolEquipped += OnEquip;
    }

    void OnEquip()
    {
        if (useProcAnim)
            procAnim.OnEquip();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (tool == null)
            tool = GetComponent<Tool>(); // Get the Tool component on this GameObject

        tool.OnToolUsed += PlayAnimation;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAnimation()
    {
        if (useProcAnim)
        procAnim.StartSwing();

        else
        {
            if (restartAnimOnClick)
             anim.Stop();

             anim.Play();

        }


    }

    private void OnToolThrown()
    {
        Debug.Log("STOP ANIMATION");
        StopAnimation();
    }
    public void StopAnimation()
    {
        if (useProcAnim)
            procAnim.PauseAnimation();
        else
        {
                anim.Stop();

        }
    }
    private void OnDestroy()
    {
        tool.OnToolUsed -= PlayAnimation;
    }


}
