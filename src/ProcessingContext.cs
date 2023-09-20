using System;

namespace cuvis_net
{


    public class ProcessingContext : System.IDisposable
    {
        cuvis_proc_args_t modeArgs_ = new cuvis_proc_args_t();

        internal int handle_ = 0;

        public ProcessingContext(Calibration calib)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_create_from_calib(calib.handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public ProcessingContext(SessionFile session)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_create_from_session_file(session.handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public ProcessingContext(Measurement mesu)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_create_from_mesu(mesu.handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public Measurement Apply(Measurement mesu)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_apply(handle_, mesu.handle_))
            {
                throw new SDK_Exception();
            }
            mesu.Refresh();
            return mesu;
        }

        public bool CalcDistance(double distMM)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_calc_distance(handle_, distMM))
            {
                throw new SDK_Exception();
            }
            return true;
        }

        public bool Recalib
        {
            set
            {
                modeArgs_.allow_recalib = value ? 1 : 0;
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_set_args(handle_, modeArgs_))
                {
                    throw new SDK_Exception();
                }
            }
            get
            {
                return (modeArgs_.allow_recalib != 0);
            }
        }

        public ProcessingMode ProcessingMode
        {
            set
            {
                modeArgs_.processing_mode = (cuvis_processing_mode_t)value;
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_set_args(handle_, modeArgs_))
                {
                    throw new SDK_Exception();
                }
            }
            get
            {
                return (ProcessingMode)modeArgs_.processing_mode;
            }
        }

        public void SetReference(Measurement mesu, ReferenceType referenceType)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_set_reference(handle_, mesu.handle_, (cuvis_reference_type_t)referenceType))
            {
                throw new SDK_Exception();
            }
        }
		
		public void ClearReference(ReferenceType referenceType)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_clear_reference(handle_, (cuvis_reference_type_t)referenceType))
            {
                throw new SDK_Exception();
            }
        }

        public Measurement GetReference(ReferenceType referenceType)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_get_reference(handle_, pHandle, (cuvis_reference_type_t)referenceType))
            {
                throw new SDK_Exception();
            }
            var mesuHandle = cuvis_il.p_int_value(pHandle);
            return new Measurement(mesuHandle);
        }

        public bool HasReference(ReferenceType referenceType)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_has_reference(handle_, (cuvis_reference_type_t)referenceType, pHandle))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(pHandle);
            return value == 1;
        }

        //mark/todo add procMode Argument
        public bool IsCapable(Measurement mesu, ProcessingMode processingMode, bool allowRecalib)
        {
            cuvis_proc_args_t args = new cuvis_proc_args_t();
            args.processing_mode = (cuvis_processing_mode_t)processingMode;
            args.allow_recalib = allowRecalib ? 1 : 0;
            
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_proc_cont_is_capable(handle_, mesu.handle_, args, pHandle))
            {
                throw new SDK_Exception();
            }
            int value = cuvis_il.p_int_value(pHandle);
            return value == 1;
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
            cuvis_il.cuvis_proc_cont_free(pHandle);
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

        ~ProcessingContext()
        {
            Dispose(disposing: false);
        }


        public string CalibrationID
        {
            get { return cuvis_il.cuvis_proc_cont_get_calib_id_swig(handle_); }
        }
    }


}