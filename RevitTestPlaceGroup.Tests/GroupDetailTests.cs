using Autodesk.Revit.DB;
using NUnit.Framework;
using RevitTestPlaceGroup.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitTestPlaceGroup.Tests
{
    public class GroupDetailTests : OneTimeOpenDocumentTest
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
}
