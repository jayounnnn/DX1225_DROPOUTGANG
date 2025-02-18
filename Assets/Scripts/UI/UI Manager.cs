using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;

    [SerializeField] private GameObject UnequipSwordUI;
    [SerializeField] private GameObject EquipSwordUI;

    [SerializeField] private GameObject SwordFinishUI;

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void EquipUI()
    {
        UnequipSwordUI.SetActive(false);
        EquipSwordUI.SetActive(true);
    }

    public void UnEquipUI()
    {
        UnequipSwordUI.SetActive(true);
        EquipSwordUI.SetActive(false);
    }

    public void EnableFinisherUI()
    {
        SwordFinishUI.SetActive(true);
        StartCoroutine(DisableFinisherAfterTime(2f));
    }
    private IEnumerator DisableFinisherAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwordFinishUI.SetActive(false);
    }

    public void DisableFinisherUI()
    {
        SwordFinishUI.SetActive(false);
    }
}