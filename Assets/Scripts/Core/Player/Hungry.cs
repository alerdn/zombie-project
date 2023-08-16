using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hungry : MonoBehaviour
{
    [SerializeField] private Image _hungryBar;
    [SerializeField] private float _timeToReplenish = 1000f; //milisegundos
    [SerializeField] private float _hungryDamage = .1f;
    private float _currentHungry;
    private float _maxEat = 1f;
    private Coroutine _scoroutine;
    private Coroutine _rcoroutine;

    private PlayerUIReferences _playerUIReferences;

    private void Start()
    {
        _currentHungry = _maxEat;
        _hungryBar.fillAmount = _currentHungry;

        _scoroutine = StartCoroutine(SpendHungry());

        _playerUIReferences = GetComponentInParent<PlayerUIReferences>();
    }

    private IEnumerator SpendHungry()
    {
        if (_rcoroutine != null)
        {
            StopCoroutine(_rcoroutine);
            _rcoroutine = null;
        }

        float time = 0;
        float startHitValue = _maxEat / 2;

        while (_currentHungry > 0f)
        {
            if (_currentHungry < startHitValue)
            {
                _playerUIReferences.VisualHealth.Health.TakeDamage(_hungryDamage);
            }

            time = Time.deltaTime;
            _currentHungry -= time / _timeToReplenish;
            _hungryBar.fillAmount = _currentHungry;

            yield return new WaitForEndOfFrame();
        }

        _scoroutine = null;
    }

    public IEnumerator ReplenishHungry(int eatValue)
    {
        if (_scoroutine != null)
        {
            StopCoroutine(_scoroutine);
            _scoroutine = null;
        }

        float hungry = _currentHungry;
        float time = 0;

        if (_currentHungry >= _maxEat)
        {
            _currentHungry = _maxEat;
            yield break;
        }

        _currentHungry += eatValue / 100f;

        while (hungry < _currentHungry)
        {
            time += Time.deltaTime;
            hungry += time / _timeToReplenish;
            _hungryBar.fillAmount = hungry;

            yield return new WaitForEndOfFrame();
        }

        _rcoroutine = null;

        StartCoroutine(SpendHungry());
    }
}
