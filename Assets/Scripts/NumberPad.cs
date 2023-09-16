using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberPad : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _codeText;
    [SerializeField]
    private Transform _cardSpawn;
    [SerializeField]
    private GameObject _cardPreFab;
    [SerializeField]
    private Image _screen;

    private const string _code = "1234";
    private string _currentCode = string.Empty;
    private bool _isShowingAlert = false;
    private bool _isOn = true;

    public void AddDigit(string digit)
    {
        if (!_isShowingAlert && _isOn)
        {
            VerifyCode(digit);
        }
    }

    private void VerifyCode(string digit)
    {
        _currentCode += digit;

        if (_currentCode == _code)
        {
            UpdateCodeText("Code Valid", Color.green);
            Instantiate(_cardPreFab, _cardSpawn.position, _cardPreFab.transform.rotation);
            StartCoroutine(ClearCodeCourotine(turnOf: true));
        }
        else if (_currentCode.Length == 4 && _codeText.text != _code)
        {
            UpdateCodeText("Invalid Code", Color.red);
            StartCoroutine(ClearCodeCourotine(turnOf: false));
        }
        else
        {
            var replacedString = new string('*', _currentCode.Length);
            UpdateCodeText(replacedString, Color.black);
        }
    }

    private IEnumerator ClearCodeCourotine(bool turnOf)
    {
        _isShowingAlert = true;
        yield return new WaitForSeconds(2);
        ClearCode();
        _isShowingAlert = false;
        if (turnOf)
        {
            TurnOff();
        }
    }

    private void UpdateCodeText(string text, Color color)
    {
        _codeText.text = text;
        _codeText.color = color;
    }

    private void ClearCode()
    {
        UpdateCodeText(string.Empty, Color.black);
        _currentCode = string.Empty;
    }

    private void TurnOff()
    {
        _isOn = false;
        _screen.color = Color.black;
    }
}
