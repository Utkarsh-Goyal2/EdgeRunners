using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
public class homemanager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject defaultPanel;
    public GameObject inventoryPanel;
    public GameObject levelsPanel;

    public void load_default()
    {
        defaultPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        levelsPanel.SetActive(false);
    }
    public void load_inventory()
    {
        defaultPanel.SetActive(false);
        inventoryPanel.SetActive(true);
        levelsPanel.SetActive(false);
    }
    public void load_levels()
    {
        defaultPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

}
