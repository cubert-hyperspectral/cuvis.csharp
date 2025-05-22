
using System;

namespace cuvis_net
{

    public class Exporter : System.IDisposable
    {
        internal int handle_ = 0;
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
            cuvis_il.cuvis_exporter_free(pHandle);

            disposed = true;
        }
        protected virtual void Flush()
        {
            cuvis_il.cuvis_exporter_flush(handle_);
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        ~Exporter()
        {
            Dispose(disposing: false);
        }



        public Measurement Apply(Measurement mesu)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_exporter_apply(handle_, mesu.handle_))
            {
                throw new SDK_Exception();
            }
            if(cuvis_status_t.status_ok != cuvis_il.cuvis_exporter_flush(handle_))
            {
                throw new SDK_Exception();
            }
            return mesu;
        }
    }


    public class CubeExporter : Exporter
    {
        public CubeExporter(GeneralExportSettings ge, SaveArgs fs)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_exporter_create_cube(pHandle, ge.GetInternal(), fs.GetInternal()))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }
    }

    public class TiffExporter : Exporter
    {
        public TiffExporter(GeneralExportSettings ge, TiffExportSettings fs)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_exporter_create_tiff(pHandle, ge.GetInternal(), fs.GetInternal()))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }
    }

    public class EnviExporter : Exporter
    {
        public EnviExporter(GeneralExportSettings ge)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_exporter_create_envi(pHandle, ge.GetInternal()))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }
    }

    public class ViewExporter : Exporter
    {
        public ViewExporter(GeneralExportSettings ge, ViewExportSettings fs)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_exporter_create_view(pHandle, ge.GetInternal(), fs.GetInternal()))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }
    }
}