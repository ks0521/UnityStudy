using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InventoryPresenter invenPresenter;

    public void InteractionInven()
    {
        bool isOpen = invenPresenter.gameObject.activeSelf;
        invenPresenter.gameObject.SetActive(!isOpen);
        invenPresenter.RefreshInventoryUI();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            InteractionInven();
        }
    }
}
