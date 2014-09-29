using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nebulas.Events;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Collections.Generic;
using Lidgren.Network;

namespace NebulasUnitTest
{
    [TestClass]
    public class EventTests
    {

        [TestMethod]
        public void EventCreation()
        {
            Nebulas.Events.EventListener listener = new Nebulas.Events.EventListener();
            Nebulas.Events.Event evt = new Event("echo");

            Assert.AreEqual("echo", evt.mName);

            String result = listener.RespondTo(evt);
            Assert.AreEqual("Echo: No properties", result);

            Assert.AreEqual("nil", evt.GetProperty("weight"));

            evt.SetProperty("weight", "1");
            Assert.AreEqual("1", evt.GetProperty("weight"));

            result = listener.RespondTo(evt);
            Assert.AreEqual("Echo: Properties", result);
        }

        [TestMethod]
        public void EventDispatch()
        {
            Nebulas.Events.EventStream source = new Nebulas.Events.EventStream();
            Nebulas.Events.EventStream sink = new Nebulas.Events.EventStream();
            Nebulas.Events.EventListener listener = new Nebulas.Events.EventListener();
            Nebulas.Events.Event evt = new Event("echo");
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            //Push, Broadcast, Cutoff
            Assert.IsTrue(stream.Length == 0);
            source.PushEvent(evt);
            Assert.IsTrue(source.EventCount() == 1);
            source.Broadcast(stream);
            Assert.IsTrue(stream.Length > 0);
            //Check that our event is still intact
            System.IO.MemoryStream stream_copy = new System.IO.MemoryStream();
            stream_copy.SetLength(stream.Length);
            stream.WriteTo(stream_copy);
            stream_copy.Position = 0;
            stream.Position = 0;
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            List<Event> reconstruction = (List<Event>)formatter.Deserialize(stream_copy);
            Assert.IsTrue(reconstruction.Count == 1);
            Event evt_copy = reconstruction[0];
            Assert.AreEqual(evt.mName, evt_copy.mName);
            Assert.AreEqual(evt.mTimestamp, evt_copy.mTimestamp);
            foreach(KeyValuePair<String, String> kvp in evt.mProperties)
            {
                String value;
                if(evt_copy.mProperties.TryGetValue(kvp.Key, out value))
                {
                    Assert.AreEqual(kvp.Value, value);
                }
                else
                {
                    Assert.Fail("Missing properties detected");
                }
            }
            Assert.IsTrue(sink.EventCount() == 0);
            sink.RecieveEvents(stream);
            Assert.IsTrue(sink.EventCount() == 1);
            sink.DispatchEvents();
            Assert.IsTrue(sink.EventCount() == 0);
        }

        [TestMethod]
        public void EventRejection()
        {
            EventStream source = new EventStream();
            EventListener listener = new EventListener();
            Event evt = new Event("echo");
            System.Threading.Thread.Sleep(1000);
            source.UpdateCutoff();
            Assert.IsTrue(source.EventCount() == 0);
            source.PushEvent(evt);
            Assert.IsTrue(source.EventCount() == 0);
        }

    }
}