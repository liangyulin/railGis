using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    class CTEFeature
    {
        public static void linesPointsLoad(string shpFileName)
        {
            string tMsg = String.Empty;
            IFeatureLayer66 cFeatureLayer = null;
            IFeatureGroups66 cFeatureLayerGroups = null;
            IFeatureGroup66 cFeatureGroupPoint = null;

            try
            {
                //
                // A. Instantiate Terra Explorer Globe
                //
                var sgworld = new SGWorld66();

                //
                // B.  Create shape file layer
                //
                {

                    // B1. Generate shape file connection string
                    string tShapeFileName = @"D:\GISData\jiqing\" + shpFileName + ".shp"; //Path.Combine(SamplesDirectory, @"Resources\ShapeFiles\PointLayers\Captials\World_Capital.shp");
                    string tConnectionString = String.Format("FileName={0};TEPlugName=OGR;", tShapeFileName);

                    // B2. Create point feature layer
                    cFeatureLayer = sgworld.Creator.CreateFeatureLayer(shpFileName, tConnectionString);
                }

                //
                // C. Display points as text label & set label text property to one of the shape text type attribute 
                //
                {
                    // C1. Set Point FeatureGroup DisplayAs property to label
                    cFeatureLayerGroups = cFeatureLayer.FeatureGroups;
                    cFeatureGroupPoint = cFeatureLayerGroups.Point;
                    cFeatureGroupPoint.DisplayAs = ObjectTypeCode.OT_LABEL;

                    // C2. Set label "Scale" property for visualization from high altitude
                    {
                        double dScale = 5.0;
                        cFeatureGroupPoint.SetProperty("Scale", dScale);
                    }


                    // C3. Set label "Text" property to the first found text attribute "Name" property
                    {
                        // Get layer data source attributes
                        var cFeatureLayerDataSource = cFeatureLayer.DataSourceInfo;
                        var cAttributes = cFeatureLayerDataSource.Attributes;
                        // Import all data source attributes
                        cFeatureLayerDataSource.Attributes.ImportAll = true;
                        //cFeatureLayerDataSource.
                        for (var attrIndex = 0; attrIndex < cFeatureLayerDataSource.Attributes.Count; attrIndex++)
                        {

                            IAttribute66 cAttribute = (IAttribute66)cFeatureLayerDataSource.Attributes[attrIndex];
                            //Console.Write(cAttribute.Name + cAttribute.Size);

                            if (cAttribute.Name == "TE_DESC")
                            {
                                cAttribute = (IAttribute66)cFeatureLayerDataSource.Attributes[attrIndex];
                                cFeatureGroupPoint.SetProperty("Text", "[" + cAttribute.Name + "]");

                                break;

                            }

                        }


                        // Iterate attributes, find first attribute of type text and set its name to label text
                        //for (int )
                        //foreach (IAttribute65 cAttribute in cFeatureLayerDataSource.Attributes)
                        //{
                        //    if (cAttribute.Type == AttributeTypeCode.AT_TEXT)
                        //    {
                        //        cFeatureGroupPoint.SetProperty("Text", String.Format("[{0}]", cAttribute.Name));
                        //        break;
                        //    }
                        //}
                    }
                }

                ////
                //// D. Perform spatial query
                ////
                //{
                //    // D1. Generate polygon geometry (Part of Europe)
                //    List<double> cSQVerticesArrayDbl = new List<double>() { -10.0,50.0,0.0,
                //                                                            -10.0,30.0,0.0,
                //                                                             30.0,30.0,0.0,
                //                                                             30.0,50.0,0.0 };

                //    // D2. Create polygon geometry from linear ring and call spatial query
                //    IFeatures66 cSQResFeatures = null;
                //    IGeometry cSQPlgGeometry = null;
                //    ILinearRing cSQRing = sgworld.Creator.GeometryCreator.CreateLinearRingGeometry(cSQVerticesArrayDbl.ToArray());
                //    cSQPlgGeometry = sgworld.Creator.GeometryCreator.CreatePolygonGeometry(cSQRing, null);
                //    cSQResFeatures = cFeatureLayer.ExecuteSpatialQuery(cSQPlgGeometry, IntersectionType.IT_INTERSECT);
                //}

                //
                // E. Load Feature layer in "Entire" mode
                //
                {
                    cFeatureLayer.Streaming = false;

                    cFeatureLayer.Load();
                    

                    //  cFeatureLayer.
                }


                ////
                //// F. FlyTo feature layer view point
                ////
                //{
                //    var cFlyToPos = cFeatureLayer.Position.Copy();
                //    cFlyToPos.Pitch = -89.0; // Set camera to look downward on polygon 
                //    cFlyToPos.X = 10.50;
                //    cFlyToPos.Y = 47.50;
                //    cFlyToPos.Distance = 3500000;
                //    sgworld.Navigate.FlyTo(cFlyToPos, ActionCode.AC_FLYTO);
                //}
            }
            catch (Exception ex)
            {
                tMsg = String.Format("PointLayerButton_Click Exception: {0}", ex.Message);
                Console.WriteLine(tMsg);
                //MessageBox.Show(tMsg);
            }
        }
    }
}
