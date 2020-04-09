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
    public class Case3_ResetPassword
    {
        const string RESET_URL = "ResetPasswordButton";
        const string RESET_FIELD = "ResetPasswordEmail";

        const string POPUP_SUCCESS = "ExtendedPopUp";

        [UnityTest]
        public IEnumerator SuccessResetPassword()
        {

            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            yield return helper.WaitFor(2.5F);
            helper.ClickButton(RESET_URL);
            yield return helper.WaitFor(1.2F);
            helper.SetInputField(RESET_FIELD, "TestReset@mail.ru");
            yield return helper.WaitFor(0.6F);
            helper.ClickButton(RESET_URL);
            yield return helper.WaitFor(1.5F);
            Assert.True(GameObject.Find(POPUP_SUCCESS));
            Assert.True(helper.IsScene(TestHelper.Scenes.Login));
        }

    
    }
}