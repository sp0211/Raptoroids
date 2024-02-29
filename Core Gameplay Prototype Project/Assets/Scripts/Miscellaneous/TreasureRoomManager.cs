using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreasureRoomManager : MonoBehaviour
{

    private static TreasureRoomManager instance;
    public static TreasureRoomManager Instance { get { return instance; } }
    public TMP_Text gemText;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        gemText.text = "Gems: 0";
    }


    public void UpdateDisplay(int currentGems){
        gemText.text = $"Gems: {currentGems}";
    }
}
