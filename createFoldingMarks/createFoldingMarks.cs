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
        // public static NXOpen.ListingWindow theLW;

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
                // theLW = theSession.ListingWindow;

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

            NXOpen.Sketch[] sketches = currentDrawing.GetDraftingSketches();
            
            SketchInDraftingBuilder sketchInDraftingBuilder = workPart.Sketches.CreateSketchInDraftingBuilder();
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