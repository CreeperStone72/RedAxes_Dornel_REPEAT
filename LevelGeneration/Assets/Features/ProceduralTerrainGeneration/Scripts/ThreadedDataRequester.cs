namespace ProceduralTerrainGeneration {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;

    public class ThreadedDataRequester : MonoBehaviour {
        private static ThreadedDataRequester _instance;
        private readonly Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

        private void Awake() { _instance = FindObjectOfType<ThreadedDataRequester>(); }

        private void Update() {
            lock (_dataQueue) {
                if (_dataQueue.Count <= 0) return;
                
                for (var i = 0; i < _dataQueue.Count; i++) {
                    var threadInfo = _dataQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        public static void RequestData(Func<object> generateData, Action<object> callback) {
            void ThreadStart() { _instance.DataThread(generateData, callback); }
            new Thread(ThreadStart).Start();
        }

        private void DataThread(Func<object> generateData, Action<object> callback) {
            var data = generateData();
            lock (_dataQueue) { _dataQueue.Enqueue(new ThreadInfo(callback, data)); }
        }

        private struct ThreadInfo {
            public readonly Action<object> callback;
            public readonly object parameter;

            public ThreadInfo(Action<object> callback, object parameter) {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
}