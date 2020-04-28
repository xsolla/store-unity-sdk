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
    public class SignOut
    {
       
        const string SIGN_OUT_BUTTON = "Button_SignOut";

        

        [UnityTest]
        public IEnumerator SuccessSignOut()
        {

            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Store);
            yield return helper.WaitFor(3.5F);
            helper.ClickSimpleTextButton(SIGN_OUT_BUTTON);
            yield return helper.WaitFor(4.0F);
            Assert.True(helper.IsScene(TestHelper.Scenes.Login));
            yield return helper.WaitFor(1.5F);
        }

        
    }
}
