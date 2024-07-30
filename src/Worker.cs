
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

        public void RegisterWorkerCallback(WorkerCallback callback, uint concurrency)
        {
            ResetWorkerCallback();

            WorkerCallbackState ws = new WorkerCallbackState(callback, concurrency, this);

            workerThreadRun = true;
            workerThread = new Thread(new ThreadStart(ws.Process));
            workerThread.Start();
        }

        public void ResetWorkerCallback()
        {
            workerThreadRun = false;
            if (workerThread != null)
            {
                workerThread.Join();
            }
        }

        private class WorkerCallbackState
        {
            WorkerCallback callback;
            uint concurrency;
            Worker parent;

            public WorkerCallbackState(WorkerCallback callback, uint concurrency, Worker parent)
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

        #region Getter / Setter

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

        System.Tuple<Measurement, ViewResult> GetNextMeasurement(ulong timeout_ms = 0)
        {
            var cur_mesu = cuvis_il.new_p_int();
            var cur_view = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_next_result(handle_, cur_mesu, cur_view, timeout_ms))
            {
                throw new SDK_Exception();
            }
            Measurement mesu = new Measurement(cuvis_il.p_int_value(cur_mesu));
            var view = Viewer.createViewData(cuvis_il.p_int_value(cur_view));
            return new System.Tuple<Measurement, ViewResult>(mesu, view);
        }

        public bool Processing
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_is_processing(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val) != 0;
            }
            set
            {
                if (value)
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_start(handle_))
                    {
                        throw new SDK_Exception();
                    }
                }
                else
                {
                    if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_stop(handle_))
                    {
                        throw new SDK_Exception();
                    }
                }
            }
        }

        public WorkerState State
        {
            get
            {
                var val = cuvis_il.new_p_cuvis_worker_state_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_state(handle_, val))
                {
                    throw new SDK_Exception();
                }

                return new WorkerState(cuvis_il.p_cuvis_worker_state_t_value(val));
            }
        }

        public int ThreadsBusy
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_threads_busy(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val);
            }
        }

        public ulong InputQueueLimit
        {
            get
            {
                var val = cuvis_il.new_p_ulong();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_input_queue_limit(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_ulong_value(val);
            }
        }

        public ulong OutputQueueLimit
        {
            get
            {
                var val = cuvis_il.new_p_ulong();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_output_queue_limit(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_ulong_value(val);
            }
        }
        public ulong MandatoryQueueLimit
        {
            get
            {
                var val = cuvis_il.new_p_ulong();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_mandatory_queue_limit(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_ulong_value(val);
            }
        }
        public ulong SupplementaryQueueLimit
        {
            get
            {
                var val = cuvis_il.new_p_ulong();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_supplementary_queue_limit(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_ulong_value(val);
            }
        }

        public bool CanDropResults
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_can_drop_results(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val) != 0;
            }
        }
        public bool CanSkipMeasurments
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_can_skip_measurements(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val) != 0;
            }
        }
        public bool CanSkipSupplementarySteps
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_worker_get_can_skip_supplementary(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val) != 0;
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
                // Free any other managed objects here.
                //
            }

            ResetWorkerCallback();

            var pHandle = cuvis_il.new_p_int();
            cuvis_il.cuvis_worker_stop(handle_);
            cuvis_il.cuvis_worker_drop_all_queued(handle_);
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