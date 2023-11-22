using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] QuickSlotsView quickSlotsView;

    public void Init(Player owner)
    {
        quickSlotsView.Init(owner);
    }
}
