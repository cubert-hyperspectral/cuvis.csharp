
using System;
using System.Collections.Generic;
using System.Threading;

namespace cuvis_net
{


    public class AcquistionContext : System.IDisposable
    {
        private bool stateCheckRun = false;
        private Thread stateCheckThread = null;

        internal int handle_ = 0;

        public AcquistionContext(Calibration calib)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_create_from_calib(calib.handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public AcquistionContext(SessionFile sess, bool simulate)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_create_from_session_file(sess.handle_, simulate ? 1 : 0, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public SessionData SessionData
        {
            set
            {
                cuvis_session_info_t session = new cuvis_session_info_t();
                session.name = value.Name;
                session.sequence_no = value.SequenceNumber;
                session.session_no = value.SessionNumber;
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_set_session_info(handle_, session))
                {
                    throw new SDK_Exception();
                }
            }
            get
            {
                cuvis_session_info_t session = new cuvis_session_info_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_get_session_info(handle_, session))
                {
                    throw new SDK_Exception();
                }
                return new SessionData(session.name, session.session_no, session.sequence_no);

            }
        }

        #region Informational Properties

        public HardwareState State
        {
            get
            {
                SWIGTYPE_p_cuvis_hardware_state_t val = cuvis_il.new_p_cuvis_hardware_state_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_get_state(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return (HardwareState)cuvis_il.p_cuvis_hardware_state_t_value(val);
            }
        }

        public int QueueSize
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_queue_size_get(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val);
            }
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_queue_size_set(handle_, value))
                {
                    throw new SDK_Exception();
                }
            }
        }

        public int QueueUsed
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_queue_used_get(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val);
            }
        }

        public int ComponentCount
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_get_component_count(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val);
            }
        }

        public bool HasNextMeasurement
        {
            get
            {
                var val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_has_next_measurement(handle_, val))
                {
                    throw new SDK_Exception();
                }
                int value = cuvis_il.p_int_value(val);
                return value != 0;
            }
        }

        public Measurement GetNextMeasurement(int timeout_ms)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_get_next_measurement(handle_, val, timeout_ms))
            {
                throw new SDK_Exception();
            }
            return new Measurement(cuvis_il.p_int_value(val));
        }

        public int GetTemperature(int id)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_temperature_get(handle_, id, val))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(val);
            return value;
        }

        public int GetBandwidth(int id)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_bandwidth_get(handle_, id, val))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(val);
            return value;
        }

        public int GetDriverQueueSize(int id)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_driver_queue_size_get(handle_, id, val))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(val);
            return value;
        }

        public int GetDriverQueueUsed(int id)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_driver_queue_used_get(handle_, id, val))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(val);
            return value;
        }


        public int GetHardwareQueueSize(int id)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_hardware_queue_size_get(handle_, id, val))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(val);
            return value;
        }

        public int GetHardwareQueueUsed(int id)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_hardware_queue_used_get(handle_, id, val))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(val);
            return value;
        }



        #endregion

        #region Aquisition Properties

        public double FPS
        {
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_fps_set(handle_, value))
                {
                    throw new SDK_Exception();
                }
            }
            get
            {
                SWIGTYPE_p_double val = cuvis_il.new_p_double();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_fps_get(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_double_value(val);
            }
        }
        public Async SetFPSAsync(double value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_fps_set_async(handle_, pAsync, value))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }



        public bool PreviewMode
        {
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_preview_mode_set(handle_, value ? 1 : 0))
                {
                    throw new SDK_Exception();
                }
            }
            get
            {
                SWIGTYPE_p_int val = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_preview_mode_get(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_int_value(val) > 0;
            }
        }
        public Async PreviewModeAsync(bool value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_preview_mode_set_async(handle_, pAsync, value ? 1 : 0))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }

        public double IntegrationTime
        {
            set
            {
                cuvis_il.cuvis_acq_cont_integration_time_set(handle_, value);
            }
            get
            {
                SWIGTYPE_p_double val = cuvis_il.new_p_double();
                cuvis_il.cuvis_acq_cont_integration_time_get(handle_, val);
                return cuvis_il.p_double_value(val);
            }
        }

        public Async SetIntegrationTimeAsync(double value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_integration_time_set_async(handle_, pAsync, value))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }

        public bool AutoExposure
        {
            set
            {
                cuvis_il.cuvis_acq_cont_auto_exp_set(handle_, value ? 1 : 0);
            }
            get
            {
                SWIGTYPE_p_int val = cuvis_il.new_p_int();
                cuvis_il.cuvis_acq_cont_auto_exp_get(handle_, val);
                return cuvis_il.p_int_value(val) > 0;
            }
        }

        public Async SetAutoExposureAsync(bool value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_auto_exp_set_async(handle_, pAsync, value ? 1 : 0))
            {
                throw new SDK_Exception();
            }
            return new Async(cuvis_il.p_int_value(pAsync));
        }

        public double AutoExposureComp
        {
            set
            {
                cuvis_il.cuvis_acq_cont_auto_exp_comp_set(handle_, value);
            }
            get
            {
                SWIGTYPE_p_double val = cuvis_il.new_p_double();
                cuvis_il.cuvis_acq_cont_auto_exp_comp_get(handle_, val);
                return cuvis_il.p_double_value(val);
            }
        }

        public Async SetAutoExposureCompAsync(double value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_auto_exp_comp_set_async(handle_, pAsync, value))
            {
                throw new SDK_Exception();
            }
            return new Async(cuvis_il.p_int_value(pAsync));
        }


        public OperationMode OperationMode
        {
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_operation_mode_set(handle_, (cuvis_operation_mode_t)value))
                {
                    throw new SDK_Exception();
                }
            }
            get
            {
                SWIGTYPE_p_cuvis_operation_mode_t val = cuvis_il.new_p_cuvis_operation_mode_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_operation_mode_get(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return (OperationMode)cuvis_il.p_cuvis_operation_mode_t_value(val);
            }
        }

        public Async SetOperationModeAsync(OperationMode value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_operation_mode_set_async(handle_, pAsync, (cuvis_operation_mode_t)value))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }

        public int Average
        {
            set
            {
                cuvis_il.cuvis_acq_cont_average_set(handle_, value);
            }
            get
            {
                var val = cuvis_il.new_p_int();
                cuvis_il.cuvis_acq_cont_average_get(handle_, val);
                return cuvis_il.p_int_value(val);
            }
        }

        public Async SetAverageAsync(int value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_average_set_async(handle_, pAsync, value))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }

        public bool Continuous
        {
            set
            {
                cuvis_il.cuvis_acq_cont_continuous_set(handle_, value ? 1 : 0);
            }
        }

        public Async SetContinuousAsync(bool value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_continuous_set_async(handle_, pAsync, value ? 1 : 0))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }

        public ComponentInfo GetComponentInfo(int id)
        {
            var val = cuvis_il.new_p_int();
            cuvis_component_info_t ci = new cuvis_component_info_t();
            cuvis_il.cuvis_acq_cont_get_component_info(handle_, id, ci);
            return new ComponentInfo(ci);
        }

        public bool GetOnline(int id)
        {
            var val = cuvis_il.new_p_int();
            cuvis_il.cuvis_comp_online_get(handle_, id, val);
            int value = cuvis_il.p_int_value(val);
            return value == 1;
        }

        public double GetGain(int id)
        {
            var val = cuvis_il.new_p_double();
            cuvis_il.cuvis_comp_gain_get(handle_, id, val);
            return cuvis_il.p_double_value(val);
        }

        public void SetGain(int id, double value)
        {
            cuvis_il.cuvis_comp_gain_set(handle_, id, value);
        }


        public Async SetGainAsync(int id, double value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_gain_set_async(handle_, id, pAsync, value))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }
        public double GetIntegrationTimeFactor(int id)
        {
            var val = cuvis_il.new_p_double();
            cuvis_il.cuvis_comp_integration_time_factor_get(handle_, id, val);
            return cuvis_il.p_double_value(val);
        }

        public void SetIntegrationTimeFactor(int id, double value)
        {
            cuvis_il.cuvis_comp_integration_time_factor_set(handle_, id, value);
        }

        public Async SetIntegrationTimeFactorAsync(int id, double value)
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_comp_integration_time_factor_set_async(handle_, id, pAsync, value))
            {
                throw new SDK_Exception();
            }

            return new Async(cuvis_il.p_int_value(pAsync));
        }

        #endregion

        public AsyncMesu Capture()
        {
            var pAsync = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_acq_cont_capture_async(handle_, pAsync))
            {
                throw new SDK_Exception();
            }
            AsyncMesu am = new AsyncMesu(cuvis_il.p_int_value(pAsync));
            return am;
        }


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
            cuvis_il.cuvis_acq_cont_free(pHandle);
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

        ~AcquistionContext()
        {
            Dispose(disposing: false);
        }


        #region State Callback

        public void RegisterStateChangeCallback(StateCallback callback, bool outputInitial)
        {
            ResetStateChangeCallback();

            CheckState cs = new CheckState(callback, this, outputInitial);

            stateCheckRun = true;

            stateCheckThread = new Thread(new ThreadStart(cs.Process));
            stateCheckThread.Start();
        }

        public void ResetStateChangeCallback()
        {
            stateCheckRun = false;
            if (stateCheckThread != null)
            {
                stateCheckThread.Join();
            }
        }

        public delegate void StateCallback(HardwareState state, Dictionary<int, ComponentStateInfo> component_state_info);
        private class CheckState
        {
            StateCallback callback;
            AcquistionContext parent;
            bool outputInitial;

            public CheckState(StateCallback callback, AcquistionContext parent, bool outputInital)
            {
                this.callback = callback;
                this.parent = parent;
                this.outputInitial = outputInitial;
            }
            public void Process()
            {
                HardwareState last_state = HardwareState.Offline;
                Dictionary<int, ComponentStateInfo> lastComponentStates = new Dictionary<int, ComponentStateInfo>();

                for (int i = 0; i < parent.ComponentCount; i++)
                {
                    ComponentInfo info = parent.GetComponentInfo(i);
                    lastComponentStates.Add(i, new ComponentStateInfo(info.DisplayName, false));
                }

                int pollTimeMs = 500;

                bool firstPending = outputInitial;
                while (parent.stateCheckRun)
                {
                    bool stateChanged = firstPending;
                    firstPending = false;

                    HardwareState currentState = parent.State;
                    if (last_state != currentState)
                    {
                        stateChanged = true;
                        last_state = currentState;
                    }
                    for (int k = 0; k < parent.ComponentCount; k++)
                    {
                        bool comp_state = parent.GetOnline(k);
                        bool last_comp_state = lastComponentStates[k].Online;

                        if (comp_state != last_comp_state)
                        {
                            stateChanged = true;
                            lastComponentStates[k].Online = comp_state;
                        }

                    }
                    if (stateChanged)
                    {
                        callback(last_state, lastComponentStates);
                    }
                    else
                    {
                        Thread.Sleep(pollTimeMs);
                    }

                }
            }


        }
        #endregion


        #region Component

        public IEnumerable<Component> Components 
        {
            get {
                List<Component> components = new List<Component>();
                for (int i = 0; i < ComponentCount; i++) 
                {
                    components.Add(new Component(this, i));
                }
                return components;
            }
        }

        #endregion
    }


    public class Component
    {
        private AcquistionContext _acq;
        private int _idx;
        internal Component(AcquistionContext acq, int idx)
        {
            _acq = acq;
            _idx = idx;
        }

        public bool Online
        {
            get
            {
                return _acq.GetOnline(_idx);
            }
        }

        public int Temperature
        {
            get
            {
                return _acq.GetTemperature(_idx);
            }
        }

        public int Bandwidth
        {
            get
            {
                return _acq.GetBandwidth(_idx);
            }
        }

        public double Gain
        {
            get
            {
                return _acq.GetGain(_idx);
            }
            set
            {
                _acq.SetGain(_idx, value);
            }
        }

        public double IntegrationTimeFactor
        {
            get
            {
                return _acq.GetIntegrationTimeFactor(_idx);
            }
            set
            {
                _acq.SetIntegrationTimeFactor(_idx, value);
            }
        }

        public ComponentInfo Info 
        {
            get 
            {
                return _acq.GetComponentInfo(_idx);
            }
        }
    }
}