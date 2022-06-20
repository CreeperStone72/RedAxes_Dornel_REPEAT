using System;
using System.Collections.Generic;
using FMODUnity;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar.VFX
{
    [CreateAssetMenu(fileName = "NorseGameEvent", menuName = "Norsevar/Events/NorseGame")]
    public class NorseGameEvent : ScriptableObject
    {
        #region Private Fields

        private readonly List<IGameEventListener> _listeners = new();
        private readonly List<Action> _actions = new();

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("Sound")] public bool sound;

        [FormerlySerializedAs("FMODEvent")] [ShowIf("sound")]
        public EventReference fmodEvent;

        [FormerlySerializedAs("VFX")] public bool vfx;

        [FormerlySerializedAs("_vfxOffset")] [ShowIf("vfx")]
        public Vector3 vfxOffset;

        [FormerlySerializedAs("_vfxPrefab")] [ShowIf("vfx")]
        public GameObject vfxPrefab;

        [ShowIf("vfx")] public float vfxTime;

        public bool timeScale;
        [ShowIf("timeScale")] public TimeScaleDataSO timeScaleData;

        public bool playerSay;
        [ShowIf("playerSay")] public string playerSayText;

        public bool startDialogue;
        [ShowIf("startDialogue")] public string dialogueId;

        #endregion

        #region Public Methods

        [ContextMenu("Raise Event")]
        public virtual void Raise(params object[] args)
        {
            GameObject vfxInstance = null;
            if (vfx) vfxInstance = NorseGame.Instance.Get<ObjectPooler>().PoolVFX(vfxPrefab, vfxTime);

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case GameObject o:
                    {
                        if (sound)
                            RuntimeManager.PlayOneShot(fmodEvent, o.transform.position);
                        if (vfx)
                        {
                            if (vfxInstance != null)
                                vfxInstance.transform.position = o.transform.position + vfxOffset;
                        }

                        break;
                    }
                    case Vector3 v:
                    {
                        if (vfx)
                        {
                            if (vfxInstance != null)
                                vfxInstance.transform.position = v;
                        }

                        if (sound) RuntimeManager.PlayOneShot(fmodEvent, v);
                        break;
                    }
                }
            }
            
            if (sound) RuntimeManager.PlayOneShot(fmodEvent);
            
            if (vfx)
            {
                if (vfxInstance != null)
                    vfxInstance.SetActive(true);
            }

            if (timeScale)
                SlowMoManager.Instance.SlowTime(timeScaleData);

            if (playerSay)
                NorseGame.Instance.Get<PlayerSayerBehaviour>().Say(playerSayText);

            if (startDialogue)
                DialogueManager.instance.StartConversation(dialogueId);

            for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i].OnEventRaised();
            for (int i = _actions.Count - 1; i >= 0; i--) _actions[i].Invoke();
        }

        public virtual void RaiseNoParams()
        {
            Raise();
        }

        public void RegisterListener(IGameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
        
        public void RegisterAction(Action action)
        {
            if (!_actions.Contains(action))
                _actions.Add(action);
        }
        
        public void UnregisterAction(Action action)
        {
            if (_actions.Contains(action))
                _actions.Remove(action);
        }

        #endregion
    }
}