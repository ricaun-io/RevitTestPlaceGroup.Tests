# RevitTestPlaceGroup.Tests

[![Revit 2021](https://img.shields.io/badge/Revit-2021+-blue.svg)](../..)
[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](../..)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

RevitTest project to test workaround to create Detail Groups in Revit without ActiveView.

This project use the [ricaun.RevitTest](https://ricaun.com/RevitTest) test framework.

## Problem Description

Project is based on the problem described in the [Revit API Forum](https://forums.autodesk.com/t5/revit-api-forum/placing-detail-group-in-legend-view-without-access-to-ui-design/td-p/12792753).

The method `PlaceGroup` place the Group detail in the `ActiveView`, but the `ActiveView` is not available the `Group` is created in an invalid view (`OwnerViewId` is `ElementId.InvalidElementId`).

A workaround is to use a `Group` in the document and copy to the view you want, and change the type of the `Group`, than change the location.

The method below does this workaround:
```c#
public Group CreateGroupDetail(GroupType groupType, View view, XYZ location);
```

* [GroupDetailService.cs](RevitTestPlaceGroup.Tests/GroupDetailService.cs)

If there is not detail `Group` in the document, the method `CreateGroupDetail` throws an `Exception`. 

*Could be possible to open a document copy a view with a detail `Group` and paste in the main document, to make the method to work without a `Group` created. This is not implemented in this project.*

## Video

Videos in English about this project.

[![VideoIma1]][Video1]

## License

This project is [licensed](LICENSE) under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](../../stargazers)!

[Video1]: https://youtu.be/VhaFv_UaHGU
[VideoIma1]: https://img.youtube.com/vi/VhaFv_UaHGU/mqdefault.jpg