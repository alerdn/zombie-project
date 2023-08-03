using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private Button _connectButton;
    [SerializeField]
    private Vector2 _nameLengthRule = new Vector2(1, 12);

    private void Start()
    {
        // Se estamos no servidor, ou seja, sem image gráfica
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        _nameField.text = PlayerPrefs.GetString(Constants.PlayerNameKey, string.Empty);
        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        _connectButton.interactable =
            _nameField.text.Length >= _nameLengthRule.x &&
            _nameField.text.Length <= _nameLengthRule.y;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(Constants.PlayerNameKey, _nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
