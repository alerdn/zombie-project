using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Thirst : MonoBehaviour
{
    [SerializeField] private Image _thirstBar;
    [field: SerializeField] public float TimeToHandleBar = 1000f; //milisegundos
    [SerializeField] private float _thirstDamage = .1f;
    private float _currentThirst;
    private float _maxDrink = 1f;
    private Coroutine _scoroutine;
    private Coroutine _rcoroutine;

    private float _defaultTimeToHandleBar = 1000f;

    private PlayerUIReferences _playerUIReferences;

    private void Start()
    {
        _currentThirst = _maxDrink;
        _thirstBar.fillAmount = _currentThirst;

        _scoroutine = StartCoroutine(SpendThirst());

        _defaultTimeToHandleBar = TimeToHandleBar;

        _playerUIReferences = GetComponentInParent<PlayerUIReferences>();
    }

    private IEnumerator SpendThirst()
    {
        if (TimeToHandleBar != _defaultTimeToHandleBar) TimeToHandleBar = _defaultTimeToHandleBar; 

        if (_rcoroutine != null)
        {
            StopCoroutine(_rcoroutine);
            _rcoroutine = null;
        }

        float time = 0;
        float startHitValue = _maxDrink / 2;

        while (_currentThirst > 0f)
        {
            if (_currentThirst < startHitValue)
            {
                _playerUIReferences.VisualHealth.Health.TakeDamage(_thirstDamage);
            }

            time = Time.deltaTime;
            _currentThirst -= time / TimeToHandleBar;
            _thirstBar.fillAmount = _currentThirst;

            yield return new WaitForEndOfFrame();
        }

        _scoroutine = null;
    }

    private IEnumerator ReplenishThirst(int eatValue)
    {
        if (_scoroutine != null)
        {
            StopCoroutine(_scoroutine);
            _scoroutine = null;
        }

        float thirst = _currentThirst;
        float time = 0;

        _currentThirst += eatValue / 100f;

        while (thirst < _currentThirst)
        {
            time += Time.deltaTime;
            thirst += time / TimeToHandleBar;
            _thirstBar.fillAmount = thirst;

            yield return new WaitForEndOfFrame();
        }

        _rcoroutine = null;

        _scoroutine = StartCoroutine(SpendThirst());
    }

    public IEnumerator CallReplenishThirst(int eatValue)
    {
        if (_currentThirst >= _maxDrink)
        {
            _currentThirst = _maxDrink;
            yield break;
        }
        if (_rcoroutine != null) yield return new WaitUntil(() => _rcoroutine != null);
        _rcoroutine = StartCoroutine(ReplenishThirst(eatValue));
    }
}
