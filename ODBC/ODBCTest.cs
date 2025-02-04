/* ODBCTest.cs
 * 
 * Ein Journal um eine grundlegende Verbindung zu einer Datenbank herzustellen. Die Verbindung erfolgt über die
 * ODBC-Schnittstelle und ein entsprechender Datenbanktreiber wird vorausgesetzt bzw. muss auf dem ausführenden 
 * System installiert sein. Das Journal kann zum Beispiel dafür verwendet werden um Stücklisten, Teiledaten, 
 * Materialien oder anderes mit einer Datenbank zu synchronisieren. Auch die Synchronisation zwischen Siemens NX 
 * und PDM/MES/ERP ist hiermit möglich. Das Journal muss jedoch noch um die eigentliche Programmlogik erweitert
 * werden.
 *
 */

using System;
// using System.IO;
// using System.Text;
using System.Data.Odbc;
using NXOpen;
// using NXOpen.UF;


namespace NXToolsJoergMarschner
{
    internal class ODBCTest
    {
        // Alternativ: "DSN=xxxx"
        private static string connectionString = "Driver={MariaDB ODBC 3.2 Driver};" +  // Name vom ODBC-Treiber
                                                "SERVER=???;" +                         // IP oder FQDN vom SQL-Server
                                                "USER=???;" +                           // SQL-Benutzername
                                                "PASSWORD=???;" +                       // Passwort vom SQL-Benutzername
                                                "DATABASE=???;";                        // Name der Datenbank
        
        private static NXOpen.Session theSession;
        public static NXOpen.ListingWindow theLW;
        // private static NXOpen.Part workPart;
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

                // workPart = theSession.Parts.Work;
                // displayPart = theSession.Parts.Display;

                // theUI = UI.GetUI();
                // theUfSession = UFSession.GetUFSession();

            } catch (NXOpen.NXException ex) {
                UI.GetUI().NXMessageBox.Show("Error / Fehler", NXOpen.NXMessageBox.DialogType.Error, ex.Message);
                return;
            }

            // Löschen oder auskommentieren um die Debuginformationen zu unterdrücken
            theLW.Open();
            
            theLW.WriteFullline("ODBC-Verbindungstest.");
            theLW.WriteFullline("");

            // Verbindung zur Datenbank herstellen
            OdbcConnection connection;
            try {
                connection =  new OdbcConnection(connectionString);
                connection.Open();
                theLW.WriteFullline("Verbindung hergestellt!");
            } catch (System.Exception ex) {
                UI.GetUI().NXMessageBox.Show("Error / Fehler", NXOpen.NXMessageBox.DialogType.Error, "Die Verbindung zur Datenbank konnte nicht hergestellt werden.");
                theLW.WriteFullline(ex.Message);
                theLW.WriteFullline(ex.ToString());
                return;
            }
            


            //------------------------------------------------------------------------------
            // Hier kann dann die Programmlogik eingefügt werden. Die untenstehenden Zeilen dienen nur als Beispiel. ;-)
                OdbcCommand command = new OdbcCommand("SELECT * FROM tabelle");
                command.Connection = connection;
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read()) {
                    int column = 0;
                    theLW.WriteFullline(reader.GetName(column) + ": " + reader.GetFieldValue<string>(column));
                }
                reader.Close();
            //------------------------------------------------------------------------------



            // Verbindung schließen und Journal beenden.
            try {
                connection.Close();
            } catch (Exception ex) {
                UI.GetUI().NXMessageBox.Show("Error / Fehler", NXMessageBox.DialogType.Error, "Die Verbindung zur Datenbank konnte nicht ordnungsgemäß beendet werden.");
                theLW.WriteFullline(ex.Message);
                theLW.WriteFullline(ex.ToString());
                return;
            }
        }

        /* Unload-Fuktion, wird von NX automatisch aufgerufen */
        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }
    }
}