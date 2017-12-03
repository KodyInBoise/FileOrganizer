; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "File Organizer"
#define MyAppVersion "0.2"
#define MyAppPublisher "Kody Kriner"
#define MyAppURL "http://www.kodykriner.com"
#define MyAppExeName "FileOrganizer.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{01FE9F95-8C57-4775-B751-D06F01B17632}
AppName={#MyAppName}
AppVersion=
AppVerName={#MyAppName} - v{#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=\\Mac\Home\Documents\Visual Studio 2017\Projects\FileOrganizer
OutputBaseFilename=FileOrganizer_Installer
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon=\\Mac\Home\Documents\Visual Studio 2017\Projects\FileOrganizer\FileOrganizer\bin\Debug\main.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "\\Mac\Home\Documents\Visual Studio 2017\Projects\FileOrganizer\FileOrganizer\bin\Debug\FileOrganizer.exe"; DestDir: "{app}"; Flags: ignoreversion;
Source: "\\Mac\Home\Documents\Visual Studio 2017\Projects\FileOrganizer\FileOrganizer\bin\Debug\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

