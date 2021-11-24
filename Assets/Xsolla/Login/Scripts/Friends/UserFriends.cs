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

		private Coroutine _refreshCoroutine;

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
			Func<FriendModel, bool> predicate = user => user.Id.Equals(userId);
			var result = Friends.FirstOrDefault(predicate);

			if (result == null)
				result = Pending.FirstOrDefault(predicate);
			if (result == null)
				result = Blocked.FirstOrDefault(predicate);
			if (result == null)
				result = Requested.FirstOrDefault(predicate);

			return result;
		}

		public void UpdateFriends([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			IsUpdated = false;
			StartCoroutine(UpdateFriendsCoroutine(onSuccess, onError));
		}

		private IEnumerator UpdateFriendsCoroutine(Action onSuccess, Action<Error> onError)
		{
			bool? isTokenValid = null;
			bool socialBusy = true;
			bool friendsBusy = true;
			bool blockedBusy = true;
			bool pendingBusy = true;
			bool requestedBusy = true;

			SdkLoginLogic.Instance.ValidateToken(
				Token.Instance,
				onSuccess: _ => isTokenValid = true,
				onError: _ => isTokenValid = false
			);

			yield return new WaitWhile(() => isTokenValid == null);

			if (isTokenValid == true)
			{
				UpdateUserSocialFriends(() => socialBusy = false, onError);
				UpdateUserFriends(() => friendsBusy = false, onError);
				UpdateBlockedUsers(() => blockedBusy = false, onError);
				UpdatePendingUsers(() => pendingBusy = false, onError);
				UpdateRequestedUsers(() => requestedBusy = false, onError);
				yield return new WaitWhile(() => friendsBusy || blockedBusy || pendingBusy || requestedBusy || socialBusy);

				UpdateSocialFriends(onError: onError);
				IsUpdated = true;
				if (onSuccess != null)
					onSuccess.Invoke();
				if (AllUsersUpdatedEvent != null)
					AllUsersUpdatedEvent.Invoke();
			}
			else
			{
				if (onError != null)
					onError.Invoke(new Error(ErrorType.InvalidToken, errorMessage:"Your token is not valid"));
			}
		}

		private void UpdateUserSocialFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			SdkFriendsLogic.Instance.ForceUpdateFriendsFromSocialNetworks(onSuccess, onError);
		}

		private void UpdateUserFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			SdkFriendsLogic.Instance.GetUserFriends(friends =>
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
			SdkFriendsLogic.Instance.GetBlockedUsers(users =>
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
			SdkFriendsLogic.Instance.GetPendingUsers(users =>
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
			SdkFriendsLogic.Instance.GetRequestedUsers(users =>
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
			SdkFriendsLogic.Instance.GetFriendsFromSocialNetworks(users =>
			{
				SocialFriends = users;
				if (SocialFriendsUpdatedEvent != null)
					SocialFriendsUpdatedEvent.Invoke();
				if (onSuccess != null)
					onSuccess.Invoke();
			}, onError);
		}

		private void RaiseOnError(string message, Action<Error> onError = null)
		{
			Debug.LogError(message);
			var error = new Error(ErrorType.IncorrectFriendState, errorMessage: message);
			if (onError != null)
				onError.Invoke(error);
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
			if (Blocked.Contains(user))
				RaiseOnError(string.Format("Can not block user with this nickname = {0}. They are already blocked.", user.Nickname), onError);
			else
			{
				SdkFriendsLogic.Instance.BlockUser(user, blockedUser =>
				{
					RemoveUserFromMemory(user);
					UpdateBlockedUsers(onError: onError);
					if (onSuccess != null)
						onSuccess.Invoke(blockedUser);
				}, onError);
			}
		}

		public void UnblockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Blocked.Contains(user))
				RaiseOnError(string.Format("Can not unblock user with this nickname = {0}. This user is not in the list of blocked friends.", user.Nickname), onError);
			else
				SdkFriendsLogic.Instance.UnblockUser(user, u =>
				{
					RemoveUserFromMemory(user);
					if (onSuccess != null)
						onSuccess.Invoke(u);
				}, onError);
		}

		public void AddFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (Friends.Contains(user) || Blocked.Contains(user) || Pending.Contains(user) || Requested.Contains(user))
				RaiseOnError(string.Format("Can not add friend with this nickname = {0}. This friend is not in the 'initial' state.", user.Nickname), onError);
			else
				SdkFriendsLogic.Instance.SendFriendshipInvite(user, u =>
				{
					RemoveUserFromMemory(user);
					UpdateRequestedUsers(onError: onError);
					if (onSuccess != null)
						onSuccess.Invoke(u);
				}, onError);
		}

		public void RemoveFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Friends.Contains(user))
				RaiseOnError(string.Format("Can not remove friend with this nickname = {0}. This user is not in the friend list.", user.Nickname), onError);
			else
			{
				SdkFriendsLogic.Instance.RemoveFriend(user, u =>
				{
					if (onSuccess != null)
						onSuccess.Invoke(u);
				}, onError);
			}
		}

		public void AcceptFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Pending.Contains(user))
				RaiseOnError(string.Format("Can not accept friendship from the user = {0}. They are not a pending user.", user.Nickname), onError);
			else
				SdkFriendsLogic.Instance.AcceptFriendship(user, u =>
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
				RaiseOnError(string.Format("Can not accept friendship from the user = {0}. They are not a pending user.", user.Nickname), onError);
			else
				SdkFriendsLogic.Instance.DeclineFriendship(user, u =>
				{
					RemoveUserFromMemory(user);
					if (onSuccess != null)
						onSuccess.Invoke(u);
				}, onError);
		}

		public void CancelFriendshipRequest(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Requested.Contains(user))
				RaiseOnError(string.Format("Can not cancel friendship request to = {0}. This user doesn't have a friend request.", user.Nickname), onError);
			else
				SdkFriendsLogic.Instance.CancelFriendshipRequest(user, u =>
				{
					RemoveUserFromMemory(user);
					if (onSuccess != null)
						onSuccess.Invoke(u);
				}, onError);
		}

		public void SearchUsersByNickname(string nickname, [CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null)
		{
			SdkLoginLogic.Instance.SearchUsersByNickname(nickname, onSuccess, onError);
		}
	}
}
