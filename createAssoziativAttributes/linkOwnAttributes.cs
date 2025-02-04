/* linkOwnAttributes.cs
 * 
 * Dieses Journal erstellt assoziative Ausdrucke von Teileattributen innerhalb eines aktiven Teils. Es eignet sich 
 * um z.B. Stücklistenattribute für die Konstruktion verfügbar zu machen. Beispielsweise kann damit das Material oder
 * die Teilenummer für eine Teilebeschriftungen/Kennzeichnungen am Bauteil weiterverwendet werden.
 * 
 */

using System;
// using System.IO;
// using System.Text;
using NXOpen;
// using NXOpen.UF;

namespace NXToolsJoergMarschner
{
    internal class linkOwnAttributes
    {
        private static string[] attributesToLink = {"MATERIAL_TOOLING"};

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


            // ----------------------------------------------
            //  Die gewünschten assoziativen Ausdrücke/Attribute erstellen
            // ----------------------------------------------
            try {
                // Im aktiiven Teil für jedes Attribut einen assoziativen Ausdruck anlegen.
                NXOpen.Expression[] sourceExpressions = new NXOpen.Expression[attributesToLink.Length];
                for (int i = 0; i < attributesToLink.Length; i++) {
                    sourceExpressions[i] = workPart.Expressions.GetAttributeExpression(workPart, attributesToLink[i], NXOpen.NXObject.AttributeType.String, -1);
                }
            } catch (System.Exception ex) {
                UI.GetUI().NXMessageBox.Show("Fehler!", NXMessageBox.DialogType.Error, ex.Message.ToString());
                // Bei einem Fehler zu derUnDo-Marke zurückkehren (Alle Änderungen verwerfen)
                theSession.UndoToMark(undoMarkId, "CreateAssoziativAttributes");
                return;
            }
            
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
