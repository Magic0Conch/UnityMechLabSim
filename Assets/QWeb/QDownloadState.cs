using System.Collections.Generic;
using UnityEngine;

namespace QWeb
{
    /// <summary>
    /// 不应该调用
    /// </summary>
    class QDownloadState : MonoBehaviour
    {

        static QDownloadState instance;
        static QDownloadState Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("QDownloadState").AddComponent<QDownloadState>();
                }
                return instance;
            }
        }

        QDownloadFile qDownloadFile;
        List<QAbstractWeb> list = new List<QAbstractWeb>();
        
        public static void Add(QAbstractWeb value)
        {
            Instance.list.Add(value);
        }

        public static void Clear()
        {
            Instance.list.Clear();
        }

        private void Update()
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsOk)
                {
                    list[i].SetCompleted();
                    list.RemoveAt(i);
                    break;
                }
                qDownloadFile = list[i] as QDownloadFile;
                if (qDownloadFile != null && qDownloadFile.update)
                {
                    qDownloadFile.SetUpdate();
                }
            }
        }

        private void OnDestroy()
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                list[i].Cancel();
            }
        }
    }
}   