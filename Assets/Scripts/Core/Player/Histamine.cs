using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Histamine : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Image _histamineImage;
    private int _maxHistamine = 1;
    private float _currentHistamine;
    private float _autoHistamineCount = .1f;
    private bool _isRunning;

    private void Start()
    {
        _inputReader.RunEvent += HandleRun;

        _currentHistamine = _maxHistamine;
        _histamineImage.fillAmount = _currentHistamine;
    }

    private IEnumerator HandleHistamine()
    {
        yield return new WaitForSeconds(5f);
        while (_isRunning)
        {
            if (_currentHistamine == 0) yield break;

            _currentHistamine -= _autoHistamineCount;
            _histamineImage.fillAmount = _currentHistamine;
            yield return new WaitForSeconds(1f);
        }

        if (_currentHistamine >= _maxHistamine) yield break;
        while (!_isRunning && _currentHistamine <= _maxHistamine)
        {
            _currentHistamine += _autoHistamineCount;
            _histamineImage.fillAmount = _currentHistamine;
            yield return new WaitForSeconds(1f);
        }
    }

    private void HandleRun(bool isRunning)
    {
        _isRunning = isRunning;
        StartCoroutine(HandleHistamine());
    }
}
