using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualHealth : MonoBehaviour
{
    [field: SerializeField] public Health Health { get; private set; }

    [SerializeField] private Image _healthImage;

    private void Start()
    {
        Health.OnHit += ChangeHealthUI;
    }

    public void ChangeHealthUI(float health)
    {
        float healthValue = health / 100f;
        _healthImage.fillAmount = healthValue;
    }

    private void OnDestroy()
    {
        Health.OnHit -= ChangeHealthUI;
    }
}
