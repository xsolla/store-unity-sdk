using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

public class UserFriends : MonoSingleton<UserFriends>
{
	private const float REFRESH_USER_FRIENDS_TIMEOUT = 20.0F;
	
	public event Action UserFriendsUpdatedEvent;
	public event Action BlockedUsersUpdatedEvent;
	public event Action PendingUsersUpdatedEvent;
	public event Action RequestedUsersUpdatedEvent;
	public event Action SocialFriendsUpdatedEvent;
	public event Action AllUsersUpdatedEvent;
	public bool IsUpdated { get; private set; }
	
	public List<FriendModel> Friends { get; private set; }
	public List<FriendModel> Blocked { get; private set; }
	public List<FriendModel> Pending { get; private set; }
	public List<FriendModel> Requested { get; private set; }
	public List<FriendModel> SocialFriends { get; private set; }

	private Coroutine RefreshCoroutine;

	public override void Init()
	{
		base.Init();
		IsUpdated = false;
		Friends = new List<FriendModel>();
		Blocked = new List<FriendModel>();
		Pending = new List<FriendModel>();
		Requested = new List<FriendModel>();
		SocialFriends = new List<FriendModel>();

		StartRefreshUsers();
	}

	public FriendModel GetUserById(string userId)
	{
		Func<FriendModel, bool> predicate = u => u.Id.Equals(userId);
		if (Friends.Any(predicate)) return Friends.First(predicate);
		if (Pending.Any(predicate)) return Pending.First(predicate);
		if (Blocked.Any(predicate)) return Blocked.First(predicate);
		if (Requested.Any(predicate)) return Requested.First(predicate);
		return null;
	}
	
	public void StartRefreshUsers()
	{
		StopRefreshUsers();
		RefreshCoroutine = StartCoroutine(PeriodicallyRefreshUserFriends());
	}

	public void StopRefreshUsers()
	{
		if (RefreshCoroutine == null) return;
		StopCoroutine(RefreshCoroutine);
		RefreshCoroutine = null;
	}

	private void RefreshUsersMethod([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		IsUpdated = false;
		StartCoroutine(UpdateFriendsCoroutine(onSuccess, onError));
	}
	
	public void UpdateFriends([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		StopRefreshUsers();
		RefreshUsersMethod(() =>
		{
			if (onSuccess != null)
				onSuccess.Invoke();

			StartRefreshUsers();
		}, err =>
		{
			if (onError != null)
				onError.Invoke(err);

			StartRefreshUsers();
		});
	}
	
	private IEnumerator PeriodicallyRefreshUserFriends()
	{
		while (true)
		{
			yield return new WaitForSeconds(REFRESH_USER_FRIENDS_TIMEOUT);
			var busy = true;
			RefreshUsersMethod(() => busy = false, _ => busy = false);
			yield return new WaitWhile(() => busy);	
		}
	}
	
	private IEnumerator UpdateFriendsCoroutine(Action onSuccess, Action<Error> onError)
	{
		bool? isTokenValid = null;
		bool socialBusy = true;
		bool friendsBusy = true;
		bool blockedBusy = true;
		bool pendingBusy = true;
		bool requestedBusy = true;

		DemoController.Instance.GetImplementation().ValidateToken(DemoController.Instance.GetImplementation().Token, onSuccess: _ => isTokenValid = true, onError: _ => isTokenValid = false);
		yield return new WaitWhile(() => isTokenValid == null);

		if (isTokenValid == true)
		{
			UpdateUserSocialFriends(() => socialBusy = false, onError);
			UpdateUserFriends(() => friendsBusy = false, onError);
			UpdateBlockedUsers(() => blockedBusy = false, onError);
			UpdatePendingUsers(() => pendingBusy = false, onError);
			UpdateRequestedUsers(() => requestedBusy = false, onError);
			yield return new WaitWhile(() => friendsBusy || blockedBusy || pendingBusy || requestedBusy || socialBusy);

			UpdateSocialFriends();
			IsUpdated = true;

			if (onSuccess != null)
				onSuccess.Invoke();

			if (AllUsersUpdatedEvent != null)
				AllUsersUpdatedEvent.Invoke();
		}
		else
		{
			StoreDemoPopup.ShowError(new Error(errorMessage: "Your token is not valid"));
			DemoController.Instance.SetState(MenuState.Authorization);
			StopRefreshUsers();
		}
	}

	private void UpdateUserSocialFriends(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().ForceUpdateFriendsFromSocialNetworks(onSuccess, onError);
	}
	
	private void UpdateUserFriends(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetUserFriends(friends =>
		{
			Friends = friends;

			if (UserFriendsUpdatedEvent != null)
				UserFriendsUpdatedEvent.Invoke();

			if (onSuccess != null)
				onSuccess.Invoke();
		}, onError);
	}
	
	private void UpdateBlockedUsers(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetBlockedUsers(users =>
		{
			Blocked = users;

			if (BlockedUsersUpdatedEvent != null)
				BlockedUsersUpdatedEvent.Invoke();

			if (onSuccess != null)
				onSuccess.Invoke();

		}, onError);
	}
	
	private void UpdatePendingUsers(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetPendingUsers(users =>
		{
			Pending = users;

			if (PendingUsersUpdatedEvent != null)
				PendingUsersUpdatedEvent.Invoke();

			if (onSuccess != null)
				onSuccess.Invoke();

		}, onError);
	}
	
	private void UpdateRequestedUsers(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetRequestedUsers(users =>
		{
			Requested = users;

			if (RequestedUsersUpdatedEvent != null)
				RequestedUsersUpdatedEvent.Invoke();

			if (onSuccess != null)
				onSuccess.Invoke();

		}, onError);
	}
	
	private void UpdateSocialFriends(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetFriendsFromSocialNetworks(users =>
		{
			SocialFriends = users;

			if (SocialFriendsUpdatedEvent != null)
				SocialFriendsUpdatedEvent.Invoke();

			if (onSuccess != null)
				onSuccess.Invoke();

		}, onError);
	}
	
	private void ShowErrorMessage(string message, Action<Error> onError = null)
	{
		Debug.LogError(message);
		var err = Error.UnknownError;
		err.errorMessage = message;

		if (onError != null)
			onError.Invoke(err);
	}

	private void RemoveUserFromMemory(FriendModel user)
	{
		if (Friends.Contains(user))
		{
			Friends.Remove(user);

			if (UserFriendsUpdatedEvent != null)
				UserFriendsUpdatedEvent.Invoke();
		}
		if (Pending.Contains(user))
		{
			Pending.Remove(user);

			if (PendingUsersUpdatedEvent != null)
				PendingUsersUpdatedEvent.Invoke();
		}
		if (Requested.Contains(user))
		{
			Requested.Remove(user);

			if (RequestedUsersUpdatedEvent != null)
				RequestedUsersUpdatedEvent.Invoke();
		}
		if (Blocked.Contains(user))
		{
			Blocked.Remove(user);

			if (BlockedUsersUpdatedEvent != null)
				BlockedUsersUpdatedEvent.Invoke();
		}
	}
	
	public void BlockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Blocked.Contains(user))
			DemoController.Instance.GetImplementation().BlockUser(user, u =>
			{
				RemoveUserFromMemory(user);
				UpdateBlockedUsers();

				if (onSuccess != null)
					onSuccess.Invoke(u);

			}, onError);
	}
	
	public void UnblockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Blocked.Contains(user))
		{
			var message = string.Format("Can not unblock user with nickname = {0}, because we have not this user in blocked friends!", user.Nickname);
			ShowErrorMessage(message, onError);
		}
		else
			DemoController.Instance.GetImplementation().UnblockUser(user, u =>
			{
				RemoveUserFromMemory(user);

				if (onSuccess != null)
					onSuccess.Invoke(u);
			}, onError);
	}
	
	public void AddFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (Friends.Contains(user) || Blocked.Contains(user) || Pending.Contains(user) || Requested.Contains(user))
		{
			var message = string.Format("Can not add friend with nickname = {0}, because this friend is not in 'initial' state!", user.Nickname);
			ShowErrorMessage(message, onError);
		}
		else
			DemoController.Instance.GetImplementation().SendFriendshipInvite(user, u =>
			{
				RemoveUserFromMemory(user);
				UpdateRequestedUsers();

				if (onSuccess != null)
					onSuccess.Invoke(u);
			}, onError);
	}
	
	public void RemoveFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Friends.Contains(user))
		{
			var message = string.Format("Can not remove friend with nickname = {0}, because we have not this friend!", user.Nickname);
			ShowErrorMessage(message, onError);
		}
		else
			DemoController.Instance.GetImplementation().RemoveFriend(user, u =>
			{
				RemoveUserFromMemory(user);

				if (onSuccess != null)
					onSuccess.Invoke(u);
			}, onError);
	}
	
	public void AcceptFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Pending.Contains(user))
		{
			var message = string.Format("Can not accept friendship from nickname = {user.Nickname}, because we have not this pending user!", user.Nickname);
			ShowErrorMessage(message, onError);
		}
		else
			DemoController.Instance.GetImplementation().AcceptFriendship(user, u =>
			{
				RemoveUserFromMemory(user);
				UpdateUserFriends();

				if (onSuccess != null)
					onSuccess.Invoke(u);
			}, onError);
	}
	
	public void DeclineFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Pending.Contains(user))
		{
			var message = string.Format("Can not accept friendship from nickname = {0}, because we have not this pending user!", user.Nickname);
			ShowErrorMessage(message, onError);
		}
		else
			DemoController.Instance.GetImplementation().DeclineFriendship(user, u =>
			{
				RemoveUserFromMemory(user);

				if (onSuccess != null)
					onSuccess.Invoke(u);

			}, onError);
	}
	
	public void CancelFriendshipRequest(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Requested.Contains(user))
		{
			var message = string.Format("Can not cancel friendship request to = {0}, because we have not this requested user!", user.Nickname);
			ShowErrorMessage(message, onError);
		}
		else
			DemoController.Instance.GetImplementation().CancelFriendshipRequest(user, u =>
			{
				RemoveUserFromMemory(user);
				if (onSuccess != null)
					onSuccess.Invoke(u);
			}, onError);
	}

	public void SearchUsersByNickname(string nickname, [CanBeNull] Action<List<FriendModel>> onSuccess = null,
		[CanBeNull] Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().SearchUsersByNickname(nickname, onSuccess, onError);
	}
}
