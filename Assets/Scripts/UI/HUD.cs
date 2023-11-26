using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] QuickSlotsView quickSlotsView;
    [SerializeField] EquipmentView equipmentView;

    Player player;

    public void Init(Player owner)
    {
        player = owner;

        quickSlotsView.Init(owner);
        equipmentView.Init(owner);

        equipmentView.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            var state = !equipmentView.gameObject.activeSelf;
            equipmentView.gameObject.SetActive(state);
            player.GetComponent<StarterAssetsInputs>().SetCursorState(!state);
        }
    }
}
