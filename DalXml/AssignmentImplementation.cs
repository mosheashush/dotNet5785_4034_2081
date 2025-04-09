using Dal;
using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal
{
    internal class AssignmentImplementation : IAssignment
    {
        private XElement? createAssignmentElement(Assignment item)
        {
            return new XElement("Assignment",
                new XElement("Id", item.Id),
                new XElement("CallId", item.CallId),
                new XElement("VolunteerId", item.VolunteerId),
                new XElement("StarCall", item.StarCall),
                new XElement("CompletionTime", item.CompletionTime),
                new XElement("FinishType", item.FinishType)
                );
        }
        static Assignment getAssignment(XElement assignment)
        {
            return new DO.Assignment()
            {
                Id = (int?)assignment.Element("Id") ?? throw new ArgumentNullException("Id element is missing"),
                CallId = (int?)assignment.Element("CallId") ?? throw new ArgumentNullException("CallId element is missing"),
                VolunteerId = (int?)assignment.Element("VolunteerId") ?? throw new ArgumentNullException("VolunteerId element is missing"),
                StarCall =  (DateTime?)assignment.Element("StarCall") ?? throw new ArgumentNullException("StarCall element is missing"),
                CompletionTime = (DateTime?)assignment.Element("CompletionTime") ?? throw new ArgumentNullException("CompletionTime element is missing"),
                FinishType = assignment.Element("FinishType") != null ?
                             Enum.Parse<CompletionType>(assignment.Element("FinishType")?.Value ?? throw new ArgumentNullException("FinishType element is missing")) :
                             (CompletionType?)null
            };
        }
        public void Create(Assignment item)
        {
            if (item.Id == 0)
                item = item.WithId(Config.NextAssignmentId);

            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            XElement newAssignment = createAssignmentElement(item) ?? throw new InvalidOperationException("Failed to create assignment element");
            assignmentRootElem.Add(newAssignment);

            XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
        }

        public void Delete(int id)
        {
            // Implementation Delete assignment by XElement

            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            XElement? assignmentElem = assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
            if (assignmentElem == null)
                throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist");

            assignmentElem.Remove();
            XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
        }

        public void DeleteAll()
        {
            // Implementation Delete assignment by XElement

            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
            assignmentRootElem.RemoveAll();
            XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
        }

        public Assignment? Read(int id)
        {
            // Implementation Read assignment by XElement
            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
            XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().
                                       FirstOrDefault(st => (int?)st.Element("Id") == id);
            if (assignmentElem == null)
                throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist");
            return getAssignment(assignmentElem);
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

            assignmentsRootElem.Add(new XElement("Assignment", createAssignmentElement(item)));

            XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
        }
    }
}
//XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().
//                                       FirstOrDefault(st => (int?)st.Element("Id") == id);
//return assignmentElem is null ? null : getAssignment(assignmentElem);