using System;
using System.Linq;
using Grasshopper.Kernel;

using DB = Autodesk.Revit.DB;

namespace RhinoInside.Revit.GH.Components
{
  public class AnalyseCurtainGrid : AnalysisComponent
  {
    public override Guid ComponentGuid => new Guid("D7B5C58E-8EDC-40C5-9BF8-078642090264");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override string IconTag => "ACG";

    public AnalyseCurtainGrid() : base(
      name: "Analyze Curtain Grid",
      nickname: "A-CG",
      description: "Analyze given curtain grid",
      category: "Revit",
      subCategory: "Analyze"
    )
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager manager)
    {
      manager.AddParameter(
        param: new Parameters.DataObject<DB.CurtainGrid>(),
        name: "Curtain Grid",
        nickname: "CG",
        description: "Curtain Grid",
        access: GH_ParamAccess.item
        );
    }

    protected override void RegisterOutputParams(GH_OutputParamManager manager)
    {
      // grid cells
      manager.AddParameter(
        param: new Parameters.DataObject<DB.CurtainCell>(),
        name: "Curtain Grid Cells",
        nickname: "CGC",
        description: "Grid cells generated by the given Curtain Grid",
        access: GH_ParamAccess.item
        );

      // grid parts
      manager.AddParameter(
        param: new Parameters.CurtainGridMullion(),
        name: "Curtain Grid Mullions",
        nickname: "CGM",
        description: "Grid mullion elements generated by the given Curtain Grid",
        access: GH_ParamAccess.item
        );
      manager.AddParameter(
        param: new Parameters.Element(),
        name: "Curtain Grid Panels",
        nickname: "CGP",
        description: "Grid panel elements generated by the given Curtain Grid",
        access: GH_ParamAccess.item
        );

      // grid lines
      // U
      manager.AddParameter(
        param: new Parameters.CurtainGridLine(),
        name: "Curtain Grid Lines Along U Axis",
        nickname: "CGUL",
        description: "Grid line elements generated by the given Curtain Grid along the U axis",
        access: GH_ParamAccess.item
        );
      // U grid properties
      manager.AddNumberParameter(
        name: "Curtain Grid Angle (U Axis / Grid 1)",
        nickname: "CGUA",
        description: "The angle for the U grid line pattern of the given curtain grid",
        access: GH_ParamAccess.item
        );
      manager.AddParameter(
        param: new Parameters.CurtainGridAlignType_ValueList(),
        name: "Curtain Grid Alignment Type (U Axis / Grid 1)",
        nickname: "CGUAT",
        description: "The alignment type for the U grid line pattern of the given curtain grid",
        access: GH_ParamAccess.item
        );
      manager.AddNumberParameter(
        name: "Curtain Grid Offset (U Axis / Grid 1)",
        nickname: "CGUO",
        description: "The offset for the U grid line pattern of the given curtain grid",
        access: GH_ParamAccess.item
        );

      // V
      manager.AddParameter(
        param: new Parameters.CurtainGridLine(),
        name: "Curtain Grid Lines Along V Axis",
        nickname: "CGVL",
        description: "Grid line elements generated by the given Curtain Grid along the V axis",
        access: GH_ParamAccess.item
        );
      // V grid properties
      manager.AddNumberParameter(
        name: "Curtain Grid Angle (V Axis / Grid 2)",
        nickname: "CGVA",
        description: "The angle for the V grid line pattern of the given curtain grid",
        access: GH_ParamAccess.item
        );
      manager.AddParameter(
        param: new Parameters.CurtainGridAlignType_ValueList(),
        name: "Curtain Grid Alignment Type (V Axis / Grid 2)",
        nickname: "CGVAT",
        description: "The alignment type for the V grid line pattern of the given curtain grid",
        access: GH_ParamAccess.item
        );
      manager.AddNumberParameter(
        name: "Curtain Grid Offset (V Axis / Grid 2)",
        nickname: "CGVO",
        description: "The offset for the V grid line pattern of the given curtain grid",
        access: GH_ParamAccess.item
        );

    }

    protected override void TrySolveInstance(IGH_DataAccess DA)
    {
      // get input
      Types.DataObject<DB.CurtainGrid> dataObj = default;
      if (!DA.GetData("Curtain Grid", ref dataObj))
        return;

      DB.CurtainGrid cgrid = dataObj.Value;

      DA.SetDataList("Curtain Grid Cells", cgrid.GetCurtainCells().Select(x => new Types.DataObject<DB.CurtainCell>(x, srcDocument: dataObj.Document)));
      DA.SetDataList("Curtain Grid Mullions", cgrid.GetMullionIds().Select(x => Types.CurtainGridMullion.FromElement(dataObj.Document.GetElement(x))));
      DA.SetDataList("Curtain Grid Panels", cgrid.GetPanelIds().Select(x => Types.Element.FromElement(dataObj.Document.GetElement(x))));

      // GetVGridLineIds returns grid lines perpendicular to V
      DA.SetDataList("Curtain Grid Lines Along U Axis", cgrid.GetVGridLineIds().Select(x => Types.CurtainGridLine.FromElement(dataObj.Document.GetElement(x))));
      DA.SetData("Curtain Grid Angle (U Axis / Grid 1)", cgrid.Grid1Angle);
      DA.SetData("Curtain Grid Alignment Type (U Axis / Grid 1)", new Types.CurtainGridAlignType(cgrid.Grid1Justification));
      DA.SetData("Curtain Grid Offset (U Axis / Grid 1)", cgrid.Grid1Offset);

      // GetUGridLineIds returns grid lines perpendicular to U
      DA.SetDataList("Curtain Grid Lines Along V Axis", cgrid.GetUGridLineIds().Select(x => Types.CurtainGridLine.FromElement(dataObj.Document.GetElement(x))));
      DA.SetData("Curtain Grid Angle (V Axis / Grid 2)", cgrid.Grid2Angle);
      DA.SetData("Curtain Grid Alignment Type (V Axis / Grid 2)", new Types.CurtainGridAlignType(cgrid.Grid2Justification));
      DA.SetData("Curtain Grid Offset (V Axis / Grid 2)", cgrid.Grid2Offset);
    }
  }
}
