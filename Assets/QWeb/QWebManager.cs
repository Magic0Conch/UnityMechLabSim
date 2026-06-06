using System.Collections.Generic;
using UnityEngine;
namespace QWeb
{
    /// <summary>
    /// 上传下载队列
    /// </summary>
    public class QWebManager : MonoBehaviour
    {

        static QWebManager instance;
        static QWebManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("QWebPool").AddComponent<QWebManager>();
                }
                return instance;
            }
        }

        int current = 0;
        QAbstractWeb[] pool = new QAbstractWeb[1];
        List<QAbstractWeb> queue = new List<QAbstractWeb>();

        public static void SetMax(int max)
        {
            Instance.pool = new QAbstractWeb[max];
        }

        public static int GetTotalSize() { return Instance.pool.Length; }

        public static QAbstractWeb Add(QAbstractWeb web)
        {
            bool isStart = false;
            if (Instance.current < instance.pool.Length)
            {
                for (int i = 0; i < instance.pool.Length; i++)
                {
                    if (instance.pool[i] == null)
                    {
                        instance.pool[i] = web;
                        web.Start();
                        isStart = true;
                        Instance.current++;
                        break;
                    }
                }
            }

            if (!isStart) instance.queue.Add(web);
            return web;
        }

        public static QWebManager Remove(QAbstractWeb web)
        {
            if (Instance.queue.Contains(web))
            {
                instance.queue.Remove(web);
            }
            else
            {
                for(int i = 0; i < instance.pool.Length; i++)
                {
                    if(web == instance.pool[i])
                    {
                        instance.pool[i] = null;
                        web.Cancel();
                        Instance.current--;
                        break;
                    }
                }
            }
            
            return instance;
        }

        public static QWebManager Clear()
        {
            Instance.queue.Clear();
            for (int i = 0; i < instance.pool.Length; i++)
            {
                if (instance.pool[i] != null)
                {
                    instance.pool[i].Cancel();
                }
            }
            instance.current = 0;
            return instance;
        }

        private void Update()
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] != null && pool[i].IsOk)
                {
                    pool[i] = null;
                    current--;
                    if (queue.Count > 0)
                    {
                        pool[i] = queue[0];
                        queue.RemoveAt(0);
                        pool[i].Start();
                        current++;
                    }
                }
            }
        }
    }
}