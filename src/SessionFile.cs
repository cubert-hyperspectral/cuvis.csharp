
using System;

namespace cuvis_net
{
    public class SessionFile : System.IDisposable
    {
        internal int handle_ = 0;

        public SessionFile(string path)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_session_file_load(path, pHandle))
            {
                throw new SDK_Exception();
            }

            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public Measurement GetMeasurement(int frameNo, SessionItemType type = SessionItemType.Frames)
        {
            var pHandle = cuvis_il.new_p_int();
            var ret = cuvis_il.cuvis_session_file_get_mesu(handle_, frameNo, (cuvis_session_item_type_t)type, pHandle);
            if (ret == cuvis_status_t.status_no_measurement) 
            {
                return null;
            }
            if (cuvis_status_t.status_ok != ret)
            {
                throw new SDK_Exception();
            }
            var mesuHandle = cuvis_il.p_int_value(pHandle);
            return new Measurement(mesuHandle);
        }

        public Measurement GetReferenceMeasurement(int frameNo, ReferenceType type)
        {
            var pHandle = cuvis_il.new_p_int();
            var ret = cuvis_il.cuvis_session_file_get_reference_mesu(handle_, frameNo, (cuvis_reference_type_t)type, pHandle);
            if (ret == cuvis_status_t.status_no_measurement)
            {
                return null;
            }
            if (cuvis_status_t.status_ok != ret)
            {
                throw new SDK_Exception();
            }
            var mesuHandle = cuvis_il.p_int_value(pHandle);
            return new Measurement(mesuHandle);
        }


        public int GetSize(SessionItemType type = SessionItemType.Frames)
        {

            SWIGTYPE_p_int val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_session_file_get_size(handle_, (cuvis_session_item_type_t)type, val))
            {
                throw new SDK_Exception();
            }
            return cuvis_il.p_int_value(val);

        }

        public Measurement this[int index]
        {
            get
            {
                return GetMeasurement(index);
            }

        }

        public int Length 
        {
            get 
            {
                return GetSize();
            }
        }

        public double FPS
        {
            get
            {
                SWIGTYPE_p_double val = cuvis_il.new_p_double();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_session_file_get_fps(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return cuvis_il.p_double_value(val);
            }
        }

        public string Hash
        {
            get { return cuvis_il.cuvis_session_file_get_hash_swig(handle_); }
        }

        public OperationMode OperationMode
        {
            get
            {
                SWIGTYPE_p_cuvis_operation_mode_t val = cuvis_il.new_p_cuvis_operation_mode_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_session_file_get_operation_mode(handle_, val))
                {
                    throw new SDK_Exception();
                }
                return (OperationMode)cuvis_il.p_cuvis_operation_mode_t_value(val);
            }
        }


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

            var pHandle = cuvis_il.new_p_int();
            cuvis_il.p_int_assign(pHandle, handle_);
            cuvis_il.cuvis_session_file_free(pHandle);

            disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        ~SessionFile()
        {
            Dispose(disposing: false);
        }

    }
}