using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class SuccessAutorization
    {
        const int LOGIN = 123;

        const string USERNAME_FIELD = "LoginUsername";


        [UnityTest]
        public IEnumerator AuthTest()
        {
            SceneManager.LoadScene("Login");
            yield return new WaitWhile(() =>
                SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Login")
            );
            yield return new WaitWhile(() =>
                GameObject.Find(USERNAME_FIELD) == null
            );
            GameObject.Find(USERNAME_FIELD).GetComponent<InputField>().text = "test123";
            // GameObject.Find(USERNAME_FIELD).GetComponent<Button>().SendMessage()

            yield return new WaitForSeconds(5.0F);
        }
    }
}
