/* createFoldingMarks.cs
 * 
 * 
 * 
 */

using System;
// using System.IO;
// using System.Text;
using NXOpen;
// using NXOpen.UF;

namespace NXToolsJoergMarschner
{
    internal class createFoldingMarks
    {
        private static NXOpen.Session theSession;
        public static NXOpen.ListingWindow theLW;

        private static NXOpen.Part workPart;
        // private static NXOpen.Part displayPart;

        // private static NXOpen.UI theUI;
        // private static NXOpen.UF.UFSession theUfSession;


        //------------------------------------------------------------------------------
        // Hauptprogramm
        //------------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            try {
                theSession = NXOpen.Session.GetSession();
                theLW = theSession.ListingWindow;

                workPart = theSession.Parts.Work;
                // displayPart = theSession.Parts.Display;

                // theUI = UI.GetUI();
                // theUfSession = UFSession.GetUFSession();

            } catch (NXOpen.NXException ex) {
                UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, ex.Message);
                return;
            }

            // ----------------------------------------------
            //  UnDo-Marke setzen
            // ----------------------------------------------
            Session.UndoMarkId undoMarkId = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "CreateAssoziativAttributes");

            NXOpen.Drawings.DrawingSheet currentDrawing = workPart.DrawingSheets.CurrentDrawingSheet;
            if (currentDrawing == null) {
                UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, "Aktuell ist kein Zeichnungsblatt geöffnet.");
                return;
            }

            double SheetLength = currentDrawing.Length;
            double SheetHeight = currentDrawing.Height;

            currentDrawing.ActivateForSketching();

            // NXOpen.Sketch[] sketches = currentDrawing.GetDraftingSketches();
            
            SketchInDraftingBuilder sketchInDraftingBuilder = workPart.Sketches.CreateSketchInDraftingBuilder();
            
            NXOpen.Sketch sketch = (NXOpen.Sketch) sketchInDraftingBuilder.Commit();
            sketch.Activate(Sketch.ViewReorient.False);
            sketchInDraftingBuilder.Destroy();

            // Rahmen erstellen
            {
                NXOpen.Point3d[] pt = new Point3d[5];
                pt[0] = new Point3d(1,              1,              0);
                pt[1] = new Point3d(SheetLength-1,  1,              0);
                pt[2] = new Point3d(SheetLength-1,  SheetHeight-1,  0);
                pt[3] = new Point3d(1,              SheetHeight-1,  0);

                NXOpen.Line[] line = new Line[4];
                line[0] = workPart.Curves.CreateLine(pt[0], pt[1]);
                line[1] = workPart.Curves.CreateLine(pt[1], pt[2]);
                line[2] = workPart.Curves.CreateLine(pt[2], pt[3]);
                line[3] = workPart.Curves.CreateLine(pt[3], pt[0]);
                
                sketch.AddGeometry(line[0], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
                sketch.AddGeometry(line[1], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
                sketch.AddGeometry(line[2], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
                sketch.AddGeometry(line[3], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
            }
            

            // Lochmarkierung >= A4 hinzufügen
            if (SheetHeight >= 297) {
                NXOpen.Point3d[] pt = new Point3d[2];
                pt[0] = new Point3d(1,  297/2, 0);
                pt[1] = new Point3d(15, 297/2, 0);
                NXOpen.Line line = workPart.Curves.CreateLine(pt[0], pt[1]);
                sketch.AddGeometry(line, NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
            } 
 

            // Faltmarken Y erstellen
            int i = 1;
            while (SheetHeight > i * 297) {
                NXOpen.Point3d[] pt = new Point3d[4];
                pt[0] = new Point3d(1,              297*i,          0);
                pt[1] = new Point3d(5,              297*i,          0);
                pt[2] = new Point3d(SheetLength-1,  297*i,          0);
                pt[3] = new Point3d(SheetLength-5,  297*i,          0);

                NXOpen.Line[] line = new Line[2];
                line[0] = workPart.Curves.CreateLine(pt[0], pt[1]);
                line[1] = workPart.Curves.CreateLine(pt[2], pt[3]);
                
                sketch.AddGeometry(line[0], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
                sketch.AddGeometry(line[1], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);

                i++;
            }

            // Faltmarken X erstellen
            double[] faltungen;
            if (SheetLength <= 210) { // A4 Hochformat oder kleiner
                faltungen = new double[] {};
            } else if (SheetLength == 297) { // A4 Quer
                faltungen = new double[] {20, 125, 230};
            } else if (SheetLength == 420) { // A3
                faltungen = new double[] {20, 125, 230};
            }else if (SheetLength == 594) { // A2
                faltungen = new double[] {20, 212, 404};
            } else if (SheetLength == 841) { // A1
                faltungen = new double[] {20, 210, 400, 525.5, 651};
            } else if (SheetLength == 1189) { // A0
                faltungen = new double[] {20, 210, 400, 590, 780, 889.5, 999};
            } else { // Sonderbreiten/-größen die nicht oben aufgelistet sind.
                faltungen = new double[] {20};
                int index = 0;
                
                // Blattbreite - Schriftfeld - Faltungen von links muss weniger als 380mm ergeben. Ansonsten aller 190mm Faltung hinzufügen...
                while ((SheetLength - 190 - faltungen[index]) > 380) { 
                    Array.Resize(ref faltungen, faltungen.Length+1);
                    faltungen[index+1] = faltungen[index]+190;
                    index++;
                }

                if (index % 2 == 0) { // Bei gerader Anzahl von Faltungen
                    Array.Resize(ref faltungen, faltungen.Length+2);
                    faltungen[index+1] = (SheetLength - 190 - faltungen[index]) / 2 + faltungen[index];
                    faltungen[index+2] = (SheetLength - 190 - faltungen[index]) / 2 + faltungen[index+1];
                } else {
                    Array.Resize(ref faltungen, faltungen.Length+3);
                    faltungen[index+1] = faltungen[index] + 190;
                    faltungen[index+2] = (SheetLength - 190 - faltungen[index+1]) / 2 + faltungen[index+1];
                    faltungen[index+3] = (SheetLength - 190 - faltungen[index+1]) / 2 + faltungen[index+2];
                }
            }

            foreach (double faltung in faltungen) {
                NXOpen.Point3d[] pt = new Point3d[4];
                NXOpen.Line[] line = new Line[2];
                
                pt[0] = new Point3d(faltung,     1,                  0);
                pt[1] = new Point3d(faltung,     5,                  0);
                pt[2] = new Point3d(faltung,     SheetHeight-1,      0);
                pt[3] = new Point3d(faltung,     SheetHeight-5,      0);

                line[0] = workPart.Curves.CreateLine(pt[0], pt[1]);
                line[1] = workPart.Curves.CreateLine(pt[2], pt[3]);
                
                sketch.AddGeometry(line[0], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
                sketch.AddGeometry(line[1], NXOpen.Sketch.InferConstraintsOption.InferNoConstraints);
            } 

            // Skizze/Zeichnung updaten
            sketch.Update();
            // sketch.UpdateNavigator(); // NX2406
            sketch.Deactivate(Sketch.ViewReorient.False, Sketch.UpdateLevel.SketchOnly);
            sketch.SetName("FoldingMarks");

            // Skizze rot einfärben und Linienstärke anpassen
            NXOpen.DisplayModification displayModification = theSession.DisplayManager.NewDisplayModification();
            
            displayModification.ApplyToAllFaces = true;
            displayModification.ApplyToOwningParts = false;
            // displayModification.EndPointDisplayState = false; // NX2412
            
            displayModification.NewColor = 186;
            displayModification.NewWidth = DisplayableObject.ObjectWidth.Thin;
            
            NXOpen.DisplayableObject[] displayableObjects = new NXOpen.DisplayableObject[] {sketch};
            displayModification.Apply(displayableObjects);
        }


        //------------------------------------------------------------------------------
        //  Unload-Fuktion, wird von NX automatisch aufgerufen
        //------------------------------------------------------------------------------
        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }
    }
}