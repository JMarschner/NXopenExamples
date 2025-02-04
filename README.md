# NXopenExamples
Diverse NXopen- bzw. Journal-Beispiele (c# und vba) für die Anpassung und Erweiterung von Siemens NX. Diese Beispiele stellen nur einen kleinen Teil dessen dar, was mit NXOpen möglich ist. Sie sind so konzipiert, dass diese einfach zu verstehen/modifizieren sind und unter NX auch unkompiliert, sowie ohne Authorenlizenz ausgeführt werden können. 

## Grundinstallation
 1. VSCode (https://code.visualstudio.com/) installieren
 2. .Net installieren (https://dotnet.microsoft.com/)
 3. Repository klonen/herunterladen und entpacken oder alternativ ein eigenes Projekt erstellen. (Anleitung im nachfolgenden Punkt beschrieben.)

### Ein eigenes NXOpen-Projekt in VSCode anlegen
 1. Mittels "dotnet new console" ein neues Projekt anlegen.
 2. In der *.csproj Referenzen zu NXOpen hinzufügen. (Die Referenzen können aus einem Beispielprojekt kopiert werden. Die Beispiele finden sich z.B. unter %UGII_BASE_DIR%/UGOPEN/NXOpenExamples/CS/Templates.)
 3. Bei der Bearbeitung der *.cs-Daten sollte nun Intellisense zur Verfügung stehen.

## Scripte/Journale
### ODBC (ODBC/ODBCTest.cs)
Ein Journal um eine grundlegende Verbindung zu einer Datenbank herzustellen. Die Verbindung erfolgt über die ODBC-Schnittstelle und ein entsprechender Datenbanktreiber wird vorausgesetzt bzw. muss auf dem ausführenden System installiert sein. Das Journal kann zum Beispiel dafür verwendet werden um Stücklisten, Teiledaten, Materialien oder anderes mit einer Datenbank zu synchronisieren. Auch die Synchronisation zwischen Siemens NX und PDM/MES/ERP ist hiermit möglich. Das Journal muss jedoch noch um die eigentliche Programmlogik erweitert werden.