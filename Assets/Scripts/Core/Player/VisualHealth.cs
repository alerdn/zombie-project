using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualHealth : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Image _healthImage;

    private void Start()
    {
        _health.OnHit += ChangeHealthUI;
    }

    public void ChangeHealthUI(int health)
    {
        float healthValue = (float)health / 100f;
        Debug.Log(healthValue);
        _healthImage.fillAmount = healthValue;
    }

    private void OnDestroy()
    {
        _health.OnHit -= ChangeHealthUI;
    }
}
