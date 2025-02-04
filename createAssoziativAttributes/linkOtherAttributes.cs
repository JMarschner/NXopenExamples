/* linkOtherAttributes.cs
 * 
 * Dieses Journal durchsucht alle geöffneten Teile nach einem bestimmten Attribut (z.B. nach "UM_TOP", der obersten Baugruppe 
 * eines Moldwizard-Projekts).
 * 
 * In diesem Bauteil werden dann assoziative Ausdrucke erstellt, welche auf die anzugebenden Teileattribute verweisen 
 * (z.B. "UM_PROJECT", der Projektnummer eines Moldwizard-Projekts). Die so erstellten Ausdrücke werden dann assoziativ in das
 * aktuell aktive Bauteil gelinkt.
 * 
 * Das Makro kann z.B. dafür genutzt werden um Projektinformationen in Unterbaugruppen oder Einzelteilen verfügbar zu machen. 
 * Mit den Ausdrücken lassen sich dann assoziative Formelemente aufbauen (z.B. Teilebeschriftungen/Kennzeichnungen).
 * 
 */

using System;
// using System.IO;
// using System.Text;
using NXOpen;
// using NXOpen.UF;

namespace NXToolsJoergMarschner
{
    internal class LinkOtherAttributes
    {
        private static string attributeToSearch = "UM_TOP";
        private static string[] attributesToLink = {"UM_PROJECT", "CUSTOMER", "DESIGNER"};

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


            // ----------------------------------------------
            //  Alle geladene Einzelteile nach einem bestimmten Attribut durchsuchen (z.B. nach "UM_TOP", der obersten 
            //  Baugruppe eines Moldwizard-Projekts), um dann im aktuell geöffneten Teil eine assoziativ verlinktes Attribut
            //  zu erstellen (z.B. von "UM_PROJECT", der Projektnummer eines Moldwizard-Projekts). 
            // ----------------------------------------------
            NXOpen.PartCollection partCollection = theSession.Parts;
                
            // Alle in der NX-Sitzung geladenen Teile nach dem Ursprungsteil durchsuchen.
            foreach (NXOpen.BasePart basePart in (NXOpen.BasePart[]) partCollection.ToArray()) {                     
                if (basePart.HasUserAttribute(attributeToSearch, NXObject.AttributeType.String, -1)) {                    
                    try { 
                        // Im Ursprungsteil für jedes Attribut einen assoziativen Ausdruck anlegen.
                        NXOpen.Expression[] sourceExpressions = new NXOpen.Expression[attributesToLink.Length];
                        for (int i = 0; i < attributesToLink.Length; i++) {
                            sourceExpressions[i] = basePart.Expressions.GetAttributeExpression(basePart, attributesToLink[i], NXOpen.NXObject.AttributeType.String, -1);
                        }

                        // Im aktuellen Arbeitsteil die teileübergreifenden Ausdrücke erstellen.
                        NXOpen.InterpartExpressionsBuilder interpartExpressionsBuilder = workPart.Expressions.CreateInterpartExpressionsBuilder();
                        interpartExpressionsBuilder.SetExpressions(sourceExpressions, attributesToLink);
                        interpartExpressionsBuilder.Commit();
                        interpartExpressionsBuilder.Destroy();
                    } catch (NXException ex) {
                        UI.GetUI().NXMessageBox.Show("Fehler!", NXMessageBox.DialogType.Error, ex.Message.ToString());
                        // Bei einem Fehler zu derUnDo-Marke zurückkehren (Alle Änderungen verwerfen)
                        theSession.UndoToMark(undoMarkId, "CreateAssoziativAttributes");
                        return;
                    }
                }
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
