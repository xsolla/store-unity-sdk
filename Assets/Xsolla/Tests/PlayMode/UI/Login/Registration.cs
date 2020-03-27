using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;
using NUnit.Framework;


namespace Tests
{
	public class Registration
	{
		
		const string SIGNUP_BUTTON = "SignUpTabButton";
		const string SIGNUP_USERNAME_FIELD = "SignUpUsername";
		const string SIGNUP_EMAIL_FIELD = "SignUpEmail";
		const string SIGNUP_PASSWORD_FIELD = "SignUpPassword";
		const string SIGNUP_CREATE_BUTTON = "CreateAccountButton";

		const string POPUP_ERROR = "ErrorPopUp";
		const string POPUP_SUCCESS = "ExtendedPopUp";

	

		[UnityTest]
		public IEnumerator FailedRegistration()
		{

			TestHelper helper = TestHelper.Instance;
		
			yield return helper.LoadScene(TestHelper.Scenes.Login);
			yield return helper.WaitFor(1.5F);
			helper.ClickButton(SIGNUP_BUTTON);
			yield return helper.WaitFor(0.5F);
			helper.SetInputField(SIGNUP_USERNAME_FIELD, "TestUsername");
			yield return helper.WaitFor(0.5F);
			helper.SetInputField(SIGNUP_EMAIL_FIELD, "TestEmail@mail.ru");
			yield return helper.WaitFor(0.5F);
			helper.SetInputField(SIGNUP_PASSWORD_FIELD, "123456");
			yield return helper.WaitFor(0.5F);
			helper.ClickButton(SIGNUP_CREATE_BUTTON);
			yield return helper.WaitFor(2.5F);
			Assert.True(GameObject.Find(POPUP_ERROR));
			Assert.True(helper.IsScene(TestHelper.Scenes.Login));
		
		}

		[UnityTest]
		public IEnumerator SuccessRegistration()
		{
			

			TestHelper helper = TestHelper.Instance;
			yield return helper.LoadScene(TestHelper.Scenes.Login);
			string timeStamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString();
			string login_value = "sdk_autotest_"+timeStamp.ToString();
			string email_value =  timeStamp.ToString()+ "sdk_autotest@mail.ru";
			yield return helper.WaitFor(1.5F);
			helper.ClickButton(SIGNUP_BUTTON);
			yield return helper.WaitFor(0.5F);
			helper.SetInputField(SIGNUP_USERNAME_FIELD, login_value);
			yield return helper.WaitFor(0.5F);
			helper.SetInputField(SIGNUP_EMAIL_FIELD, email_value);
			yield return helper.WaitFor(0.5F);
			helper.SetInputField(SIGNUP_PASSWORD_FIELD, "123456");
			yield return helper.WaitFor(0.5F);
			helper.ClickButton(SIGNUP_CREATE_BUTTON);
			yield return helper.WaitFor(2.5F);
			Assert.True(GameObject.Find(POPUP_SUCCESS));
			Assert.True(helper.IsScene(TestHelper.Scenes.Login));
			
		}
	}
}
