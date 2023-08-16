using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hungry : MonoBehaviour
{
    [SerializeField] private Image _hungryBar;
    [SerializeField] private float _timeToHandleBar = 1000f; //milisegundos
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
            _currentHungry -= time / _timeToHandleBar;
            _hungryBar.fillAmount = _currentHungry;

            yield return new WaitForEndOfFrame();
        }

        _scoroutine = null;
    }

    private IEnumerator ReplenishHungry(int eatValue)
    {
        if (_scoroutine != null)
        {
            StopCoroutine(_scoroutine);
            _scoroutine = null;
        }

        float hungry = _currentHungry;
        float time = 0;

        _currentHungry += eatValue / 100f;

        while (hungry < _currentHungry)
        {
            time += Time.deltaTime;
            hungry += time / _timeToHandleBar;
            _hungryBar.fillAmount = hungry;

            yield return new WaitForEndOfFrame();
        }

        _rcoroutine = null;

        _scoroutine = StartCoroutine(SpendHungry());
    }

    public IEnumerator CallReplenishHungry(int eatValue)
    {
        if (_currentHungry >= _maxEat)
        {
            _currentHungry = _maxEat;
            yield break;
        }
        if (_rcoroutine != null) yield return new WaitUntil(() => _rcoroutine != null);
        _rcoroutine = StartCoroutine(ReplenishHungry(eatValue));
    }
}
