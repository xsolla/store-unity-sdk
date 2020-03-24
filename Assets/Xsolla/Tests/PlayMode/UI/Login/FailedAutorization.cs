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
    public class FailedAutorization
    {
        const string USERNAME_FIELD = "LoginUsername";
        const string USERPASSWORD_FIELD = "LoginPassword";
        const string USERLOGIN_BUTTON = "LoginButton";
        const string POPUP_ERROR = "ErrorPopUp";

        [UnityTest]

     
        public IEnumerator FailedAutorizationTest()
        {
            SceneManager.LoadScene("Login");
            yield return new WaitWhile(() =>
                SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Login")
            );
            yield return new WaitWhile(() =>
                GameObject.Find(USERNAME_FIELD) == null
            );
            GameObject.Find(USERNAME_FIELD).GetComponent<InputField>().text = "test123";
            GameObject.Find(USERPASSWORD_FIELD).GetComponent<InputField>().text = "232324";
            yield return new WaitForSeconds(0.01F);
            GameObject.Find(USERLOGIN_BUTTON).GetComponent<Button>().onClick.Invoke();
            yield return new WaitForSeconds(4.0F);
            Assert.True(GameObject.Find(POPUP_ERROR));

        }
    }
}
