using UnityEngine;
using UnityEngine.UI;

public class UserInfoDrawer : MonoBehaviour
{
    [SerializeField] private Text userEmail;
    
    void Start()
    {
        if (UserInfoContainer.LastEmail != null && userEmail != null)
        {
            userEmail.text = UserInfoContainer.LastEmail.ToUpper();
        }
		else
		{
			userEmail.text = string.Empty;
		}

        Destroy(this, 0.1F);
    }
}
