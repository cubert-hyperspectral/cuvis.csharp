
using System;
using System.Collections.Generic;
using System.Drawing;

namespace cuvis_net
{
    public class Viewer : System.IDisposable
    {
        internal int handle_ = 0;
        public Viewer(cuvis_viewer_settings_t settings)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_viewer_create(pHandle, settings))
            {
                throw new SDK_Exception();
            }
            handle_ = cuvis_il.p_int_value(pHandle);
        }

        internal static ViewResult createViewData(int currentView)
        {
            var countHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_view_get_data_count(currentView, countHandle))
            {
                throw new SDK_Exception();
            }
            int dataCount = cuvis_il.p_int_value(countHandle);

            List<ImageData<byte>> dataList = new List<ImageData<byte>>();
            List<Bitmap> imageList = new List<Bitmap>();


            for (int i = 0; i < dataCount; i++)
            {
                cuvis_view_data_t view_data = new cuvis_view_data_t();
                if (cuvis_status_t.status_ok != cuvis_il.cuvis_view_get_data(currentView, i, view_data))
                {
                    throw new SDK_Exception();
                }

                ImageData<byte> data;
                switch (view_data.data.format)
                {
                    case cuvis_imbuffer_format_t.imbuffer_format_uint8:
                        {
                            switch (view_data.category)
                            {
                                case cuvis_view_category_t.view_category_data:
                                    dataList.Add(new ImageData<byte>(view_data.data));
                                    break;
                                case cuvis_view_category_t.view_category_image:
                                    imageList.Add(ImageData<byte>.ToGreyscale(new ImageData<byte>(view_data.data)));
                                    break;
                                default:
                                    throw new System.ArgumentException("unsupported view bit depth");
                                    break;
                            }
                            break;
                        }
                    default:
                        throw new System.ArgumentException("unsupported view bit depth");
                        break;
                }
            }
            return new ViewResult(dataList,imageList,true);
        }

        public ViewResult Apply(Measurement mesu)
        {
            var pHandle = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_viewer_apply(handle_, mesu.handle_, pHandle))
            {
                throw new SDK_Exception();
            }
            int currentView = cuvis_il.p_int_value(pHandle);
            return createViewData(currentView);
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
            cuvis_il.cuvis_viewer_free(pHandle);

            disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        ~Viewer()
        {
            Dispose(disposing: false);
        }
    }

    public class ViewResult
    {
        private List<ImageData<byte>> data_;
        private List<Bitmap> images_;
        private bool show_;

        public List<ImageData<byte>> Data { get { return data_; } }
        public List<Bitmap> Images { get { return images_; } }

        public bool Show { get { return show_; } }

        public ViewResult(List<ImageData<byte>> data, List<Bitmap> images, bool show)
        {
            this.data_ = data;
            this.images_ = images;
            this.show_ = show;
        }
    }
}