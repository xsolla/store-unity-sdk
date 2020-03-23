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

        [UnityTest]
        public IEnumerator AuthTest()
        {
            Debug.Log("Load scene");
            SceneManager.LoadScene("Login");
            yield return new WaitWhile(() =>
                SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Login")
            );
            Debug.Log("Wait 5 sec");
            yield return new WaitForSeconds(5.0F);

            Debug.Log("Find login");
            yield return new WaitWhile(() =>
                GameObject.Find("Login") == null
            );
            Debug.Log("Wait 5 sec");
            yield return new WaitForSeconds(5.0F);
            GameObject login = GameObject.Find("Login");
            Debug.Log("Set text");
            InputField inputField = login.GetComponent<InputField>();
            inputField.ActivateInputField();
            inputField.textComponent.text = "Text";
            Debug.Log("Wait 5 sec");
            yield return new WaitForSeconds(5.0F);
        }
    }
}