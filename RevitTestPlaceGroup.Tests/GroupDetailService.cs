using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitTestPlaceGroup.Tests
{
    public class GroupDetailService
    {
        public IEnumerable<GroupType> SelectType(Document document)
        {
            var elements = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_IOSDetailGroups)
                .WhereElementIsElementType()
                .OfClass(typeof(GroupType))
                .OfType<GroupType>();

            return elements;
        }

        public IEnumerable<Group> Select(Document document)
        {
            var elements = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_IOSDetailGroups)
                .WhereElementIsNotElementType()
                .OfClass(typeof(Group))
                .OfType<Group>();

            return elements;
        }

        public Group CreateGroupDetail(GroupType groupType, View view, XYZ location)
        {
            var document = groupType.Document;

            Group groupBase = Select(document).FirstOrDefault();

            if (groupBase is null)
            {
                throw new Exception("No Detail Group found in the document.");
            }

            var groupView = document.GetElement(groupBase.OwnerViewId) as View;

            var elementsToCopy = new[] { groupBase.Id };
            var options = new CopyPasteOptions();
            var elements = ElementTransformUtils.CopyElements(groupView, elementsToCopy, view, Transform.Identity, options);

            var newGroup = elements.Select(e => document.GetElement(e)).OfType<Group>().FirstOrDefault();
            newGroup.ChangeTypeId(groupType.Id);

            var locationPoint = newGroup.Location as LocationPoint;
            var oldLocation = locationPoint.Point;
            locationPoint.Point = location;

            return newGroup;
        }
    }
}
