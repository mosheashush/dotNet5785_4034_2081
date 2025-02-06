using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal
{
    internal class AssignmentImplementation : IAssignment
    {

        
        static Assignment getAssignment (XElement assignment)
        {
            return new DO.Assignment()
            {
                Id = (int)assignment.Element("Id"),
                CallId = (int)assignment.Element("CallId"),
                VolunteerId = (int)assignment.Element("VolunteerId"),
                StarCall = (DateTime)assignment.Element("StarCall"),
                FinishType = assignment.Element("FinishType") != null ?
                             Enum.Parse<CompletionType>(assignment.Element("FinishType").Value) :
                             (CompletionType?)null
            };
        }
        public void Create(Assignment item)
        {
            List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
            if (assignments.FirstOrDefault(a => a.Id == item.Id) != null)
                throw new DalAlreadyExistsException($"Assignment with the same ID={item.Id} already exists...");
            else
            {
                XElement assignmentElem = createssignmentElement(item);
                XMLTools.SaveListToXMLElement( assignmentElem , Config.s_assignments_xml);
                assignments.Add(item);
                XMLTools.SaveListToXMLSerializer<Assignment>(assignments, Config.s_assignments_xml);
            }
        }

        public void Delete(int id)
        {
            List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
            if (assignments.RemoveAll(it => it.Id == id) == 0)
                throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
        }

        public void DeleteAll()
        {
             XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);  
        }

        public Assignment? Read(int id)
        {
            XElement? assignmentElem = 
                                      XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(st =>
                                      (int?)st.Element("Id") == id);
            return assignmentElem is null ? null : getAssignment(assignmentElem);
        }

        public Assignment? Read(Func<Assignment, bool> filter)
        {
            return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a)).FirstOrDefault(filter);
        }

        public IEnumerable<Assignment>? ReadAll(Func<Assignment, bool>? filter = null)
        {
            IEnumerable<Assignment> assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a));
            return assignments == null ? null : assignments.Select(a => a);

        }

        public void Update(Assignment item)
        {
            XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
            ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                    .Remove();

            assignmentsRootElem.Add(new XElement("Assignment", createssignmentElement(item)));

            XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
        }

        private XElement? createssignmentElement(Assignment item)
        {
            return new XElement("Assignment",
                new XElement("Id", item.Id),
                new XElement("CallId", item.CallId),
                new XElement("VolunteerId", item.VolunteerId),
                new XElement("StarCall", item.StarCall),
                new XElement("FinishType", item.FinishType)
                );
        }
    }
}
