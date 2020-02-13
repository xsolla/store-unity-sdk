﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla;
using Xsolla.Core;
using Xsolla.Login;

public class LoginPage : Page, ILogin
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private Button login_Btn;
    [SerializeField] private Toggle rememberMe_ChkBox;
    [SerializeField] private Toggle showPassword_Toggle;
    [Header("Swap Button Images")]
    [SerializeField] private Image login_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;
    
    const string DefaultLoginId = "e6dfaac6-78a8-11e9-9244-42010aa80004";
    const string DefaultStoreProjectId = "44056";

    public Action<User> OnSuccessfulLogin { get; set; }
    public Action<Error> OnUnsuccessfulLogin { get; set; }

    private void Awake()
    {
        showPassword_Toggle.onValueChanged.AddListener((mood) => {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        login_Btn.onClick.AddListener(Login);
        login_InputField.onValueChanged.AddListener(ChangeButtonImage);
        password_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5)
        {
            if (login_Image.sprite != enabled_Sprite)
                login_Image.sprite = enabled_Sprite;
        }
        else if (login_Image.sprite == enabled_Sprite)
                login_Image.sprite = disabled_Sprite;
    }

    private void Start()
    {
        login_InputField.text = XsollaLogin.Instance.LastUserLogin;
        password_InputField.text = XsollaLogin.Instance.LastUserPassword;

        LogInHotkeys hotkeys = gameObject.GetComponent<LogInHotkeys>();
        hotkeys.EnterKeyPressedEvent += Login;
        hotkeys.TabKeyPressedEvent += ChangeFocus;
    }

	private void ChangeFocus()
	{
        if (login_InputField.isFocused)
            password_InputField.Select();
        else
            login_InputField.Select();
    }

    private void OnLogin(User user)
    {
        if (XsollaLogin.Instance.IsTokenValid && XsollaSettings.UseJwtValidation)
        {
            Debug.Log(string.Format("Your token {0} is active", XsollaLogin.Instance.Token));
        }
        else if (!XsollaSettings.UseJwtValidation)
        {
            Debug.Log("Unsafe signed in");
        }

        if (XsollaSettings.LoginId == DefaultLoginId)
        {
	        SceneManager.LoadScene("Store");
        }
        else
        {
	        if (XsollaSettings.StoreProjectId == DefaultStoreProjectId)
	        {
			    OnSuccessfulLogin?.Invoke(user);
	        }
	        else
	        {
		        SceneManager.LoadScene("Store");
	        }
        }
    }

    public void Login()
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5)
        {
            XsollaLogin.Instance.SignIn(login_InputField.text, password_InputField.text, rememberMe_ChkBox.isOn, OnLogin, OnUnsuccessfulLogin);
        }
        else
            Debug.Log("Fill all fields");
    }
}