using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
    public class UserAttributes : MonoSingleton<UserAttributes>
	{
		private List<UserAttribute> _readOnlyAttributes;
		private List<UserAttribute> _customAttributesOriginal;
		private List<UserAttribute> _customAttributes;
		private List<UserAttribute> _modifiedAttributes;
		private List<string> _removedAttributes;

		private bool _isUpdateInProgress;
		private Queue<UpdateQueueItem> _updateQueue = new Queue<UpdateQueueItem>();

		//TEXTREVIEW
		/// <summary>
		/// Returns count of changes since last push, -1 if attributes were not initialized
		/// </summary>
		public int ChangesCount
		{
			get
			{
				if (_modifiedAttributes == null || _removedAttributes == null)
				{
					Debug.LogError($"UserAttributes: Get or update attributes before use");
					return -1;
				}

				return (_modifiedAttributes.Count + _removedAttributes.Count);
			}
		}

		//TEXTREVIEW
		/// <summary>
		/// Updates current user attributes from server. Also called from withtin GetAttributes method if not called earlier.
		/// </summary>
		/// <param name="onSuccess">Success callback</param>
		/// <param name="onError">Error callback</param>
		public void UpdateAttributes(Action onSuccess = null, Action<Error> onError = null)
		{
			_isUpdateInProgress = true;

			List<string> attributeKeys = null;//Get all existing attributes without filter
			string userId = null;//Get current user attributes
			string token = Token.Instance;
			string projectId = XsollaSettings.StoreProjectId;

			void FinalizeUpdate(List<UserAttribute> readOnlyAttributes, List<UserAttribute> customAttributes, Error error)
			{
				if (error != null)
				{
					while (_updateQueue.Count > 0)
					{
						var item = _updateQueue.Dequeue();
						item.OnError?.Invoke(error);
					}

					onError?.Invoke(error);
				}

				if (readOnlyAttributes != null && customAttributes != null)
				{
					_readOnlyAttributes = readOnlyAttributes;
					_customAttributesOriginal = customAttributes;
					_customAttributes = new List<UserAttribute>(_customAttributesOriginal);
					_modifiedAttributes = new List<UserAttribute>();
					_removedAttributes = new List<string>();

					while (_updateQueue.Count > 0)
					{
						var item = _updateQueue.Dequeue();
						item.OnGetReadonly?.Invoke(readOnlyAttributes);
						item.OnGetCustom?.Invoke(customAttributes);
					}

					onSuccess?.Invoke();
				}

				_isUpdateInProgress = false;
			}

			Action<Error> onErrorGet = error =>
			{
				FinalizeUpdate(null, null, error);
			};

			Action<List<UserAttribute>> onSuccessGetReadonly = readOnlyAttributes =>
			{
				Action<List<UserAttribute>> onSuccessGetCustom = customAttributes => FinalizeUpdate(readOnlyAttributes, customAttributes, error: null);
				SdkUserAccountLogic.Instance.GetUserAttributes(token, projectId, UserAttributeType.CUSTOM, attributeKeys, userId, onSuccessGetCustom, onErrorGet);
			};

			SdkUserAccountLogic.Instance.GetUserAttributes(token, projectId, UserAttributeType.READONLY, attributeKeys, userId, onSuccessGetReadonly, onErrorGet);
		}

		//TEXTREVIEW
		/// <summary>
		/// Returns user readonly attributes. Updates user attributes if not done beforehand.
		/// </summary>
		/// <param name="onSuccess">Success callback</param>
		/// <param name="onError">Error callback</param>
		public void GetReadonlyAttributes(Action<List<UserAttribute>> onSuccess, Action<Error> onError)
		{
			if (_readOnlyAttributes != null)
			{
				onSuccess?.Invoke(_readOnlyAttributes);
				return;
			}

			_updateQueue.Enqueue(new UpdateQueueItem() {OnGetReadonly=onSuccess, OnError=onError});

			if (!_isUpdateInProgress)
				UpdateAttributes();
		}

		//TEXTREVIEW
		/// <summary>
		/// Returns user custom attributes. Updates user attributes if not done beforehand.
		/// </summary>
		/// <param name="onSuccess">Success callback</param>
		/// <param name="onError">Error callback</param>
		public void GetCustomAttributes(Action<List<UserAttribute>> onSuccess, Action<Error> onError)
		{
			if (_customAttributes != null)
			{
				onSuccess?.Invoke(_customAttributes);
				return;
			}

			_updateQueue.Enqueue(new UpdateQueueItem() { OnGetCustom = onSuccess, OnError = onError });

			if (!_isUpdateInProgress)
				UpdateAttributes();
		}

		//TEXTREVIEW
		/// <summary>
		/// Replaces one key of custom attribute with another, keeping the value.
		/// </summary>
		/// <param name="oldKey">Key to replace</param>
		/// <param name="newKey">New key</param>
		/// <param name="error">Error code, 'None' if replacement succeeded</param>
		/// <returns>True in case of success, false otherwise</returns>
		public bool ChangeKey(string oldKey, string newKey, out AttributesError error)
		{
			var attributeToModify = GetAttribute(oldKey, out error);
			if (attributeToModify == null)
				return false;

			if (oldKey == newKey)
			{
				Debug.LogError($"UserAttributes.ChangeKey: New key equals old key. OldKey:{oldKey} NewKey:{newKey}");
				error = AttributesError.IncorrectKey;
				return false;
			}

			if (newKey.Length > 256 || !Regex.IsMatch(newKey, "^[A-Za-z0-9_]+$"))
			{
				Debug.LogError($"UserAttributes.ChangeKey: New key does not follow rules MaxLength:256 Pattern:[A-Za-z0-9_]+. OldKey:{oldKey} NewKey:{newKey}");
				error = AttributesError.IncorrectKey;
				return false;
			}

			if (!IsNewKey(newKey))
			{
				Debug.LogError($"UserAttributes.ChangeKey: This key already exists. OldKey:{oldKey} NewKey:{newKey}");
				error = AttributesError.IncorrectKey;
				return false;
			}

			error = AttributesError.None;
			attributeToModify.key = newKey;
			if (!_modifiedAttributes.Contains(attributeToModify))
				_modifiedAttributes.Add(attributeToModify);

			return true;
		}

		//TEXTREVIEW
		/// <summary>
		/// Changes the value of custom attribute
		/// </summary>
		/// <param name="key">Attribute key</param>
		/// <param name="newValue">New value</param>
		/// <param name="error">Error code, 'None' if value update succeeded</param>
		/// <returns>True in case of success, false otherwise</returns>
		public bool ChangeValue(string key, string newValue, out AttributesError error)
		{
			var attributeToModify = GetAttribute(key, out error);
			if (attributeToModify == null)
				return false;

			if (string.IsNullOrEmpty(newValue))
			{
				Debug.LogError($"UserAttributes.ChangeValue: Value can not be null or empty");
				error = AttributesError.IncorrectValue;
				return false;
			}

			if (newValue.Length > 256)
			{
				Debug.LogError($"UserAttributes.ChangeValue: New value does not follow rule MaxLength:256 Key:{key} NewValue:{newValue}");
				error = AttributesError.IncorrectValue;
				return false;
			}

			error = AttributesError.None;
			attributeToModify.value = newValue;
			if (!_modifiedAttributes.Contains(attributeToModify))
				_modifiedAttributes.Add(attributeToModify);

			return true;
		}

		//TEXTREVIEW
		/// <summary>
		/// Removes custom attribute
		/// </summary>
		/// <param name="key">Attribute key</param>
		/// <param name="error">Error code, 'None' if deletion succeeded</param>
		/// <returns>True in case of success, false otherwise</returns>
		public bool RemoveAttribute(string key, out AttributesError error)
		{
			var attributeToDelete = GetAttribute(key, out error);
			if (attributeToDelete == null)
				return false;

			error = AttributesError.None;
			_customAttributes.Remove(attributeToDelete);
			_modifiedAttributes.Remove(attributeToDelete);
			_removedAttributes.Add(key);

			return true;
		}

		//TEXTREVIEW
		/// <summary>
		/// Adds new attribute
		/// </summary>
		/// <param name="newKey">Attribute key</param>
		/// <param name="newValue">Attribute value</param>
		/// <param name="error">Error code, 'None' if addition succeeded</param>
		/// <param name="isPublic">Mark attribute as 'public' or 'private'</param>
		/// <returns>New attribute in case of success, null otherwise</returns>
		public UserAttribute AddNewAttribute(string newKey, string newValue, out AttributesError error, bool isPublic = true)
		{
			if (_customAttributes == null)
			{
				Debug.LogError($"UserAttributes: Get or update attributes before use");
				error = AttributesError.NotInitialized;
				return null;
			}

			if (string.IsNullOrEmpty(newKey))
			{
				Debug.LogError($"UserAttributes.AddNewAttribute: Key can not be null or empty");
				error = AttributesError.IncorrectKey;
				return null;
			}

			if (newKey.Length > 256 || !Regex.IsMatch(newKey, "^[A-Za-z0-9_]+$"))
			{
				Debug.LogError($"UserAttributes.AddNewAttribute: New key does not follow rules MaxLength:256 Pattern:[A-Za-z0-9_]+. NewKey:{newKey}");
				error = AttributesError.IncorrectKey;
				return null;
			}

			if (string.IsNullOrEmpty(newValue))
			{
				Debug.LogError($"UserAttributes.AddNewAttribute: Value can not be null or empty");
				error = AttributesError.IncorrectValue;
				return null;
			}

			if (newValue.Length > 256)
			{
				Debug.LogError($"UserAttributes.AddNewAttribute: New value does not follow rule MaxLength:256 Key:{newKey} NewValue:{newValue}");
				error = AttributesError.IncorrectValue;
				return null;
			}

			if (!IsNewKey(newKey))
			{
				Debug.LogError($"UserAttributes.AddNewAttribute: This key already exists. NewKey:{newKey} NewValue:{newValue}");
				error = AttributesError.IncorrectKey;
				return null;
			}

			error = AttributesError.None;
			var newAttribute = new UserAttribute();
			newAttribute.key = newKey;
			newAttribute.permission = isPublic ? "public" : "private";
			newAttribute.value = newValue;

			_customAttributes.Add(newAttribute);
			_modifiedAttributes.Add(newAttribute);

			return newAttribute;
		}

		//TEXTREVIEW
		/// <summary>
		/// Sends accumulated changes to server. Must be called before application close in order to maintain changes.
		/// </summary>
		/// <param name="onSuccess">Success callback</param>
		/// <param name="onCancelled">Cancelled callback. Called if attributes were not loaded beforehand or no changes made</param>
		/// <param name="onError">Error callback</param>
		public void PushChanges(Action onSuccess = null, Action onCancelled = null, Action<Error> onError = null)
		{
			var changesCount = ChangesCount;
			if (changesCount < 1)
			{
				Debug.Log($"UserAttributes.PushChanges: Push cancelled. Changes count:'{changesCount}'");
				onCancelled?.Invoke();
				return;
			}

			if (_removedAttributes.Count > 0 && _modifiedAttributes.Count > 0)
			{
				Action onSuccessDeletion = () =>
				{
					Action onSuccessUpdate = () => FinalizePush(onSuccess);
					PushModifiedChanges(onSuccessUpdate, onError);
				};
				Action<Error> onErrorDeletion = error =>
				{
					onError?.Invoke(error);
					Action onSuccessUpdate = () => FinalizePush(onSuccess);
					PushModifiedChanges(onSuccessUpdate, onError);
				};
				PushRemoveChanges(onSuccessDeletion, onError);
			}
			else if (_modifiedAttributes.Count > 0)
			{
				Action onSuccessUpdate = () => FinalizePush(onSuccess);
				PushModifiedChanges(onSuccessUpdate,onError);
			}
			else if (_removedAttributes.Count > 0)
			{
				Action onSuccessDeletion = () => FinalizePush(onSuccess);
				PushRemoveChanges(onSuccessDeletion, onError);
			}
		}

		//TEXTREVIEW
		/// <summary>
		/// Reverts local changes
		/// </summary>
		public bool RevertChanges()
		{
			if (ChangesCount < 1)
				return false;

			_customAttributes.Clear();
			_customAttributes.AddRange(_customAttributesOriginal);
			_modifiedAttributes.Clear();
			_removedAttributes.Clear();
			return true;
		}

		private UserAttribute GetAttribute(string key, out AttributesError error)
		{
			if (_customAttributes == null)
			{
				Debug.LogError($"UserAttributes: Get or update attributes before use");
				error = AttributesError.NotInitialized;
				return null;
			}

			if (string.IsNullOrEmpty(key))
			{
				Debug.LogError($"UserAttributes: Key can not be null or empty");
				error = AttributesError.IncorrectKey;
				return null;
			}

			UserAttribute attributeToReturn = null;
			foreach (var attribute in _customAttributes)
			{
				if (attribute.key == key)
				{
					attributeToReturn = attribute;
					break;
				}
			}

			if (attributeToReturn == null)
			{
				Debug.LogError($"UserAttributes: Could not find attribute. Key:{key}");
				error = AttributesError.KeyNotFound;
				return null;
			}

			error = AttributesError.None;
			return attributeToReturn;
		}

		private bool IsNewKey(string key)
		{
			var isNew = true;
			foreach (var attribute in _customAttributes)
			{
				if (attribute.key == key)
				{
					isNew = false;
					break;
				}
			}
			return isNew;
		}

		private void PushRemoveChanges(Action onSuccess, Action<Error> onError)
		{
			Action onSuccessRemove = () =>
			{
				_removedAttributes.Clear();
				onSuccess?.Invoke();
			};

			string token = Token.Instance;
			string projectId = XsollaSettings.StoreProjectId;
			SdkUserAccountLogic.Instance.RemoveUserAttributes(token, projectId, _removedAttributes, onSuccessRemove, onError);
		}

		private void PushModifiedChanges(Action onSuccess, Action<Error> onError)
		{
			Action onSuccessUpdate = () =>
			{
				_modifiedAttributes.Clear();
				onSuccess?.Invoke();
			};

			string token = Token.Instance;
			string projectId = XsollaSettings.StoreProjectId;
			SdkUserAccountLogic.Instance.UpdateUserAttributes(token, projectId, _modifiedAttributes, onSuccessUpdate, onError);
		}

		private void FinalizePush(Action onSuccess)
		{
			if (_removedAttributes.Count == 0 && _modifiedAttributes.Count == 0)
			{
				_customAttributesOriginal.Clear();
				_customAttributesOriginal.AddRange(_customAttributes);
				onSuccess?.Invoke();
			}
		}

		public enum AttributesError
		{
			None,
			NotInitialized,
			KeyNotFound,
			DuplicateKey,
			IncorrectKey,
			IncorrectValue
		}

		private class UpdateQueueItem
		{
			public Action<List<UserAttribute>> OnGetReadonly;
			public Action<List<UserAttribute>> OnGetCustom;
			public Action<Error> OnError;
		}
	}
}
