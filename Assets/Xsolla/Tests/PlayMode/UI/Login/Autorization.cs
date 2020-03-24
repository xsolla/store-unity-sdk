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
    public class Autorization
    {
        const string USERNAME_FIELD = "LoginUsername";
        const string USERPASSWORD_FIELD = "LoginPassword";
        const string USERLOGIN_BUTTON = "LoginButton";

        const string POPUP_ERROR = "ErrorPopUp";

        [UnityTest]
        public IEnumerator SuccessAutorizationTest()
        {
            Debug.Log("SUCCESS CASE");
            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitFor(8.0F);
            Assert.True(helper.IsScene(TestHelper.Scenes.Store));
        }

        [UnityTest]
        public IEnumerator FailedAutorizationTest()
        {
            Debug.Log("FAILED CASE");
            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232324");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitFor(4.0F);
            Assert.True(GameObject.Find(POPUP_ERROR));
            Assert.True(helper.IsScene(TestHelper.Scenes.Login));
        }
    }
}
