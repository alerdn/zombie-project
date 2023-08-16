using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hungry : MonoBehaviour
{
    public event Action<float> OnHungry;

    [SerializeField] private Image _hungryBar;
    [SerializeField] private float _timeToHandleBar = 1000f; //milisegundos
    [SerializeField] private float _hungryDamage = .1f;
    public float CurrentHungry;
    private float _maxEat = 1f;
    private Coroutine _scoroutine;
    private Coroutine _rcoroutine;

    private void Start()
    {
        CurrentHungry = _maxEat;
        _hungryBar.fillAmount = CurrentHungry;

        _scoroutine = StartCoroutine(SpendHungry());
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

        while (CurrentHungry > 0f)
        {
            if (CurrentHungry < startHitValue)
            {
                OnHungry?.Invoke(_hungryDamage);
            }

            time = Time.deltaTime;
            CurrentHungry -= time / _timeToHandleBar;
            _hungryBar.fillAmount = CurrentHungry;

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

        float hungry = CurrentHungry;
        float time = 0;

        CurrentHungry += eatValue / 100f;

        while (hungry < CurrentHungry)
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
        if (CurrentHungry >= _maxEat)
        {
            CurrentHungry = _maxEat;
            yield break;
        }
        if (_rcoroutine != null) yield return new WaitUntil(() => _rcoroutine != null);
        _rcoroutine = StartCoroutine(ReplenishHungry(eatValue));
    }
}
