using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTutorialEndController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }
    }
}
