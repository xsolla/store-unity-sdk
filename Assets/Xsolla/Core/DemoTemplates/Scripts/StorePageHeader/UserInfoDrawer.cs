using UnityEngine;
using UnityEngine.UI;

public class UserInfoDrawer : MonoBehaviour
{
    [SerializeField] private Text userEmail;
    
    void Start()
    {
        if (UserInfoContainer.LastEmail != null && userEmail != null)
        {
            userEmail.text = UserInfoContainer.LastEmail;
        }
        Destroy(this, 0.1F);
    }
}
