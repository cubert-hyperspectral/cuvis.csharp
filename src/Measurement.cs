
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;

namespace cuvis_net
{


    public class Measurement : System.IDisposable, System.ICloneable
    {


        internal int handle_ = 0;
        private cuvis_mesu_metadata_t metaData_;
        private System.Collections.Generic.Dictionary<string, System.Lazy<Data>> dataMap_;
        private GeoCoordinate gpsData_;
        private Bitmap preview_image_;

        public Measurement(string path)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_load(path, pHandle))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);

            metaData_ = cuvis_il.cuvis_mesu_metadata_allocate();

            Refresh();
        }

        internal Measurement(int handle)
        {
            handle_ = handle;

            metaData_ = cuvis_il.cuvis_mesu_metadata_allocate();

            Refresh();
        }

        internal ImageData<T> LoadData<T>(string key)
        {
            var pBuf = cuvis_il.cuvis_imbuffer_allocate();
            cuvis_il.cuvis_measurement_get_data_image(handle_, key, pBuf);
            var result = new ImageData<T>(pBuf);
            cuvis_il.cuvis_imbuffer_free(pBuf);
            return result;
        }

        internal void Refresh()
        {
            dataMap_ = new System.Collections.Generic.Dictionary<string, System.Lazy<Data>>();


            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_get_metadata(handle_, metaData_))
            {
                throw new SDK_Exception();
            }


            var pCount = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_get_data_count(handle_, pCount))
            {
                throw new SDK_Exception();
            }
            for (int k = 0; k < cuvis_il.p_int_value(pCount); k++)
            {
                var pType = cuvis_il.new_p_cuvis_data_type_t();
                string key = cuvis_il.cuvis_measurement_get_data_info_swig(handle_, pType, k);
                var type = cuvis_il.p_cuvis_data_type_t_value(pType);

                if (type == cuvis_data_type_t.data_type_string)
                {
                    var value = cuvis_il.cuvis_measurement_get_data_string_swig(handle_, key);
                    dataMap_.Add(key, new System.Lazy<Data>(() => new StringData { value = value }));

                }
                else if (type == cuvis_data_type_t.data_type_sensor_info)
                {
                    var pBuf = cuvis_il.cuvis_sensor_info_allocate();
                    var value = cuvis_il.cuvis_measurement_get_data_sensor_info(handle_, key, pBuf);
                    dataMap_.Add(key, new System.Lazy<Data>(() => new SensorInfo(pBuf)));
                    cuvis_il.cuvis_sensor_info_free(pBuf);
                }
                else if (type == cuvis_data_type_t.data_type_image)
                {
                    var pBuf = cuvis_il.cuvis_imbuffer_allocate();
                    cuvis_il.cuvis_measurement_get_data_image(handle_, key, pBuf);

                    switch (pBuf.format)
                    {
                        case cuvis_imbuffer_format_t.imbuffer_format_uint8:
                            {
                                dataMap_.Add(key, new System.Lazy<Data>(() => LoadData<byte>(key)));
                                if (key.Equals("preview"))
                                {
                                    preview_image_ = ImageData<byte>.ToGreyscale(new ImageData<byte>(pBuf));
                                }
                                else if (key.Equals("pan") && preview_image_ == null)
                                {
                                    preview_image_ = ImageData<byte>.ToGreyscale(new ImageData<byte>(pBuf));
                                }
                                break;
                            }
                        case cuvis_imbuffer_format_t.imbuffer_format_uint16:
                            {
                                dataMap_.Add(key, new System.Lazy<Data>(() => LoadData<ushort>(key)));
                                break;
                            }
                        case cuvis_imbuffer_format_t.imbuffer_format_uint32:
                            {
                                dataMap_.Add(key, new System.Lazy<Data>(() => LoadData<uint>(key)));
                                break;
                            }
                        case cuvis_imbuffer_format_t.imbuffer_format_float:
                            {
                                dataMap_.Add(key, new System.Lazy<Data>(() => LoadData<float>(key)));
                                break;
                            }

                    }
                    cuvis_il.cuvis_imbuffer_free(pBuf);


                }
                else if (type == cuvis_data_type_t.data_type_gps)
                {
                    var pBuf = cuvis_il.cuvis_gps_allocate();

                    cuvis_il.cuvis_measurement_get_data_gps(handle_, key, pBuf);
                    GeoCoordinate gps = new GeoCoordinate(pBuf.latitude, pBuf.longitude, pBuf.altitude);
                    ulong time = pBuf.time;

                    dataMap_.Add(key, new System.Lazy<Data>(() => new GPSData { coordinate = gps, time = Helper.ToDateTime(time) }));

                    gpsData_ = gps;
                    cuvis_il.cuvis_gps_free(pBuf);
                    //    cuvis_gps_t t;
                    //    var value = cuvis_il.cuvis_measurement_get_data_gps(handle_, key,t);

                }


            }
        }

        public void Save(string path, CubertSaveArgs args)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_save(handle_, path, args.GetInternal()))
            {
                throw new SDK_Exception();
            }
        }

        public IEnumerable<Capability> Capabilities
        {
            get
            {
                var pHandle = cuvis_il.new_p_int();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_get_capabilities(handle_, pHandle))
                {
                    throw new SDK_Exception();
                }
                int capabilityBitmap = cuvis_il.p_int_value(pHandle);
                return CapabilityConversion.FromBitmap(capabilityBitmap);
            }
        }


        #region Measurement Metadata
        public string Name
        {
            get { return metaData_.name; }
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_set_name(handle_, value))
                {
                    throw new SDK_Exception();
                }
                Refresh();
            }
        }
        public string Path { get { return metaData_.path; } }
        public string Comment
        {
            get { return metaData_.comment; }
            set
            {
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_set_comment(handle_, value))
                {
                    throw new SDK_Exception();
                }
                Refresh();
            }
        }
        public System.DateTime CaptureTime { get { return Helper.ToDateTime(metaData_.capture_time); } }
        public System.DateTime FactoryCalibration { get { return Helper.ToDateTime(metaData_.factory_calibration); } }
        public string ProductName { get { return metaData_.product_name; } }
        public string SerialNumber { get { return metaData_.serial_number; } }
        public string Assembly { get { return metaData_.assembly; } }
        public double IntegrationTime { get { return metaData_.integration_time; } }
        public double? Distance { get { double? ret = null; if (metaData_.distance > 0.0) { ret = metaData_.distance; }; return ret; } }
        public int Averages { get { return metaData_.averages; } }

        public GeoCoordinate GPS { get { return gpsData_; } }

        public Bitmap Thumbnail { get { return preview_image_; } }

        public uint MeasurementFlags { get { return metaData_.measurement_flags; } }
        public ProcessingMode ProcessingMode { get { return (ProcessingMode)metaData_.processing_mode; } }
        public SessionData Session
        {
            get
            {
                SessionData sd = new SessionData(metaData_.session_info_name, metaData_.session_info_session_no, metaData_.session_info_sequence_no);
                return sd;
            }
        }

        #endregion

        public System.Collections.Generic.Dictionary<string, System.Lazy<Data>> Data
        {
            get { return dataMap_; }
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
            cuvis_il.cuvis_measurement_free(pHandle);
            handle_ = cuvis_il.p_int_value(pHandle);
            cuvis_il.cuvis_mesu_metadata_free(metaData_);

            disposed = true;
        }

        public void ClearCube()
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_clear_cube(handle_))
            {
                throw new SDK_Exception();
            }
            Refresh();
        }

        public void ClearReference(ReferenceType referenceType)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_clear_implicit_reference(handle_, (cuvis_reference_type_t)referenceType))
            {
                throw new SDK_Exception();
            }
            Refresh();
        }



        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        ~Measurement()
        {
            Dispose(disposing: false);
        }


        public object Clone()
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_measurement_deep_copy(handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            int newHandle = cuvis_il.p_int_value(pHandle);


            return new Measurement(newHandle);
        }

        public string GetCalibrationID
        {
            get { return cuvis_il.cuvis_measurement_get_calib_id_swig(handle_); }
        }
    }


}