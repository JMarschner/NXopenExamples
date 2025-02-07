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
### ODBC-/Datenbankverbindungen 
#### (ODBC/ODBCTest.cs)
Ein Journal um eine grundlegende Verbindung zu einer Datenbank herzustellen. Die Verbindung erfolgt über die ODBC-Schnittstelle und ein entsprechender Datenbanktreiber wird vorausgesetzt bzw. muss auf dem ausführenden System installiert sein. 

Das Journal kann zum Beispiel dafür verwendet werden um Stücklisten, Teiledaten, Materialien oder anderes mit einer Datenbank zu synchronisieren. Auch die Synchronisation zwischen Siemens NX und PDM/MES/ERP ist hiermit möglich. Das Journal muss jedoch noch um die eigentliche Programmlogik erweitert werden.

### Erstellen von Faltmarkierungen nach DIN 824
#### (createFoldingMarks/createFoldingMarks.cs)
Dieses Journal fügt einem Zeichnungsblatt eine Skizze mit Faltmarkierungen gemäß DIN 824 hinzu.

### Erstellen von assoziativen Texthinweisen auf Zeichnungen
#### (createDrawingNotes/createDrawingNotes.cs)
Ein Journal welches assoziative Texthinweise auf einer Zeichnung hinzufügt. Die Hinweise werden dabei immer im jeweiligen Einzelteil als Teileattribut hinterlegt. Diese Attribute können z.B. über die Stücklistenfunktion oder eine Datenbankverbindung bearbeitet werden.

Beim Verwenden der Funktion muss die Komponente, von welcher die Attribute verwendet werden sollen und die Ansicht, an die die Informationen geheftet werden sollen, ausgewählt werden. 

### Erstellen von assoziativen Attributen und teileübergreifender Ausdrücken
Die nachfolgenden Journale können z.B. dafür genutzt werden um Projektinformationen in Unterbaugruppen oder Einzelteilen verfügbar zu machen. Mit den Ausdrücken lassen sich dann assoziative Formelemente aufbauen (z.B. Teilebeschriftungen/Kennzeichnungen oder ähnliches).

#### (createAssoziativAttributes/linkOwnAttributes.cs)
Dieses Journal erstellt assoziative Ausdrucke von Teileattributen innerhalb eines aktiven Teils. Es eignet sich um z.B. Stücklistenattribute für die Konstruktion verfügbar zu machen. Beispielsweise kann damit das Material oder die Teilenummer für eine Teilebeschriftungen/Kennzeichnungen am Bauteil weiterverwendet werden.

#### (createAssoziativAttributes/linkOtherAttributes.cs)
Dieses Journal durchsucht alle geöffneten Teile nach einem bestimmten Attribut (z.B. nach "UM_TOP", der obersten Baugruppe eines Moldwizard-Projekts).

In diesem Bauteil werden dann assoziative Ausdrucke erstellt, welche auf die anzugebenden Teileattribute verweisen (z.B. "UM_PROJECT", der Projektnummer eines Moldwizard-Projekts). Die so erstellten Ausdrücke werden dann assoziativ in das aktuell aktive Bauteil gelinkt.
