using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserFriends : MonoSingleton<UserFriends>
	{
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

		private void RefreshUsersMethod([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			IsUpdated = false;
			StartCoroutine(UpdateFriendsCoroutine(onSuccess, onError));
		}
	
		public void UpdateFriends([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			RefreshUsersMethod(
				onSuccess: () => onSuccess?.Invoke(),
				onError: err => onError?.Invoke(err));
		}
	
		private IEnumerator UpdateFriendsCoroutine(Action onSuccess, Action<Error> onError)
		{
			bool? isTokenValid = null;
			bool socialBusy = true;
			bool friendsBusy = true;
			bool blockedBusy = true;
			bool pendingBusy = true;
			bool requestedBusy = true;

			DemoController.Instance.LoginDemo.ValidateToken(DemoController.Instance.LoginDemo.Token, onSuccess: _ => isTokenValid = true, onError: _ => isTokenValid = false);
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
				onSuccess?.Invoke();
				AllUsersUpdatedEvent?.Invoke();
			}
			else
			{
				StoreDemoPopup.ShowError(new Error(errorMessage: "Your token is not valid"));
				DemoController.Instance.SetState(MenuState.Authorization);
			}
		}

		private void UpdateUserSocialFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.ForceUpdateFriendsFromSocialNetworks(onSuccess, onError);
		}
	
		private void UpdateUserFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.GetUserFriends(friends =>
			{
				Friends = friends;
				UserFriendsUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}
	
		private void UpdateBlockedUsers(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.GetBlockedUsers(users =>
			{
				Blocked = users;
				BlockedUsersUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}
	
		private void UpdatePendingUsers(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.GetPendingUsers(users =>
			{
				Pending = users;
				PendingUsersUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}
	
		private void UpdateRequestedUsers(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.GetRequestedUsers(users =>
			{
				Requested = users;
				RequestedUsersUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}
	
		private void UpdateSocialFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.GetFriendsFromSocialNetworks(users =>
			{
				SocialFriends = users;
				SocialFriendsUpdatedEvent?.Invoke();
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
				DemoController.Instance.LoginDemo.BlockUser(user, u =>
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
				DemoController.Instance.LoginDemo.UnblockUser(user, u =>
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
				DemoController.Instance.LoginDemo.SendFriendshipInvite(user, u =>
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
				DemoController.Instance.LoginDemo.RemoveFriend(user, u =>
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
				DemoController.Instance.LoginDemo.AcceptFriendship(user, u =>
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
				DemoController.Instance.LoginDemo.DeclineFriendship(user, u =>
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
				DemoController.Instance.LoginDemo.CancelFriendshipRequest(user, u =>
				{
					RemoveUserFromMemory(user);
					onSuccess?.Invoke(u);
				}, onError);
		}

		public void SearchUsersByNickname(string nickname, [CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null)
		{
			DemoController.Instance.LoginDemo.SearchUsersByNickname(nickname, onSuccess, onError);
		}
	}
}
