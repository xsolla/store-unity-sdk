using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class ResetPasswordPage : Page, IResetPassword
{
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Button change_Btn;

    public Action OnSuccessfulResetPassword { get; set; }
    public Action<Error> OnUnsuccessfulResetPassword { get; set; }

    private void Awake()
    {
        change_Btn.onClick.AddListener(ResetPassword);
        email_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
    }
    
    void Start()
    {
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        change_Btn.interactable = !string.IsNullOrEmpty(email_InputField.text);
    }

    private void SuccessfulResetPassword()
    {
        Debug.Log("Successfull reseted password");
        OnSuccessfulResetPassword?.Invoke();
    }

    public void ResetPassword()
    {
        XsollaLogin.Instance.ResetPassword(email_InputField.text, SuccessfulResetPassword, OnUnsuccessfulResetPassword);
    }
}
