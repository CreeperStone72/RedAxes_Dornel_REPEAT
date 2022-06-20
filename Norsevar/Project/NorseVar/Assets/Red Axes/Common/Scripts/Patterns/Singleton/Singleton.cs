using UnityEngine;

namespace Norsevar
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {

        #region Constants and Statics

        private static T _instance;

        #endregion

        #region Properties

        public static T Instance => GetInstance();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (_instance != this && _instance != null)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        private void OnDestroy()
        {
            if (_instance != this)
                return;
            _instance = null;
        }

        #endregion

        #region Private Methods

        private static void CreateInstance()
        {
            string name = typeof(T).ToString();
            string[] split = name.Split('.');
            _instance = new GameObject(split[^1] + " Singleton").AddComponent<T>();
        }

        private static void FindInstance()
        {
            _instance = FindObjectOfType<T>();
        }

        private static T GetInstance()
        {
            if (_instance != null)
                return _instance;

            FindInstance();

            if (_instance == null)
                CreateInstance();

            return _instance;
        }

        #endregion

        #region Protected Methods

        protected abstract void OnAwake();

        #endregion

    }
}
