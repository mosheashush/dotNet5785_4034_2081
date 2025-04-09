using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dal
{
    internal class CallImplementation : ICall
    {
        public void Create(Call item)
        {
            List<Call> listCalls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (item.Id == 0)
                item = item.WithId(Config.NextCallId);
            listCalls.Add(item);
            XMLTools.SaveListToXMLSerializer(listCalls, Config.s_calls_xml);
        }

        public void Delete(int id)
        {
            List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (calls.RemoveAll(it => it.Id == id) == 0)
                throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);

        }

        public void DeleteAll()
        {
            XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
        }

        public Call? Read(int Id)
        {
            List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            Call? call = calls.FirstOrDefault(v => v.Id == Id);
            return call;
        }


        public Call? Read(Func<Call, bool> filter)
        {
            List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            Call? call = calls.FirstOrDefault(filter);
            return call;
        }

        public IEnumerable<Call>? ReadAll(Func<Call, bool>? filter = null)
        {
            List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            IEnumerable<Call> call_s = calls;
            return filter == null ? call_s : call_s.Where(filter);
        }

        public void Update(Call item)
        {
            List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
            if (calls.Remove(calls.FirstOrDefault(c => c.Id == item.Id)))
            {
                calls.Add(item);
                XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
            }
            else
            {
                throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
            }
        }
    }
}
//static Call getCall(XElement call)
//{
//    return new DO.Call()
//    {
//        Id = (int)call.Element("Id"),
//        Type = (CallType)Enum.Parse(typeof(CallType), call.Element("Type")?.Value ?? throw new InvalidOperationException("Type element is missing")),
//        FullAddress = call.Element("FullAddress")?.Value ?? throw new InvalidOperationException("FullAddress element is missing"),
//        Latitude = (double)call.Element("Latitude"),
//        Longitude = (double)call.Element("Longitude"),
//        CallStartTime = (DateTime)call.Element("CallStartTime"),
//        description = call.Element("description")?.Value,
//        MaxTimeForCall = (DateTime)call.Element("MaxTimeForCall")
//    };
//}

//static XElement createCallElement(Call call)
//{
//    return new XElement("Call",
//        new XElement("Id", call.Id),
//        new XElement("Type", call.Type),
//        new XElement("FullAddress", call.FullAddress),
//        new XElement("Latitude", call.Latitude),
//        new XElement("Longitude", call.Longitude),
//        new XElement("CallStartTime", call.CallStartTime),
//        new XElement("description", call.description),
//        new XElement("MaxTimeForCall", call.MaxTimeForCall)
//        );
//}
//List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
//if (item.Id == 0)
//    item = item.WithId(Config.NextCallId);

//else if (calls.FirstOrDefault(c => c.Id == item.Id) != null)
//{
//    throw new DalAlreadyExistsException($"Call with the same ID={item.Id} already exists...");
//}
//else
//{
//    XElement callElem = createCallElement(item);
//    XMLTools.SaveListToXMLElement(callElem, Config.s_calls_xml);
//    calls.Add(item);
//    XMLTools.SaveListToXMLSerializer<Call>(calls, Config.s_calls_xml);
//}