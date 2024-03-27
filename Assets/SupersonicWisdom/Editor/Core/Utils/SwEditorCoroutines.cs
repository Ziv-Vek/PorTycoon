using System;
using UnityEditor;
using System.Collections;
using UnityEngine;

internal class SwEditorCoroutines
{
    #region --- Members ---

    private readonly Action _callback;
    private readonly bool _delayed;
    private readonly IEnumerator _routine;
    private float _lastEditorUpdateTime;

    #endregion


    #region --- Construction ---

    public SwEditorCoroutines(IEnumerator routine, bool delayed, Action callback)
    {
        _routine = routine;
        _delayed = delayed;
        _callback = callback;
    }

    #endregion


    #region --- Mono Override ---

    private void Start ()
    {
        EditorApplication.update += Update;
    }

    private void Update ()
    {
        if (_delayed)
        {
            var floatValue = 0f;

            if (_routine.Current != null)
            {
                try
                {
                    floatValue = (float)_routine.Current;
                }
                catch (InvalidCastException)
                {
                    floatValue = 0f;
                }
            }

            if (_lastEditorUpdateTime == 0f)
            {
                if (floatValue > 0f)
                {
                    _lastEditorUpdateTime = Time.realtimeSinceStartup;

                    return;
                }
            }
            else
            {
                if (Time.realtimeSinceStartup - _lastEditorUpdateTime >= floatValue)
                {
                    _lastEditorUpdateTime = 0f;
                }
                else
                {
                    return;
                }
            }
        }

        if (!_routine.MoveNext())
        {
            StopEditorCoroutine();
            _callback?.Invoke();
        }
    }

    #endregion


    #region --- Public Methods ---

    public static SwEditorCoroutines StartDelayedEditorCoroutine(IEnumerator routine)
    {
        return StartEditorCoroutine(routine, true);
    }

    public static SwEditorCoroutines StartEditorCoroutine(IEnumerator routine, bool delayed = false, Action callback = null)
    {
        var coroutine = new SwEditorCoroutines(routine, delayed, callback);
        coroutine.Start();

        return coroutine;
    }

    #endregion


    #region --- Private Methods ---

    private void StopEditorCoroutine ()
    {
        EditorApplication.update -= Update;
    }

    #endregion
}