using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

public class UserFriends : MonoSingleton<UserFriends>
{
	private const float REFRESH_USER_FRIENDS_TIMEOUT = 10.0F;
	
	public event Action UserFriendsUpdatedEvent;
	public event Action BlockedUsersUpdatedEvent;
	public event Action PendingUsersUpdatedEvent;
	public event Action RequestedUsersUpdatedEvent;
	public event Action AllUsersUpdatedEvent;
	public bool IsUpdated { get; private set; }
	
	public List<FriendModel> Friends { get; private set; }
	public List<FriendModel> Blocked { get; private set; }
	public List<FriendModel> Pending { get; private set; }
	public List<FriendModel> Requested { get; private set; }

	private Coroutine RefreshCoroutine;

	public override void Init()
	{
		base.Init();
		IsUpdated = false;
		Friends = new List<FriendModel>();
		Blocked = new List<FriendModel>();
		Pending = new List<FriendModel>();
		Requested = new List<FriendModel>();

		StartRefreshUsers();
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
			onSuccess?.Invoke();
			StartRefreshUsers();
		}, err =>
		{
			onError?.Invoke(err);
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
		bool friendsBusy = true;
		bool blockedBusy = true;
		bool pendingBusy = true;
		bool requestedBusy = true;

		UpdateUserFriends(() => friendsBusy = false, onError);
		UpdateBlockedUsers(() => blockedBusy = false, onError);
		UpdatePendingUsers(() => pendingBusy = false, onError);
		UpdateRequestedUsers(() => requestedBusy = false, onError);
		yield return new WaitWhile(() => friendsBusy || blockedBusy || pendingBusy || requestedBusy);
		
		IsUpdated = true;
		onSuccess?.Invoke();
		AllUsersUpdatedEvent?.Invoke();
	}
	
	private void UpdateUserFriends(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetUserFriends(friends =>
		{
			Friends = friends;
			UserFriendsUpdatedEvent?.Invoke();
			onSuccess?.Invoke();
		}, onError);
	}
	
	private void UpdateBlockedUsers(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetBlockedUsers(users =>
		{
			Blocked = users;
			BlockedUsersUpdatedEvent?.Invoke();
			onSuccess?.Invoke();
		}, onError);
	}
	
	private void UpdatePendingUsers(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetPendingUsers(users =>
		{
			Pending = users;
			PendingUsersUpdatedEvent?.Invoke();
			onSuccess?.Invoke();
		}, onError);
	}
	
	private void UpdateRequestedUsers(Action onSuccess = null, Action<Error> onError = null)
	{
		DemoController.Instance.GetImplementation().GetRequestedUsers(users =>
		{
			Requested = users;
			RequestedUsersUpdatedEvent?.Invoke();
			onSuccess?.Invoke();
		}, onError);
	}
	
	private void ShowErrorMessage(string message, Action<Error> onError = null)
	{
		Debug.LogError(message);
		var err = Error.UnknownError;
		err.errorMessage = message;
		onError?.Invoke(err);
	}

	private void RemoveUserFromMemory(FriendModel user)
	{
		if (Friends.Contains(user))
		{
			Friends.Remove(user);
			UserFriendsUpdatedEvent?.Invoke();
		}
		if (Pending.Contains(user))
		{
			Pending.Remove(user);
			PendingUsersUpdatedEvent?.Invoke();
		}
		if (Requested.Contains(user))
		{
			Requested.Remove(user);
			RequestedUsersUpdatedEvent?.Invoke();
		}
		if (Blocked.Contains(user))
		{
			Blocked.Remove(user);
			BlockedUsersUpdatedEvent?.Invoke();
		}
	}
	
	public void BlockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Blocked.Contains(user))
			DemoController.Instance.GetImplementation().BlockUser(user, u =>
			{
				RemoveUserFromMemory(user);
				UpdateBlockedUsers();
				onSuccess?.Invoke(u);
			}, onError);
	}
	
	public void UnblockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Blocked.Contains(user))
			ShowErrorMessage($"Can not unblock user with nickname = {user.Nickname}, because we have not this user in blocked friends!", onError);
		else
			DemoController.Instance.GetImplementation().UnblockUser(user, u =>
			{
				RemoveUserFromMemory(user);
				onSuccess?.Invoke(u);
			}, onError);
	}
	
	public void AddFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (Friends.Contains(user) || Blocked.Contains(user) || Pending.Contains(user) || Requested.Contains(user))
			ShowErrorMessage($"Can not add friend with nickname = {user.Nickname}, because this friend is not in 'initial' state!", onError);
		else
			DemoController.Instance.GetImplementation().SendFriendshipInvite(user, u =>
			{
				RemoveUserFromMemory(user);
				UpdateRequestedUsers();
				onSuccess?.Invoke(u);
			}, onError);
	}
	
	public void RemoveFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Friends.Contains(user))
			ShowErrorMessage($"Can not remove friend with nickname = {user.Nickname}, because we have not this friend!", onError);
		else
			DemoController.Instance.GetImplementation().RemoveFriend(user, u =>
			{
				RemoveUserFromMemory(user);
				onSuccess?.Invoke(u);
			}, onError);
	}
	
	public void AcceptFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Pending.Contains(user))
			ShowErrorMessage($"Can not accept friendship from nickname = {user.Nickname}, because we have not this pending user!", onError);
		else
			DemoController.Instance.GetImplementation().AcceptFriendship(user, u =>
			{
				RemoveUserFromMemory(user);
				UpdateUserFriends();
				onSuccess?.Invoke(u);
			}, onError);
	}
	
	public void DeclineFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Pending.Contains(user))
			ShowErrorMessage($"Can not accept friendship from nickname = {user.Nickname}, because we have not this pending user!", onError);
		else
			DemoController.Instance.GetImplementation().DeclineFriendship(user, u =>
			{
				RemoveUserFromMemory(user);
				onSuccess?.Invoke(u);
			}, onError);
	}
	
	public void CancelFriendshipRequest(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Requested.Contains(user))
			ShowErrorMessage($"Can not cancel friendship request to = {user.Nickname}, because we have not this requested user!", onError);
		else
			DemoController.Instance.GetImplementation().CancelFriendshipRequest(user, u =>
			{
				RemoveUserFromMemory(user);
				onSuccess?.Invoke(u);
			}, onError);
	}
}
