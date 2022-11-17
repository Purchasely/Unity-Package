using System;
using System.Collections.Generic;
using UnityEngine;

public class AsyncCallbackHelper : MonoBehaviour
{
	private static AsyncCallbackHelper _instance;
	private static readonly object InitLock = new object();
	private readonly object _queueLock = new object();
	private readonly List<Action> _queuedActions = new List<Action>();
	private readonly List<Action> _executingActions = new List<Action>();

	public static AsyncCallbackHelper Instance
	{
		get
		{
			if (_instance == null)
			{
				Init();
			}

			return _instance;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		lock (InitLock)
		{
			if (!ReferenceEquals(_instance, null))
			{
				return;
			}

			var instances = FindObjectsOfType<AsyncCallbackHelper>();

			if (instances.Length > 1)
			{
				Debug.LogError(typeof(AsyncCallbackHelper) + " Something went really wrong " +
				               " - there should never be more than 1 " + typeof(AsyncCallbackHelper) +
				               " Reopening the scene might fix it.");
			}
			else if (instances.Length == 0)
			{
				var singleton = new GameObject();
				_instance = singleton.AddComponent<AsyncCallbackHelper>();
				singleton.name = "AsyncCallbackHelper";
				singleton.hideFlags = HideFlags.HideAndDontSave;

				DontDestroyOnLoad(singleton);

				Debug.Log("[Singleton] An _instance of " + typeof(AsyncCallbackHelper) +
				          " is needed in the scene, so '" + singleton.name +
				          "' was created with DontDestroyOnLoad.");
			}
			else
			{
				Debug.Log("[Singleton] Using _instance already created: " + _instance.gameObject.name);
			}
		}
	}

	internal void Queue(Action action)
	{
		if (action == null)
		{
			Debug.LogWarning("Trying to queue null action");
			return;
		}

		lock (Instance._queueLock)
		{
			Instance._queuedActions.Add(action);
		}
	}

	private void Update()
	{
		MoveQueuedActionsToExecuting();

		while (_executingActions.Count > 0)
		{
			var action = _executingActions[0];
			_executingActions.RemoveAt(0);
			action();
		}
	}

	private void MoveQueuedActionsToExecuting()
	{
		lock (_queueLock)
		{
			while (_queuedActions.Count > 0)
			{
				var action = _queuedActions[0];
				_executingActions.Add(action);
				_queuedActions.RemoveAt(0);
			}
		}
	}
}