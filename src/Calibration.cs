
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace cuvis_net
{
    public class Calibration : System.IDisposable
    {
        internal int handle_ = 0;

        public Calibration(string path)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_calib_create_from_path(path, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public Calibration(SessionFile session)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_calib_create_from_session_file(session.handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
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
            cuvis_il.cuvis_calib_free(pHandle);

            disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        ~Calibration()
        {
            Dispose(disposing: false);
        }

        public IEnumerable<Capability> GetCapabilities(OperationMode mode)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_calib_get_capabilities(handle_, (cuvis_operation_mode_t)mode, pHandle))
            {
                throw new SDK_Exception();
            }
            int capabilityBitmap = cuvis_il.p_int_value(pHandle);
            return CapabilityConversion.FromBitset(capabilityBitmap);
        }

        public string ID
        {
            get { return cuvis_il.cuvis_calib_get_id_swig(handle_); }
        }

        public cuvis_calibration_info_t Info
        {
            get {
                var info = new cuvis_calibration_info_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_calib_get_info(handle_, info))
                {
                    throw new SDK_Exception();
                }
                return info;
            }
        }
    }


}