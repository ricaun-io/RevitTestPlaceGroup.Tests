using Autodesk.Revit.DB;
using NUnit.Framework;
using RevitTestPlaceGroup.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitTestPlaceGroup.Tests
{
    public class Tests : OneTimeOpenDocumentTest
    {
        GroupDetailService groupDetailService = new GroupDetailService();
        protected override string FileName => "Files/ProjectGroupDetail2021.rvt";

        [Test]
        public void Test_Document_Save()
        {
            Assert.IsNotNull(document);
            Console.WriteLine(document.Title);

            document.SaveAs($"Files/{document.Title}_{application.VersionNumber}.rvt", new SaveAsOptions() { MaximumBackups = 1, OverwriteExistingFile = true });
        }

        [Test]
        public void PlaceGroupDetail_ShouldBe_InvalidElementId()
        {
            var groupType = groupDetailService.SelectType(document).FirstOrDefault();
            Assert.IsNotNull(groupType);

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("PlaceGroup");
                var group = document.Create.PlaceGroup(new XYZ(0, 0, 0), groupType);

                Assert.IsNotNull(group);
                Assert.AreEqual(ElementId.InvalidElementId, group.OwnerViewId);

                transaction.Commit();
            }

        }

        [Test]
        public void CreateGroupDetail_ShouldBe_ValidElementId()
        {
            var viewLegend = SelectViewLegend().FirstOrDefault();

            Assert.IsNotNull(viewLegend);

            var groupTypes = groupDetailService.SelectType(document);

            foreach (var groupType in groupTypes)
            {
                using (Transaction transaction = new Transaction(document))
                {
                    transaction.Start("PlaceGroup");
                    var group = groupDetailService.CreateGroupDetail(groupType, viewLegend, new XYZ(0, 0, 0));

                    Assert.IsNotNull(group);
                    Assert.AreEqual(viewLegend.Id, group.OwnerViewId);

                    transaction.Commit();
                }
            }
        }

        private IEnumerable<View> SelectViewLegend()
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .OfType<View>()
                .Where(e => e.ViewType == ViewType.Legend);
        }

        [Test]
        public void ShowGroups()
        {
            foreach (var groupType in groupDetailService.SelectType(document))
            {
                Console.WriteLine(groupType.Name);
            }
        }

    }

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

            // Console.WriteLine($"CreateGroupDetail using {groupBase.GroupType.Name} [{groupBase.GroupType.Id}] to {groupType.Name} [{groupType.Id}]");

            var groupView = document.GetElement(groupBase.OwnerViewId) as View;

            var elementsToCopy = new[] { groupBase.Id };
            var options = new CopyPasteOptions();
            var elements = ElementTransformUtils.CopyElements(groupView, elementsToCopy, view, Transform.Identity, options);

            var newGroup = elements.Select(e => document.GetElement(e)).OfType<Group>().FirstOrDefault();
            newGroup.ChangeTypeId(groupType.Id);

            var locationPoint = newGroup.Location as LocationPoint;
            var oldLocation = locationPoint.Point;
            locationPoint.Point = location;

            // Console.WriteLine($"CreateGroupDetail location {oldLocation} to {locationPoint.Point}");

            return newGroup;
        }

        private static Group ChangeView(Group group, View newView)
        {
            // Model Groups do not have an owner view
            if (group.OwnerViewId == ElementId.InvalidElementId)
                return group;

            // Group is already in the correct view
            if (group.OwnerViewId == newView.Id)
                return group;

            Console.WriteLine($"ChangeView to {group.OwnerViewId}");

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
