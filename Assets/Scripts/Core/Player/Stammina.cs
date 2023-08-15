using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stammina : MonoBehaviour
{
    public float CurrentStammina { get; private set; }

    public bool HasStammina => CurrentStammina > 0;

    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Image _StamminaImage;
    [SerializeField] private float _timeToReplenish = 500f;
    private int _maxStammina = 1;
    private Coroutine _rcoroutine;
    private Coroutine _scoroutine;


    private void Start()
    {
        CurrentStammina = _maxStammina;
        _StamminaImage.fillAmount = CurrentStammina;
    }

    private IEnumerator ReplenishStammina()
    {
        float time = 0;
        while (CurrentStammina < _maxStammina)
        {
            time += Time.deltaTime;
            CurrentStammina += time / _timeToReplenish;
            _StamminaImage.fillAmount = CurrentStammina;

            yield return new WaitForEndOfFrame();
        }

        _rcoroutine = null;
    }

    private IEnumerator SpendStammina()
    {
        float time = 0;
        while (CurrentStammina > 0)
        {
            time += Time.deltaTime;
            CurrentStammina -= time / _timeToReplenish;
            _StamminaImage.fillAmount = CurrentStammina;

            yield return new WaitForEndOfFrame();
        }

        _scoroutine = null;
    }

    public void HandleStammina(bool isRunning, bool hasShiftUp)
    {
        if (isRunning)
        {
            if (_scoroutine == null)
            {
                if (_rcoroutine != null)
                {
                    StopCoroutine(_rcoroutine);
                    _rcoroutine = null;
                }
                _scoroutine = StartCoroutine(SpendStammina());
            }
        }
        else if (hasShiftUp)
        {
            if (_rcoroutine == null)
            {
                if (_scoroutine != null)
                {
                    StopCoroutine(_scoroutine);
                    _scoroutine = null;
                }

                _rcoroutine = StartCoroutine(ReplenishStammina());
            }
        }

        return;
    }
}
