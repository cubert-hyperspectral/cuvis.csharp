using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace cuvis_net
{
    public class Event { }
    public delegate void EventCallback(Event e);


    public enum EventType : int
    {
        Event = 0
    }

    public static class EventHandler
    {
        // we need to keep a reference to the callbacks otherwise they get garbage collected
        private static Dictionary<int, cuvis_il.EventCallback> internal_callbacks = new Dictionary<int, cuvis_il.EventCallback>();
        private static int RegisterEventCallback(cuvis_il.EventCallback callback, int type)
        {
            var val = cuvis_il.new_p_int();
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_register_external_event_callback(callback, type, val))
            {
                throw new SDK_Exception();
            }
            return cuvis_il.p_int_value(val);
        }

        public static int RegisterEventCallback(EventCallback callback, EventType type)
        {
            cuvis_il.EventCallback internal_callback = (id, ev) =>
            {
                // mark/todo here get properties as soon as they are added to sdk
                Event e = new Event();
                callback(e);
            };
            int hander_id =  RegisterEventCallback(internal_callback, (int)type);
            internal_callbacks.Add(hander_id, internal_callback);
            return hander_id;
        }

        public static void UnregisterEventCallback(int handler_id)
        {
            if (cuvis_status_t.status_ok != cuvis_il.cuvis_unregister_event_callback(handler_id))
            {
                throw new SDK_Exception();
            }
            internal_callbacks.Remove(handler_id);
        }
    }
}