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
    public class Attributes
    {

        const string USERNAME_FIELD = "LoginUsername";
        const string USERPASSWORD_FIELD = "LoginPassword";
        const string USERLOGIN_BUTTON = "LoginButton";
        const string ATTRIB_BUTTON = "AttributesButton";
        const string NEW_BUTTON = "Item.New";
        const string NAME_ATRRIB = "InputField.Key";
        const string VALUE_ATRRIB = "InputField.Value";
        const string SAVE_ATRRIB_BUTTON = "pref_SimpleTextButton";
        const string ATRRIB_OBJECT = "AttributeItem(Clone)";
        const string POPUP_BUTTON = "SuccessPopupButton";
        const string STORE_BUTTON = "StoreButton";
        const string REMOVE_BUTTON = "pref_SimpleIconButton";

        [UnityTest]
        public IEnumerator AddedAtribut()
        {

            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            yield return helper.WaitFor(1.0F);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitFor(5.0F);
            helper.ClickMenuButton(ATTRIB_BUTTON);
            yield return helper.WaitFor(1.0F);
            helper.ClickSimpleTextButton(NEW_BUTTON);
            yield return helper.WaitFor(2.0F);
            AttributeItemUI attribute = helper.Find<AttributeItemUI>(ATRRIB_OBJECT);
            helper.SetInputField(NAME_ATRRIB, "AutoTestAttrib");
            attribute.OnKeyEdited();
            yield return helper.WaitFor(1.5F);
            helper.SetInputField(VALUE_ATRRIB, "AutoTestValue");
            attribute.OnValueEdited();
            yield return helper.WaitFor(1.5F);
            helper.ClickSimpleTextButton(SAVE_ATRRIB_BUTTON);
            yield return helper.WaitFor(2.0F);
            helper.ClickSimpleTextButton(POPUP_BUTTON);
            yield return helper.WaitFor(2.0F);
            helper.ClickMenuButton(STORE_BUTTON);
            yield return helper.WaitFor(1.5F);
            Assert.True(GameObject.Find("AttributesSidePanelItem(Clone)"));
            Assert.True(helper.IsScene(TestHelper.Scenes.Store));
            yield return helper.WaitFor(1.5F);
        }


        [UnityTest]
        public IEnumerator DeleteAtribut()
        {

            TestHelper helper = TestHelper.Instance;
            yield return helper.LoadScene(TestHelper.Scenes.Login);
            yield return helper.WaitFor(1.0F);
            helper.SetInputField(USERNAME_FIELD, "test123");
            helper.SetInputField(USERPASSWORD_FIELD, "232323");
            yield return helper.WaitFor(0.01F);
            helper.ClickButton(USERLOGIN_BUTTON);
            yield return helper.WaitFor(5.0F);
            helper.ClickMenuButton(ATTRIB_BUTTON);
            yield return helper.WaitFor(1.0F);
            helper.ClickSimpleButton(REMOVE_BUTTON);
            yield return helper.WaitFor(2.5F);
            helper.ClickSimpleTextButton(SAVE_ATRRIB_BUTTON);
            yield return helper.WaitFor(4.5F);
            helper.ClickSimpleTextButton(POPUP_BUTTON);
            yield return helper.WaitFor(2.5F);
            helper.ClickMenuButton(STORE_BUTTON);
            yield return helper.WaitFor(1.5F);
            Assert.False(GameObject.Find("AttributesSidePanelItem(Clone)"));
            Assert.True(helper.IsScene(TestHelper.Scenes.Store));
            yield return helper.WaitFor(1.5F);
           
 
        }


    }
}
