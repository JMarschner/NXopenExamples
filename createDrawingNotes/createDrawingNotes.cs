/* CreateDWGNote.cs
 * 
 * Makro zum erstellen von assoziativen Texthinweisen auf den Zeichnungen.
 * 
 */

using System;
// using System.IO;
// using System.Text;
using NXOpen;
using NXOpen.UF;

public class CreateDrawingNotes {
    private static NXOpen.Session theSession;
    // private static NXOpen.ListingWindow theLW;

    private static NXOpen.Part workPart;
    // private static NXOpen.Part displayPart;

    private static NXOpen.UI theUI;
    private static NXOpen.UF.UFSession theUfSession;


    //------------------------------------------------------------------------------
    // Hauptprogramm
    //------------------------------------------------------------------------------
    public static void Main(string[] args) {
        
        try {
            theSession = NXOpen.Session.GetSession();        
            workPart = theSession.Parts.Work;
            // displayPart = theSession.Parts.Display;
            // theLW = theSession.ListingWindow;

            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            
        } catch (NXOpen.NXException ex) {
            UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, ex.Message);
            return;
        }

        // ----------------------------------------------
        //  UnDo-Marke setzen
        // ----------------------------------------------
        NXOpen.Session.UndoMarkId markId1;
        markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "CreateDrawingNotes");


        // ----------------------------------------------
        //  Eine Komponente & eine Ansicht auswählen
        // ----------------------------------------------        
        NXObject component = (NXObject) selectComponent();
        NXOpen.Drawings.DraftingView view = (NXOpen.Drawings.DraftingView) selectView();

        if (component==null || view==null) {
            UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, "Fehler bei der Auswahl. NULL-Pointer");
            return;
        }


        // ----------------------------------------------
        //  Den Texthinweis erstellen und ablegen
        // ----------------------------------------------
        try {
            setNote(component, view);
        } catch (NXOpen.NXException ex) {
            UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, ex.Message);
            theSession.UndoToMark(markId1,"CreateDrawingNotes");
            return;
        }
    }


    //------------------------------------------------------------------------------
    //  Eine Komponente auswählen
    //------------------------------------------------------------------------------
    public static TaggedObject selectComponent() {
        theUfSession.Ui.SetCursorView(0); // Für das auswählen innerhalb einer Zeichnung notwendig

        TaggedObject tagobj;
        Point3d cursor;

        Selection.MaskTriple[] maskArray = new Selection.MaskTriple[1];
        maskArray[0].Type = NXOpen.UF.UFConstants.UF_component_type; // 63
        maskArray[0].Subtype = NXOpen.UF.UFConstants.UF_component_subtype;

        Selection.Response resp = theUI.SelectionManager.SelectTaggedObject(
            "Komponente auswählen",
            "Komponente auswählen",
            Selection.SelectionScope.AnyInAssembly,
            Selection.SelectionAction.ClearAndEnableSpecific,
            false,
            false,
            maskArray,
            out tagobj,
            out cursor);


        if (resp == Selection.Response.ObjectSelected || resp == Selection.Response.ObjectSelectedByName) {
            return tagobj;
        }
        
        return null;
    }

    public static TaggedObject selectView() {
        theUfSession.Ui.SetCursorView(0); // Für das auswählen innerhalb einer Zeichnung notwendig

        TaggedObject tagobj;
        Point3d cursor;

        Selection.MaskTriple[] maskArray = new Selection.MaskTriple[1];
        maskArray[0].Type = NXOpen.UF.UFConstants.UF_view_type; // 60
        maskArray[0].Subtype = NXOpen.UF.UFConstants.UF_all_subtype;

        Selection.Response resp = theUI.SelectionManager.SelectTaggedObject(
            "Ansicht auswählen",
            "Ansicht auswählen",
            Selection.SelectionScope.AnyInAssembly,
            Selection.SelectionAction.ClearAndEnableSpecific,
            false,
            false,
            maskArray,
            out tagobj,
            out cursor);


        if (resp == Selection.Response.ObjectSelected || resp == Selection.Response.ObjectSelectedByName) {
            return tagobj;
        }

        return null;
    }

    //------------------------------------------------------------------------------
    //  Den Texthinweis erstellen
    //------------------------------------------------------------------------------
    public static void setNote(NXObject component, NXOpen.Drawings.DraftingView view) {
        // ----------------------------------------------
        //  Note-Builder erstellen
        // ----------------------------------------------
        NXOpen.Annotations.DraftingNoteBuilder draftingNoteBuilder1;
        NXOpen.Annotations.DraftingNoteBuilder draftingNoteBuilder2;

        NXOpen.Annotations.SimpleDraftingAid nullNXOpen_Annotations_SimpleDraftingAid = null;
        
        draftingNoteBuilder1 = workPart.Annotations.CreateDraftingNoteBuilder(nullNXOpen_Annotations_SimpleDraftingAid);
        draftingNoteBuilder1.Origin.Anchor = NXOpen.Annotations.OriginBuilder.AlignmentPosition.BottomCenter;

        // ----------------------------------------------
        //  Text erstellen
        // ----------------------------------------------

        string[] text1 = new string[5];
        text1[0] = "<C1.25><D1><I20>Teil  - <I0><D><C>";
        text1[1] = "<C0.75>()<C>";
        text1[2] = "Material: ";
        text1[3] = "Abmessungen: ";
        text1[4] = "Maßstab: ";

        draftingNoteBuilder1.Text.TextBlock.SetText(text1);

        // ----------------------------------------------
        //  Attribute dem Text hinzufügen
        // ----------------------------------------------

        try {
            AddPartAttributeToText(draftingNoteBuilder1, component, "PART_NAME", 1, 25);
            AddPartAttributeToText(draftingNoteBuilder1, component, "NUMBER", 1, 22);
            AddPartAttributeToText(draftingNoteBuilder1, component, "$PART_NAME", 2, 9);
            AddPartAttributeToText(draftingNoteBuilder1, component, "MATERIAL_TOOLING", 3, 11);
            AddPartAttributeToText(draftingNoteBuilder1, component, "MW_STOCK_SIZE", 4, 14);
            AddPartAttributeToText(draftingNoteBuilder1, view, "VWSCALE", 5, 10);
        } catch (NXOpen.NXException ex) {
            throw;
        }
        

        // ----------------------------------------------
        //  Drafting-Note mit Ansicht verknüpfen und platzieren
        // ----------------------------------------------
        NXOpen.Annotations.Annotation.AssociativeOriginData assDate = new NXOpen.Annotations.Annotation.AssociativeOriginData();
        assDate.View = view;
        assDate.OriginType = NXOpen.Annotations.AssociativeOriginType.RelativeToView;
        draftingNoteBuilder1.Origin.SetAssociativeOrigin(assDate);

        Double[] viewBorder = new Double[4];
        theUfSession.Draw.AskViewBorders(view.Tag, viewBorder);
        /* viewBorder[]
         * [0] - X min
         * [1] - Y min
         * [2] - X max
         * [3] - Y max
         */
        NXOpen.Point3d point1 = new NXOpen.Point3d((viewBorder[0]+viewBorder[2])/2, viewBorder[3]+10, 0.0);
        //NXOpen.Point3d point1 = new NXOpen.Point3d((viewBorder[0]+viewBorder[2])/2, viewBorder[3]+30, 0.0);
        draftingNoteBuilder1.Origin.Origin.SetValue(null, view, point1);


        // ----------------------------------------------
        //  Drafting-Note absetzen
        // ----------------------------------------------
        NXOpen.Annotations.Note note1 = (NXOpen.Annotations.Note) draftingNoteBuilder1.Commit();
        draftingNoteBuilder1.Destroy();


        // ----------------------------------------------
        //  Text zentrieren
        // ----------------------------------------------

        NXOpen.DisplayableObject[] dispOjects1 = new NXOpen.DisplayableObject[1];
        dispOjects1[0] = note1;

        NXOpen.Annotations.EditSettingsBuilder editSettingsBuilder1;
        editSettingsBuilder1 = workPart.SettingsManager.CreateAnnotationEditSettingsBuilder(dispOjects1);
        editSettingsBuilder1.AnnotationStyle.LetteringStyle.HorizontalTextJustification = NXOpen.Annotations.TextJustification.Center;
        editSettingsBuilder1.Commit();
        editSettingsBuilder1.Destroy();
    }

    //------------------------------------------------------------------------------
    //  Attribute prüfen, ggf. erstellen und dem Text hinzufügen...
    //------------------------------------------------------------------------------
   
    public static void AddPartAttributeToText(NXOpen.Annotations.DraftingNoteBuilder draftingNoteBuilder1, NXObject component, string attributename, int line, int cursorPos) {
        try {
            component.GetUserAttributeAsString(attributename, NXObject.AttributeType.Any, -1);
        } catch {
            try {
                NXOpen.NXObject[] attributeObj = new NXOpen.NXObject[1];
                attributeObj[0] = (NXObject)component.Prototype;

                NXOpen.AttributePropertiesBuilder attributePropertiesBuilder;
                attributePropertiesBuilder = theSession.AttributeManager.CreateAttributePropertiesBuilder(workPart, attributeObj, AttributePropertiesBuilder.OperationType.None);
                attributePropertiesBuilder.Title = attributename;
                
                attributePropertiesBuilder.StringValue = "???";

                attributePropertiesBuilder.Commit();
                attributePropertiesBuilder.Destroy();
            } catch (NXOpen.NXException ex) {
                UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, ex.Message);
                throw;
            }
        }
        draftingNoteBuilder1.Text.TextBlock.AddAttributeReference(component, attributename, false, line, cursorPos);
    }

    //------------------------------------------------------------------------------
    //  Unload-Fuktion, wird von NX automatisch aufgerufen
    //------------------------------------------------------------------------------
    public static int GetUnloadOption(string dummy) { 
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately; 
    }
}