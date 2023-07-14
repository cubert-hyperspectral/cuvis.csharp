
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cuvis_net
{
    public delegate void WorkerCallback(Measurement mesu, ViewResult view);
    public class Worker : System.IDisposable
    {
        internal int handle_ = 0;

        private bool workerThreadRun = false;
        private Thread workerThread = null;

        public Worker(WorkerArgs args)
        {
            var pHandle = cuvis_il.new_p_int();

            cuvis_worker_settings_t settings = args.GetInternal();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_create(pHandle, settings))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        #region Getter / Setter

        public void RegisterWorkerCallback(WorkerCallback callback, uint concurrency)
        {
            ResetWorkerCallback();

            WorkerState ws = new WorkerState(callback, concurrency, this);

            workerThreadRun = true;
            workerThread = new Thread(new ThreadStart(ws.Process));
            workerThread.Start();
        }

        private class WorkerState
        {
            WorkerCallback callback;
            uint concurrency;
            Worker parent;

            public WorkerState(WorkerCallback callback, uint concurrency, Worker parent)
            {
                this.callback = callback;
                this.concurrency = concurrency;
                this.parent = parent;
            }
            public void Process()
            {
                int pollTimeMs = 10;
                Queue<Task> taskQueue = new Queue<Task>();
                while (parent.workerThreadRun)
                {
                    if (parent.HasNextMeasurement)
                    {
                        var res = parent.GetNextMeasurement();

                        Task t = new Task(() => { callback(res.Item1, res.Item2); });
                        taskQueue.Enqueue(t);
                        t.Start();

                        if (taskQueue.Count >= concurrency)
                        {
                            Task task = taskQueue.Dequeue();
                            while (true)
                            {
                                if (task.Wait(pollTimeMs))
                                {
                                    break;
                                }
                                if (!parent.workerThreadRun)
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Thread.Sleep(pollTimeMs);
                        }
                    }
                }
            }
        }



        public void ResetWorkerCallback()
        {
            workerThreadRun = false;
            if (workerThread != null)
            {
                workerThread.Join();
            }
        }

        AcquistionContext AcquistionContext
        {
            set
            {
                if (value != null)
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_acq_cont(handle_, value.handle_))
                    {
                        throw new SDK_Exception();
                    }
                }
                else
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_acq_cont(handle_, 0))
                    {
                        throw new SDK_Exception();
                    }
                }
            }
        }
        ProcessingContext ProcessingContext
        {
            set
            {
                if (value != null)
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_proc_cont(handle_, value.handle_))
                    {
                        throw new SDK_Exception();
                    }
                }
                else
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_proc_cont(handle_, 0))
                    {
                        throw new SDK_Exception();
                    }
                }
            }
        }

        Exporter Exporter
        {
            set
            {
                if (value != null)
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_exporter(handle_, value.handle_))
                    {
                        throw new SDK_Exception();
                    }
                }
                else
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_exporter(handle_, 0))
                    {
                        throw new SDK_Exception();
                    }
                }
            }
        }

        Viewer Viewer
        {
            set
            {
                if (value != null)
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_viewer(handle_, value.handle_))
                    {
                        throw new SDK_Exception();
                    }
                }
                else
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_viewer(handle_, 0))
                    {
                        throw new SDK_Exception();
                    }
                }
            }
        }

        System.Tuple<Measurement,ViewResult> GetNextMeasurement()
        {
            var cur_mesu = cuvis_il.new_p_int();
            var cur_view = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_next_result(handle_, cur_mesu, cur_view, -1))
            {
                throw new SDK_Exception();
            }
            Measurement mesu = new Measurement(cuvis_il.p_int_value(cur_mesu));
            var view = Viewer.createViewData(cuvis_il.p_int_value(cur_view));
            return new System.Tuple<Measurement, ViewResult>(mesu, view);
        }

        public int QueueHardLimit
        {
            get
            {
                var val = cuvis_il.new_p_int();
                var valb = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_queue_limits(handle_, val, valb))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val);
            }
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_queue_limits(handle_, value, QueueSoftLimit))
                {
                    throw new SDK_Exception();
                }
            }
        }

        public int QueueSoftLimit
        {
            get
            {
                var val = cuvis_il.new_p_int();
                var valb = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_queue_limits(handle_, valb, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val);
            }
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_queue_limits(handle_, QueueHardLimit, value))
                {
                    throw new SDK_Exception();
                }
            }
        }

        public bool CanDrop
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_drop_behavior(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val) != 0;
            }
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_set_drop_behavior(handle_, value ? 1 : 0))
                {
                    throw new SDK_Exception();
                }
            }
        }


        bool HasNextMeasurement
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_has_next_result(handle_, val))
                {
                    throw new SDK_Exception();
                }
                int value = cuvis_il.p_int_value(val);
                return value != 0;
            }
        }


        #endregion


        bool disposed = false;


        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Dispose();
                // Free any other managed objects here.
                //
            }

            var pHandle = cuvis_il.new_p_int();
            cuvis_il.p_int_assign(pHandle, handle_);
            cuvis_il.cuvis_worker_free(pHandle);
            handle_ = cuvis_il.p_int_value(pHandle);

            disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        ~Worker()
        {
            Dispose(disposing: false);
        }

    }
}