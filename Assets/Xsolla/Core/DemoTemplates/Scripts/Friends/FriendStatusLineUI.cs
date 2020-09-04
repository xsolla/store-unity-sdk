using UnityEngine;

public class FriendStatusLineUI : MonoBehaviour
{
	[SerializeField] private GameObject RequestAcceptedObject;
	[SerializeField] private GameObject RequestDeclinedObject;

	private void Start()
	{
		ClearAll();
	}

	private void ClearAll()
	{
		if(RequestAcceptedObject != null)
			RequestAcceptedObject.SetActive(false);
		if(RequestDeclinedObject != null)
			RequestDeclinedObject.SetActive(false);
	}

	public void EnableRequestAcceptedMessage()
	{
		ClearAll();
		if(RequestAcceptedObject != null)
			RequestAcceptedObject.SetActive(true);
	}
	
	public void EnableRequestDeclinedMessage()
	{
		ClearAll();
		if(RequestDeclinedObject != null)
			RequestDeclinedObject.SetActive(true);
	}
}
