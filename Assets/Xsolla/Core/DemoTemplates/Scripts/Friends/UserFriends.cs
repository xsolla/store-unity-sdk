using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

public class UserFriends : MonoSingleton<UserFriends>
{
	public event Action FriendsUpdatedEvent;
	public bool IsUpdated { get; private set; }
	
	public List<FriendModel> Friends { get; private set; }
	public List<FriendModel> Blocked { get; private set; }
	public List<FriendModel> Pending { get; private set; }
	public List<FriendModel> Requested { get; private set; }

	public override void Init()
	{
		base.Init();
		IsUpdated = false;
		Friends = new List<FriendModel>();
		Blocked = new List<FriendModel>();
		Pending = new List<FriendModel>();
		Requested = new List<FriendModel>();
	}

	public void UpdateFriends([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		IsUpdated = false;
		StartCoroutine(UpdateFriendsCoroutine(onSuccess, onError));
	}

	private IEnumerator UpdateFriendsCoroutine(Action onSuccess, Action<Error> onError)
	{
		bool friendsBusy = true;
		bool blockedBusy = true;
		bool pendingBusy = true;
		bool requestedBusy = true;
		
		DemoController.Instance.GetImplementation().GetUserFriends(friends =>
		{
			Friends = friends;
			friendsBusy = false;
			Debug.Log($"{friendsBusy} | {blockedBusy} | {pendingBusy} | {requestedBusy}");
		}, onError);
		DemoController.Instance.GetImplementation().GetBlockedUsers(users =>
		{
			Blocked = users;
			blockedBusy = false;
			Debug.Log($"{friendsBusy} | {blockedBusy} | {pendingBusy} | {requestedBusy}");
		}, onError);
		DemoController.Instance.GetImplementation().GetPendingUsers(users =>
		{
			Pending = users;
			pendingBusy = false;
			Debug.Log($"{friendsBusy} | {blockedBusy} | {pendingBusy} | {requestedBusy}");
		}, onError);
		DemoController.Instance.GetImplementation().GetRequestedUsers(users =>
		{
			Requested = users;
			requestedBusy = false;
			Debug.Log($"{friendsBusy} | {blockedBusy} | {pendingBusy} | {requestedBusy}");
		}, onError);
		
		yield return new WaitWhile(() => friendsBusy || blockedBusy || pendingBusy || requestedBusy);
		
		Debug.Log("UPDATED");
		IsUpdated = true;
		onSuccess?.Invoke();
		FriendsUpdatedEvent?.Invoke();
	}

	public void BlockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Friends.Contains(user) && !Pending.Contains(user) && !Requested.Contains(user))
		{
			var message =
				$"Can not block user with nickname = {user.Nickname}, because we have not this user in friend system!";
			Debug.LogError(message);
			var err = Error.UnknownError;
			err.errorMessage = message;
			onError?.Invoke(err);
			return;
		}
		DemoController.Instance.GetImplementation().BlockUser(user, onSuccess, onError);
	}
	
	public void UnblockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Blocked.Contains(user))
		{
			var message =
				$"Can not unblock user with nickname = {user.Nickname}, because we have not this user in blocked friends!";
			Debug.LogError(message);
			var err = Error.UnknownError;
			err.errorMessage = message;
			onError?.Invoke(err);
			return;
		}
		DemoController.Instance.GetImplementation().UnblockUser(user, onSuccess, onError);
	}
	
	public void RemoveFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!Friends.Contains(user))
		{
			var message =
				$"Can not remove friend with nickname = {user.Nickname}, because we have not this friend!";
			Debug.LogError(message);
			var err = Error.UnknownError;
			err.errorMessage = message;
			onError?.Invoke(err);
			return;
		}
		DemoController.Instance.GetImplementation().RemoveFriend(user, onSuccess, onError);
	}
}
