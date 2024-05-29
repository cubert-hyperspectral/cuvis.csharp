using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace cuvis_net
{
    public class General
    {
        public delegate void Log(string message, LogLevel level);

        // we need to keep a reference to the current internal callback because the garbage collection mustn't remove it.
        private static cuvis_il.LogCallbackLocalized internal_callback;

        public enum LogLevel
        {
            debug = cuvis_loglevel_t.loglevel_debug,
            info = cuvis_loglevel_t.loglevel_info,
            warning = cuvis_loglevel_t.loglevel_warning,
            error = cuvis_loglevel_t.loglevel_error,
            fatal = cuvis_loglevel_t.loglevel_fatal
        };

        private static void RegisterLogCallback(cuvis_il.LogCallbackLocalized callback, string locale, LogLevel min_level)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_register_log_callback_localized(callback, (int)min_level, locale))
            {
                throw new SDK_Exception();
            }
        }

        public static void SetLogger(Log callback, string locale = "", LogLevel min_level = LogLevel.info)
        {
            internal_callback = (m_ptr, l) =>
            {
                LogLevel logLevel = (LogLevel)l;
                string msg = global::System.Runtime.InteropServices.Marshal.PtrToStringUni(m_ptr);
                callback(msg, logLevel);
            };
            RegisterLogCallback(internal_callback, locale, min_level);
        }

        public static void ClearLogCallback()
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_reset_log_callback_localized())
            {
                throw new SDK_Exception();
            }
            internal_callback = null;
        }
        public static string Version
        {
            get
            {
                return cuvis_il.cuvis_version_swig();
            }
        }

        public static void Init(string settings_path, LogLevel global_loglevel = LogLevel.debug)
        {

            if (cuvis_status_t.status_ok != cuvis_il.cuvis_init(settings_path, (int)global_loglevel))
            {
                throw new SDK_Exception();
            }
        }
    }

    public enum async_result_t
    {
        done,
        timeout,
        overwritten,
        deferred
    };
    public class Async : IDisposable
    {
        internal int handle_ = 0;
        internal Async(int handle)
        {
            handle_ = handle;
        }

        public async_result_t Get(System.TimeSpan timeout)
        {
            var pHandle = cuvis_il.new_p_int();
            cuvis_il.p_int_assign(pHandle, handle_);
            var result =
                cuvis_il.cuvis_async_call_get(pHandle, (int)timeout.TotalMilliseconds);

            switch (result)
            {
                case cuvis_status_t.status_ok: return async_result_t.done;
                case cuvis_status_t.status_deferred: return async_result_t.deferred;
                case cuvis_status_t.status_overwritten:
                    return async_result_t.overwritten;
                case cuvis_status_t.status_timeout: return async_result_t.timeout;
                default: throw new SDK_Exception();
            }
        }

        public void Dispose()
        {
            var pHandle = cuvis_il.new_p_int();
            cuvis_il.p_int_assign(pHandle, handle_);
            cuvis_il.cuvis_async_call_free(pHandle);
            handle_ = cuvis_il.p_int_value(pHandle);
        }
    }

    public class AsyncMesu : IDisposable
    {
        internal int handle_ = 0;
        internal AsyncMesu(int handle)
        {
            handle_ = handle;
        }

        public void Dispose()
        {
            var pHandle = cuvis_il.new_p_int();
            cuvis_il.p_int_assign(pHandle, handle_);
            cuvis_il.cuvis_async_capture_free(pHandle);
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        public System.Tuple<async_result_t, Measurement> Get(System.TimeSpan timeout)
        {

            var pHandle = cuvis_il.new_p_int();
            var mesuHandle = cuvis_il.new_p_int();
            cuvis_il.p_int_assign(pHandle, handle_);
            var result = cuvis_il.cuvis_async_capture_get(pHandle, (int)timeout.TotalMilliseconds, mesuHandle);

            switch (result)
            {
                case cuvis_status_t.status_ok:
                    return new System.Tuple<async_result_t, Measurement>(async_result_t.done, new Measurement(cuvis_il.p_int_value(mesuHandle)));
                case cuvis_status_t.status_deferred: return new System.Tuple<async_result_t, Measurement>(async_result_t.deferred, null);
                case cuvis_status_t.status_overwritten:
                    return new System.Tuple<async_result_t, Measurement>(async_result_t.overwritten, null);
                case cuvis_status_t.status_timeout: return new System.Tuple<async_result_t, Measurement>(async_result_t.timeout, null);
                default: throw new SDK_Exception();
            }
        }
    }

    public enum ProcessingMode
    {
        Preview = cuvis_processing_mode_t.Preview,
        Raw = cuvis_processing_mode_t.Cube_Raw,
        DarkSubtract = cuvis_processing_mode_t.Cube_DarkSubtract,
        Reflectance = cuvis_processing_mode_t.Cube_Reflectance,
        SpectralRadiance = cuvis_processing_mode_t.Cube_SpectralRadiance
    };

    public enum OperationMode
    {
        External = cuvis_operation_mode_t.OperationMode_External,
        Internal = cuvis_operation_mode_t.OperationMode_Internal,
        Software = cuvis_operation_mode_t.OperationMode_Software
    };

    public enum ReferenceType
    {
        White = cuvis_reference_type_t.Reference_White,
        Dark = cuvis_reference_type_t.Reference_Dark,
        WhiteDark = cuvis_reference_type_t.Reference_WhiteDark,
        SpRad = cuvis_reference_type_t.Reference_SpRad,
        Distance = cuvis_reference_type_t.Reference_Distance
    };

    public enum SessionItemType
    {
        FramesNoGaps = cuvis_session_item_type_t.session_item_type_frames_no_gaps,
        Frames = cuvis_session_item_type_t.session_item_type_frames,
        References = cuvis_session_item_type_t.session_item_type_references
    };

    public enum PanSharpeningInterpolationType
    {
        NearestNeighbour = cuvis_pan_sharpening_interpolation_type_t.pan_sharpening_interpolation_type_NearestNeighbor,
        Linear = cuvis_pan_sharpening_interpolation_type_t.pan_sharpening_interpolation_type_Linear,
        Cubic = cuvis_pan_sharpening_interpolation_type_t.pan_sharpening_interpolation_type_Cubic,
        Lanczos = cuvis_pan_sharpening_interpolation_type_t.pan_sharpening_interpolation_type_Lanczos
    }

    public enum PanSharpeningAlgorithm
    {
        Noop = cuvis_pan_sharpening_algorithm_t.pan_sharpening_algorithm_Noop,
        PanRatio = cuvis_pan_sharpening_algorithm_t.pan_sharpening_algorithm_CubertPanRatio,
        CubertMacroPixel = cuvis_pan_sharpening_algorithm_t.pan_sharpening_algorithm_CubertMacroPixel,
        AlphaBlend = cuvis_pan_sharpening_algorithm_t.pan_sharpening_algorithm_AlphablendPanOverlay
    }

    public enum TiffCompressionMode
    {
        None = cuvis_tiff_compression_mode_t.tiff_compression_mode_None,
        LZW = cuvis_tiff_compression_mode_t.tiff_compression_mode_LZW
    }

    public enum TiffFormat
    {
        Single = cuvis_tiff_format_t.tiff_format_Single,
        MultiChannel = cuvis_tiff_format_t.tiff_format_MultiChannel,
        MultiPage = cuvis_tiff_format_t.tiff_format_MultiPage
    }

    public enum HardwareState
    {
        Offline = cuvis_hardware_state_t.hardware_state_offline,
        PartiallyOnline = cuvis_hardware_state_t.hardware_state_partially_online,
        Online = cuvis_hardware_state_t.hardware_state_online
    }

    public enum ComponentType
    {
        ImageSensor = cuvis_component_type_t.component_type_image_sensor,
        MiscSensor = cuvis_component_type_t.component_type_misc_sensor
    }

    public class ComponentStateInfo
    {

        private string displayName;
        private bool isOnline;

        public ComponentStateInfo(string name, bool online)
        {
            this.displayName = name;
            this.isOnline = online;
        }

        public string DisplayName { get { return displayName; } }
        public bool Online { get { return isOnline; } set { isOnline = value; } }
    }

    public enum Capability
    {
        AcquisitionCapture = 1,
        AcquisitionTimelapse = 2,
        AcquisitionContinuous = 4,
        AcquisitionSnapshot = 8,
        AcquisitionSetIntegrationtime = 16,
        AcquisitionSetGain = 32,
        AcquisitionAveraging = 64,
        ProcessingSensorRaw = 128,
        ProcessingCubeRaw = 256,
        ProcessingCubeRef = 512,
        ProcessingCubeDarkSubtract = 1024,
        ProcessingCubeFlatFielding = 2048,
        ProcessingCubeSpectralRadiance = 4096,
        ProcessingSaveFile = 8192,
        ProcessingClearRaw = 16384,
        ProcessingCalcLive = 32768,
        ProcessingAutoExposure = 65536,
        ProcessingOrientation = 131072,
        ProcessingSetWhite = 262144,
        ProcessingSetDark = 524288,
        ProcessingSetSprad = 1048576,
        ProcessingSetDistanceCalib = 2097152,
        ProcessingSetDistanceValue = 4194304,
        ProcessingUseDarkSpradcalib = 8388608,
        ProcessingUseWhiteSpradCalib = 16777216,
        ProcessingRequireWhiteDarkReflectance = 33554432
    }

    public static class CapabilityConversion
    {
        public static IEnumerable<Capability> FromBitset(int bitset)
        {
            List<Capability> capabilities = new List<Capability>();
            for (int i = 1; i <= 33554432; i = i * 2)
            {
                if ((bitset & i) != 0)
                {
                    capabilities.Add((Capability)i);
                }
            }
            return capabilities;
        }
    }

    public enum MeasurementFlag 
    {
        Overilluminated = 1,
        PoorReference = 2,
        PoorWhiteBalancing = 4,
        DarkIntTime = 8,
        DarkTemp = 16,
        WhiteIntTime = 32,
        WhiteTemp = 64,
        WhiteDarkIntTime = 128,
        WhiteDarkTemp = 256
    }

    public static class MeasurementFlagConversion 
    {
        public static IEnumerable<MeasurementFlag> FromBitset(uint bitset)
        {
            List<MeasurementFlag> measurementFlags = new List<MeasurementFlag>();
            for (int i = 1; i <= 256; i = i * 2)
            {
                if ((bitset & i) != 0)
                {
                    measurementFlags.Add((MeasurementFlag)i);
                }
            }
            return measurementFlags;
        }
    }


    public struct SessionData
    {
        public string Name { get; set; }
        public int SessionNumber { get; set; }
        public int SequenceNumber { get; set; }

        public SessionData(string name, int sessionNumber, int sequenceNumber)
        {
            Name = name;
            SessionNumber = sessionNumber;
            SequenceNumber = sequenceNumber;
        }
    };

    public struct WorkerArgs
    {

        public uint WorkerCount { get; set; }

        public int PollIntervallInMs { get; set; }
        public int QueueHardLimit { get; set; }
        public int QueueSoftLimit { get; set; }
        public bool CanDrop { get; set; }

        public WorkerArgs(uint workerCount, int pollIntervallInMs, int queueHardLimit, int queueSoftLimit, bool canDrop)
        {
            WorkerCount = workerCount;
            PollIntervallInMs = pollIntervallInMs;
            QueueHardLimit = queueHardLimit;
            QueueSoftLimit = queueSoftLimit;
            CanDrop = canDrop;
        }

        internal WorkerArgs(cuvis_worker_settings_t ws)
        {
            WorkerCount = (uint)ws.worker_count;
            PollIntervallInMs = ws.poll_interval;
            QueueHardLimit = ws.worker_queue_hard_limit;
            QueueSoftLimit = ws.worker_queue_soft_limit;
            CanDrop = ws.can_drop > 0;
        }

        internal cuvis_worker_settings_t GetInternal()
        {
            cuvis_worker_settings_t ws = new cuvis_worker_settings_t();
            ws.worker_count = (int)WorkerCount;
            ws.poll_interval = (int)PollIntervallInMs;
            ws.worker_queue_hard_limit = QueueHardLimit;
            ws.worker_queue_soft_limit = QueueSoftLimit;
            ws.can_drop = (CanDrop ? 1 : 0);
            return ws;
        }
    }

    public struct ComponentInfo
    {
        public ComponentType Type { get; set; }
        public string DisplayName { get; set; }

        public string SensorInfo { get; set; }

        public string UserField { get; set; }

        public string Pixelformat { get; set; }

        public ComponentInfo(ComponentType type, string displayname, string sensorInfo, string userfield, string pixelformat)
        {
            Type = type;
            DisplayName = displayname;
            SensorInfo = sensorInfo;
            UserField = userfield;
            Pixelformat = pixelformat;
        }

        internal ComponentInfo(cuvis_component_info_t ci)
        {
            Type = (ComponentType)ci.type;
            DisplayName = ci.displayname;
            SensorInfo = ci.sensorinfo;
            UserField = ci.userfield;
            Pixelformat = ci.pixelformat;
        }


    }

    public class SensorInfo : Data
    {

        public SensorInfo(uint averages, double temperature, double gain, DateTime readouttime, ulong frame_id)
        {
            Averages = averages;
            Temperature = temperature;
            Gain = gain;
            ReadOutTime = readouttime;
            RawFrameID = frame_id;
        }

        internal SensorInfo(cuvis_sensor_info_t si)
        {
            Averages = si.averages;
            Temperature = si.temperature;
            Gain = si.gain;
            ReadOutTime = Helper.ToDateTime(si.readout_time);
            Width = si.width;
            Height = si.height;
            RawFrameID = si.raw_frame_id;
        }

        public uint Averages { get; set; }

        public double Temperature { get; set; }

        public double Gain { get; set; }

        public uint Width { get; set; }

        public uint Height { get; set; }

        public DateTime ReadOutTime { get; set; }

        public ulong RawFrameID { get; set; }

    }

    #region Export Settings

    public struct GeneralExportSettings
    {
        public string ExportDir { get; set; }

        public string ChannelSelection { get; set; }

        public byte SpectraMultiplier { get; set; }

        public double PanScale { get; set; }

        public PanSharpeningInterpolationType PanSharpeningInterpolationType { get; set; }

        public PanSharpeningAlgorithm PanSharpeningAlgorithmType { get; set; }

        public bool AddPan { get; set; }

        public bool AddFullscalePan { get; set; }

        public bool Permissive { get; set; }

        public double BlendOpacity { get; set; }

        public static GeneralExportSettings Default
        {
            get
            {
                GeneralExportSettings args = new GeneralExportSettings();
                args.ExportDir = ".";
                args.ChannelSelection = "all";
                args.SpectraMultiplier = 1;
                args.PanScale = 0.0;
                args.PanSharpeningInterpolationType = PanSharpeningInterpolationType.Linear;
                args.PanSharpeningAlgorithmType = PanSharpeningAlgorithm.CubertMacroPixel;
                args.AddPan = false;
                args.AddFullscalePan = false;
                args.Permissive = false;
                args.BlendOpacity = 0.0;
                return args;
            }
        }

        public GeneralExportSettings(string exportDir, string channelSelection, byte spectraMultiplier, double panScale, PanSharpeningInterpolationType panSharpeningInterpolationType, PanSharpeningAlgorithm panSharpeningAlgorithmType, bool addPan, bool addFullscalePan, bool permissive, double blendOpacity)
        {
            ExportDir = exportDir;
            ChannelSelection = channelSelection;
            SpectraMultiplier = spectraMultiplier;
            PanScale = panScale;
            PanSharpeningInterpolationType = panSharpeningInterpolationType;
            PanSharpeningAlgorithmType = panSharpeningAlgorithmType;
            AddPan = addPan;
            AddFullscalePan = addFullscalePan;
            Permissive = permissive;
            BlendOpacity = blendOpacity;
        }

        internal GeneralExportSettings(cuvis_export_general_settings_t ge)
        {
            ExportDir = ge.export_dir;
            ChannelSelection = ge.channel_selection;
            SpectraMultiplier = (byte)ge.spectra_multiplier;
            PanScale = ge.pan_scale;
            PanSharpeningInterpolationType = (PanSharpeningInterpolationType)ge.pan_interpolation_type;
            PanSharpeningAlgorithmType = (PanSharpeningAlgorithm)ge.pan_algorithm;
            AddPan = ge.add_pan > 0;
            AddFullscalePan = ge.add_fullscale_pan > 0;
            Permissive = ge.permissive > 0;
            BlendOpacity = ge.blend_opacity;
        }

        internal cuvis_export_general_settings_t GetInternal()
        {
            cuvis_export_general_settings_t ge = new cuvis_export_general_settings_t();
            ge.export_dir = ExportDir;
            ge.channel_selection = ChannelSelection;
            ge.spectra_multiplier = (byte)SpectraMultiplier;
            ge.pan_scale = PanScale;
            ge.pan_interpolation_type = (cuvis_pan_sharpening_interpolation_type_t)PanSharpeningInterpolationType;
            ge.pan_algorithm = (cuvis_pan_sharpening_algorithm_t)PanSharpeningAlgorithmType;
            ge.add_pan = (AddPan ? 1 : 0);
            ge.add_fullscale_pan = (AddFullscalePan ? 1 : 0);
            ge.permissive = (Permissive ? 1 : 0);
            ge.blend_opacity = BlendOpacity;
            return ge;
        }
    }

    public struct TiffExportSettings
    {
        TiffCompressionMode CompressionMode { get; set; }

        TiffFormat Format { get; set; }

        public TiffExportSettings(TiffCompressionMode compressionMode, TiffFormat format)
        {
            CompressionMode = compressionMode;
            Format = format;
        }

        internal TiffExportSettings(cuvis_export_tiff_settings_t ts)
        {
            CompressionMode = (TiffCompressionMode)ts.compression_mode;
            Format = (TiffFormat)ts.format;
        }

        internal cuvis_export_tiff_settings_t GetInternal()
        {
            cuvis_export_tiff_settings_t ts = new cuvis_export_tiff_settings_t();
            ts.compression_mode = (cuvis_tiff_compression_mode_t)CompressionMode;
            ts.format = (cuvis_tiff_format_t)Format;
            return ts;
        }
    }

    public struct ViewExportSettings
    {
        string Userplugin { get; set; }

        public ViewExportSettings(string userplugin)
        {
            Userplugin = userplugin;
        }

        internal ViewExportSettings(cuvis_export_view_settings_t vs)
        {
            Userplugin = vs.userplugin;
        }

        internal cuvis_export_view_settings_t GetInternal()
        {
            cuvis_export_view_settings_t vs = new cuvis_export_view_settings_t();
            vs.userplugin = Userplugin;
            return vs;
        }
    }

    public struct SaveArgs
    {
        public bool AllowFragmentation { get; set; }
        public bool AllowOverride { get; set; }
        public bool AllowDrop { get; set; }
        public bool AllowSessionFile { get; set; }
        public bool AllowInfoFile { get; set; }
        public cuvis_net.OperationMode OperationMode { get; set; }
        public double FPS { get; set; }
        public int SoftLimit { get; set; }
        public int HardLimit { get; set; }
        public int MaxBufTimeMilliseconds { get; set; }

        public static SaveArgs Default
        {
            // C# doesn't allow parameterless constructors for structs
            // instead a static property with default values is defined
            get
            {
                SaveArgs args = new SaveArgs();
                args.AllowFragmentation = false;
                args.AllowOverride = false;
                args.AllowDrop = false;
                args.AllowSessionFile = true;
                args.AllowInfoFile = true;
                args.OperationMode = OperationMode.Software;
                args.FPS = 0.0;
                args.SoftLimit = 20;
                args.HardLimit = 100;
                args.MaxBufTimeMilliseconds = 10000;
                return args;
            }
        }


        internal SaveArgs(cuvis_save_args_t sa)
        {
            AllowFragmentation = sa.allow_fragmentation > 0;
            AllowOverride = sa.allow_overwrite > 0;
            AllowDrop = sa.allow_drop > 0;
            AllowSessionFile = sa.allow_session_file > 0;
            AllowInfoFile = sa.allow_info_file > 0;
            OperationMode = (cuvis_net.OperationMode)sa.operation_mode;
            FPS = sa.fps;
            SoftLimit = sa.soft_limit;
            HardLimit = sa.hard_limit;
            MaxBufTimeMilliseconds = sa.max_buftime;

        }

        internal cuvis_save_args_t GetInternal()
        {
            cuvis_save_args_t sa = new cuvis_save_args_t();
            sa.allow_fragmentation = (AllowFragmentation ? 1 : 0);
            sa.allow_overwrite = (AllowOverride ? 1 : 0);
            sa.allow_drop = (AllowDrop ? 1 : 0);
            sa.allow_session_file = (AllowSessionFile ? 1 : 0);
            sa.allow_info_file = (AllowInfoFile ? 1 : 0);
            sa.operation_mode = (cuvis_operation_mode_t)OperationMode;
            sa.fps = FPS;
            sa.soft_limit = SoftLimit;
            sa.hard_limit = HardLimit;
            sa.max_buftime = MaxBufTimeMilliseconds;
            return sa;
        }
    }

    #endregion

    internal class Helper
    {
        static public System.DateTime ToDateTime(ulong ts_ms)
        {

            //var dto = System.DateTimeOffset.FromUnixTimeMilliseconds(ts_ms);
            //return dto.UtcDateTime;
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(ts_ms);
            return dateTime;

        }
    }

    public class SDK_Exception : System.Exception
    {
        public static void set_locale(string locale_id)
        {
            cuvis_il.cuvis_set_last_error_locale(locale_id);
        }
        public SDK_Exception()
          : base(cuvis_il.cuvis_get_last_error_msg_localized())
        {

        }

    }

    public interface Data
    {

    }

    public struct GPSData : Data
    {
        public GeoCoordinate coordinate;

        public System.DateTime time;
    }
    public struct StringData : Data
    {
        public string value;

    }
    public class ImageData<data_t> : Data
    {

        public data_t[,,] arr;
        public uint[] wavelength;

        private uint width;
        private uint height;
        private uint channels;

        public uint Width { get { return width; } }
        public uint Height { get { return height; } }
        public uint Channels { get { return channels; } }


        internal static Bitmap ToGreyscale(ImageData<byte> image)
        {
            if (image.Channels != 1)
            {
                return null;
            }
            var b = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format8bppIndexed);

            ColorPalette ncp = b.Palette;
            for (int i = 0; i < 256; i++)
                ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            b.Palette = ncp;

            var BoundsRect = new Rectangle(0, 0, (int)image.Width, (int)image.Height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = bmpData.Stride * b.Height;
            var rgbValues = new byte[bytes];

            for (int i = 0; i < image.width; i++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    rgbValues[i * image.height + j] = (byte)(image.arr[i, j, 0]);
                }
            }


            Marshal.Copy(rgbValues, 0, ptr, bytes);
            b.UnlockBits(bmpData);
            return b;
        }

        internal object FromByteArray<T>(byte[] data)
        {
            if (typeof(T) == typeof(byte))
            {
                return data[0];
            }
            else if (typeof(T) == typeof(ushort))
            {
                return System.BitConverter.ToUInt16(data, 0);
            }
            else if (typeof(T) == typeof(uint))
            {
                return System.BitConverter.ToUInt32(data, 0);
            }
            else if (typeof(T) == typeof(float))
            {
                return System.BitConverter.ToSingle(data, 0);
            }
            else throw new System.Exception("unsupported data tye");
        }
        internal ImageData(cuvis_imbuffer_t buf)
        {
            this.height = buf.height;
            this.width = buf.width;
            this.channels = buf.channels;

            unsafe
            {
                arr = new data_t[buf.width, buf.height, buf.channels];

                var barr = new byte[buf.bytes];

                for (int x = 0; x < buf.width; x++)
                {
                    for (int y = 0; y < buf.height; y++)
                    {
                        for (int z = 0; z < buf.channels; z++)
                        {


                            for (int b = 0; b < buf.bytes; b++)
                            {
                                int index = ((((y) * (int)buf.width) + x) * buf.channels + z) * (int)buf.bytes + b;
                                barr[b] = cuvis_il.p_unsigned_char_getitem(buf.raw, index);
                            }

                            arr[x, y, z] = (data_t)FromByteArray<data_t>(barr);
                        }
                    }
                }

                //check wavelength???
                if (buf.wavelength != null)
                {
                    var wl = new uint[buf.channels];
                    for (int z = 0; z < buf.channels; z++)
                    {
                        wl[z] = cuvis_il.p_unsigned_int_getitem(buf.wavelength, z);
                    }
                    wavelength = wl;
                }
            }
        }

    }

}