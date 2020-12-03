using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class UserInfoDrawer : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Text UserEmailText;
#pragma warning restore 0649

	private const int USER_INFO_LIMIT = 42;

	IEnumerator Start()
    {
	    if (!UserEmailText)
	    {
		    Debug.LogWarning("Can not draw user info because 'UserEmailText' field is null or empty!");
		    yield break;
	    }

	    yield return new WaitWhile(() => DemoController.Instance.GetImplementation().Token.IsNullOrEmpty());
	    
	    var busy = true;
		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().GetUserInfo(token, info =>
	    {
			DrawInfo(info);
		    busy = false;
	    }, _ => busy = false);
	    
	    yield return new WaitWhile(() => busy);
	    //Destroy(this, 0.1F);
		this.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
	}

	private void DrawInfo(UserInfo info)
	{
		string nameToDraw;

		if (!string.IsNullOrEmpty(info.nickname))
			nameToDraw = info.nickname;
		else if (!string.IsNullOrEmpty(info.email))
			nameToDraw = info.email;
		else
			nameToDraw = string.Empty;

		if (nameToDraw.Length > USER_INFO_LIMIT)
			nameToDraw = nameToDraw.Substring(0, USER_INFO_LIMIT);

		UserEmailText.text = nameToDraw;
	}

	public void Refresh()
	{
		StartCoroutine(Start());
	}
}
