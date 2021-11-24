using System;
using UnityEngine;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public abstract class BaseUserStateUI : MonoBehaviour
	{
		private const string BLOCK_USER_OPTION = "BLOCK USER";
		private const string UNBLOCK_USER_OPTION = "UNBLOCK USER";
		private const string DELETE_USER_OPTION = "DELETE FRIEND";
	
		protected FriendUI Friend;
		protected FriendButtonsUI UserButtons;
		protected FriendActionsButton ActionsButton;
		protected FriendStatusLineUI StatusLine;
	
		protected abstract void InitUserButtons(FriendButtonsUI buttons);
		protected abstract void InitUserActionsButton(FriendActionsButton actionsButton);
		protected virtual void InitOtherElements()
		{ }
	
		public void Init(FriendUI friend, FriendButtonsUI userButtons, FriendStatusLineUI statusLine, FriendActionsButton actionsButton)
		{
			Friend = friend;
			UserButtons = userButtons;
			ActionsButton = actionsButton;
			StatusLine = statusLine;

			if (StatusLine != null)
				StatusLine.ClearMessage();
		
			if (UserButtons != null)
			{
				UserButtons.DisableAll();
				InitUserButtons(UserButtons);
			}
		
			if (ActionsButton != null)
			{
				ActionsButton.gameObject.SetActive(true);
				ActionsButton.ClearActions();
				InitUserActionsButton(ActionsButton);
			}

			InitOtherElements();
		}

		protected void SetState(UserState state)
		{
			Friend.SetUserState(state);
			Destroy(this);
		}

		private void AddFriendMethod(Action callback = null)
		{
			UserFriends.Instance.AddFriend(Friend.FriendModel, _ =>
			{
				SetState(UserState.Requested);
				if (callback != null)
					callback.Invoke();
			}, StoreDemoPopup.ShowError);
		}
	
		private void BlockUserMethod(Action callback = null)
		{
			PopupFactory.Instance.CreateConfirmation()
			.SetMessage(string.Format("Block {0}?", Friend.FriendModel.Nickname))
			.SetConfirmButtonText("BLOCK")
			.SetConfirmCallback(() =>
			{
				UserFriends.Instance.BlockUser(Friend.FriendModel, _ =>
				{
					SetState(UserState.Blocked);
					if (callback != null)
						callback.Invoke();
				}, StoreDemoPopup.ShowError);
			});
		}
	
		private void UnblockUserMethod(Action callback = null)
		{
			UserFriends.Instance.UnblockUser(Friend.FriendModel, _ =>
			{
				SetState(UserState.Initial);
				if (callback != null)
					callback.Invoke();
			}, StoreDemoPopup.ShowError);
		}
	
		private void AcceptFriendshipMethod(Action callback = null)
		{
			UserFriends.Instance.AcceptFriendship(Friend.FriendModel, _ =>
			{
				SetState(UserState.MyFriend);
				if (callback != null)
					callback.Invoke();
			}, StoreDemoPopup.ShowError);
		}
	
		private void DeclineFriendshipMethod(Action callback = null)
		{
			UserFriends.Instance.DeclineFriendship(Friend.FriendModel, _ =>
			{
				SetState(UserState.Initial);
				if (callback != null)
					callback.Invoke();
			}, StoreDemoPopup.ShowError);
		}

		private void CancelFriendshipRequestMethod(Action callback = null)
		{
			UserFriends.Instance.CancelFriendshipRequest(Friend.FriendModel, _ =>
			{
				SetState(UserState.Initial);
				if (callback != null)
					callback.Invoke();
			}, StoreDemoPopup.ShowError);
		}

		protected void EnableUnblockUserButton(Action callback = null)
		{
			UserButtons.EnableUnblockButton().onClick = () => { UnblockUserMethod(callback); };
		}

		protected void EnableBlockUserOption(Action callback = null)
		{
			ActionsButton.AddAction(BLOCK_USER_OPTION, () => BlockUserMethod(callback));
		}
	
		protected void EnableAddFriendButton(Action callback = null)
		{
			UserButtons.EnableAddFriendButton().onClick = () => { AddFriendMethod(callback); };
		}
	
		protected void EnableAcceptFriendshipButton(Action callback = null)
		{
			UserButtons.EnableAcceptButton().onClick = () => { AcceptFriendshipMethod(callback); };
		}
	
		protected void EnableDeclineFriendshipButton(Action callback = null)
		{
			UserButtons.EnableDeclineButton().onClick = () => { DeclineFriendshipMethod(callback); };
		}
	
		protected void EnableCancelFriendshipRequestButton(Action callback = null)
		{
			UserButtons.EnableCancelRequestButton().onClick = () => { CancelFriendshipRequestMethod(callback); };
		}
	
		protected void EnableDeleteUserOption(Action callback = null)
		{
			ActionsButton.AddAction(DELETE_USER_OPTION, () =>
			{
				PopupFactory.Instance.CreateConfirmation()
				.SetMessage(string.Format("Remove {0} from the friend list?", Friend.FriendModel.Nickname))
				.SetConfirmButtonText("REMOVE")
				.SetConfirmCallback(() =>
				{
					UserFriends.Instance.RemoveFriend(Friend.FriendModel, _ =>
					{
						SetState(UserState.Initial);
						if (callback != null)
							callback.Invoke();
					}, StoreDemoPopup.ShowError);
				});
			});
		}
	}
}
