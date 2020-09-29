using UnityEngine;
using UnityEngine.UI;

public class FriendGroup : MonoBehaviour
{
    [SerializeField] private Text userCounterText;
    
    public void SetUsersCount(int count)
    {
        if(userCounterText != null)
            userCounterText.text = count > 0 ?  $" ({count})" : string.Empty;
    }
}
