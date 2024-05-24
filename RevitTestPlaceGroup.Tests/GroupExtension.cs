using Autodesk.Revit.DB;
using System.Linq;

namespace RevitTestPlaceGroup.Tests
{
    public static class GroupExtension
    {
        /// <summary>
        /// Change view by copying the group to the new view and deleting the original group
        /// </summary>
        /// <param name="group"></param>
        /// <param name="newView"></param>
        public static Group ChangeView(this Group group, View newView)
        {
            // Model Groups do not have an owner view
            if (group.OwnerViewId == ElementId.InvalidElementId)
                return group;

            // Group is already in the correct view
            if (group.OwnerViewId == newView.Id)
                return group;

            var document = group.Document;
            var view = document.GetElement(group.OwnerViewId) as View;

            var elementsToCopy = new[] { group.Id };

            var options = new CopyPasteOptions();

            var elements = ElementTransformUtils.CopyElements(view, elementsToCopy, newView, Transform.Identity, options);
            document.Delete(elementsToCopy);

            var newGroup = elements.Select(e => document.GetElement(e)).OfType<Group>().FirstOrDefault();
            return newGroup;
        }
    }
}
