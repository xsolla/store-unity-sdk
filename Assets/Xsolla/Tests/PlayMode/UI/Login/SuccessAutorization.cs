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
    public class SuccessAutorization
    {
        const string USERNAME_FIELD = "LoginUsername";
        const string USERPASSWORD_FIELD = "LoginPassword";
        const string USERLOGIN_BUTTON = "LoginButton";

        [UnityTest]
        public IEnumerator SuccessAutorizationTest()
        {
            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitFor(4.0F);
            Assert.True(helper.IsScene(TestHelper.Scenes.Store));
        }

        [UnityTest]
        public IEnumerator SuccessAutorizationTest2()
        {
            yield break;
        }
    }
}
