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
