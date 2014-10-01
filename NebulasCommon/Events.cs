using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Nebulas
{
    namespace Events
    {
        [Serializable]
        public class Event : ISerializable
        {
            public String mName;
            public DateTime mTimestamp;
            //Values
            public Dictionary<String, String> mProperties;

            public Event()
            {
                mTimestamp = DateTime.UtcNow;
                mName = "UnknownEvent";
                mProperties = new Dictionary<string, string>();
            }

            public Event(String name)
            {
                mTimestamp = DateTime.UtcNow;
                mName = name;
                mProperties = new Dictionary<string, string>();
            }

            public Event(SerializationInfo info, StreamingContext context)
            {
                
                SerializationInfoEnumerator e = info.GetEnumerator();
                while(e.MoveNext())
                {
                    if(e.Name == "name")
                    {
                        mName = (String)e.Value;
                    }
                    else if(e.Name == "timestamp")
                    {
                        mTimestamp = (DateTime)e.Value;
                    }
                    else if(e.Name == "properties")
                    {
                        mProperties = (Dictionary<string, string>)e.Value;
                    }
                    else
                    {
                        //Nothing
                    }
                }
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("name", mName);
                info.AddValue("timestamp", mTimestamp);
                info.AddValue("properties", mProperties);
            }

            public void SetProperty(String key, String value)
            {
                mProperties.Add(key, value);
            }

            public void UpdateProperties(Dictionary<string, string> properties)
            {
                mProperties = properties;
            }

            public Boolean HasProperty(String key)
            {
                return mProperties.ContainsKey(key);
            }

            public String GetProperty(String key)
            {
                String response;
                if (mProperties.TryGetValue(key,out response))
                {
                    return response;
                }
                else
                {
                    return "nil";
                }
            }

            
        }

        public class EventListener
        {
            
            Dictionary<String, String> mEventMap;

            public EventListener()
            {
                mEventMap = new Dictionary<string, string>();
            }

            public String RespondTo(Event evt)
            {
                String response = "RespondTo: Error";
                if(evt.mName == "echo")
                {
                    if (evt.mProperties.Keys.Count == 0)
                    {
                        response = "Echo: No properties";
                        System.Diagnostics.Debug.WriteLine(response);
                    }
                    else
                    {
                        response = "Echo: Properties";
                        System.Diagnostics.Debug.WriteLine(response);
                        foreach (KeyValuePair<string, string> property in evt.mProperties)
                        {
                            System.Diagnostics.Debug.WriteLine(property.Key);
                            System.Diagnostics.Debug.WriteLine(property.Value);
                        }
                    }
                }
                return response;
            }

            public void AddResponse(String eventName, String response)
            {
                mEventMap.Add(eventName, response);
            }
            
        }

        public class EventStream
        {
            List<Event> mEvents;
            DateTime mCutoffTime;
            Dictionary<String, List<EventListener>> mEventListeners;

            public EventStream()
            {
                mEvents = new List<Event>();
                mCutoffTime = DateTime.UtcNow;
                mEventListeners = new Dictionary<string, List<EventListener>>();
            }

            public void PushEvent(Event evt)
            {
                if(ValidateEvent(evt))
                {
                    mEvents.Add(evt);
                }
            }

            public void AddEventSubscriber(String event_name, EventListener listener)
            {
                List<EventListener> list; 
                
                if(!mEventListeners.TryGetValue(event_name, out list))
                {
                    list = new List<EventListener>();
                    mEventListeners.Add(event_name, list);
                }
                list.Add(listener);
            }

            public void Broadcast(System.IO.Stream stream)
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, mEvents);
                stream.Flush();
            }

            public void RecieveEvents(System.IO.Stream stream)
            {

                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                QueueValid((List<Event>) formatter.Deserialize(stream));
            }
           
            protected Boolean ValidateEvent(Event evt)
            {
                return evt.mTimestamp >= mCutoffTime;
            }

            protected void QueueValid(List<Event> lst)
            {
                lst.RemoveAll(e => !ValidateEvent(e));
                mEvents.AddRange(lst);
            }

            public void DispatchEvents()
            {
                foreach (Event evt in mEvents)
                {
                    List<EventListener> subscribers;
                    if (mEventListeners.TryGetValue(evt.mName, out subscribers))
                    {
                        foreach (EventListener subscriber in subscribers)
                        {
                            subscriber.RespondTo(evt);
                        }
                    }
                }
                mEvents.Clear();
            }

            public void UpdateCutoff()
            {
                mCutoffTime = DateTime.UtcNow;
            }

            public Int32 EventCount()
            {
                return mEvents.Count;
            }
            
        }

    }
}
