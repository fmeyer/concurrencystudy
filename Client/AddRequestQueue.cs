using System;
using System.Collections.Generic;
using System.Threading;

namespace Client
{
    public class AddRequestQueue : IDisposable 
    {
        private readonly object _locker = new object();
        private readonly Thread[] _workers;
        private readonly Queue<Action> _requestsQueue = new Queue<Action>();

        public AddRequestQueue(int workerCount)
        {
            _workers = new Thread[workerCount];
                        
            for (var i = 0; i < workerCount; i++)
                (_workers[i] = new Thread(Consume)).Start();
        }

        public void Dispose()
        {
            // Enqueue null task per worker to gracefully shutdown
            foreach (var unused in _workers) Enqueue(null);
            foreach (var worker in _workers) worker.Join();
        }

        public void Enqueue(Action task)
        {
            lock (_locker)
            {
                _requestsQueue.Enqueue(task);
                Monitor.PulseAll(_locker);
            }
        }
        

        private void Consume()
        {
            while (true)
            {
                Action task;
                lock (_locker)
                {
                    while (_requestsQueue.Count == 0) Monitor.Wait(_locker);
                    task = _requestsQueue.Dequeue();
                }
                
                if (task == null) return; // exit on null tasks
                task();

                var currentThreadManagedThreadId = Thread.CurrentThread.ManagedThreadId;
                
                Console.WriteLine($"processing with thread {currentThreadManagedThreadId}");
            }
        }
    }
}