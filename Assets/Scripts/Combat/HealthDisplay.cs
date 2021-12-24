using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class HealthDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private GameObject healthBarParent;

    public event Action OnPointerEntered;
    public event Action OnPointerExited;


    private void Awake() 
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy() 
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //healthBarParent.SetActive(true);
        OnPointerEntered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExited?.Invoke();
        //healthBarParent.SetActive(false);
    }
    public void DisplayOrNot(bool status) => healthBarParent?.SetActive(status);
}
