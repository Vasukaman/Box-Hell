using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomTImePanel : MonoBehaviour
{
    [SerializeField] private RoomStateController roomController;
    [SerializeField] private TMP_Text timeText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int time = (int)(roomController.settings.preparationTime - roomController.CurrentTime);
        if (time>0)
        timeText.text = time.ToString();
        else
        timeText.text = "0";
    }
}
