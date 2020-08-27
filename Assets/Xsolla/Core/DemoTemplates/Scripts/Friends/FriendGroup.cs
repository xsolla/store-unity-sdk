using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendGroup : MonoBehaviour
{
    [SerializeField] private Text userCounterText;
    private string friendGroupName;
    
    private void Start()
    {
        friendGroupName = (userCounterText == null) ? string.Empty : userCounterText.text;
    }

    public void SetUsersCount(int count)
    {
        if (string.IsNullOrEmpty(userCounterText.text))
            return;
        if(userCounterText != null)
            userCounterText.text = count > 0 ?  $" ({count})" : string.Empty;
    }
}
