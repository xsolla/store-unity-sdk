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
    public class OnClickBuyItem
    {

        const string USERNAME_FIELD = "LoginUsername";
        const string USERPASSWORD_FIELD = "LoginPassword";
        const string USERLOGIN_BUTTON = "LoginButton";
        const string CONSUME_ITEMS_BUTTON = "CONSUME ITEMS";
   

        [UnityTest]
        public IEnumerator SuccessBuy()
        {

            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            yield return helper.WaitFor(1.0F);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitFor(5.0F);
            helper.ClickMenuButton(CONSUME_ITEMS_BUTTON);
            yield return helper.WaitFor(3.0F);
  
            Assert.True(helper.IsScene(TestHelper.Scenes.Store));
            yield return helper.WaitFor(1.5F);
        }

    }
}
