; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "CustomCalculator"
#define MyAppVersion GetVersionNumbersString('..\bin\Debug\Calculator.exe')
#define MyAppPublisher "zfxx"
#define MyAppURL "https://github.com/"
#define MyAppExeName "Calculator.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".myp"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt
#define DevStage "RC"
#define MyAppInstallPath "C:\CustomCalculator"
#define MyUid "7C85666B-EB18-42DC-8EAD-698164333F5B"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{{#MyUid}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={#MyAppInstallPath}
ChangesAssociations=yes
DisableProgramGroupPage=yes
SetupMutex=SetupMutex{#SetupSetting("AppId")}
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputBaseFilename={#MyAppName}.{#DevStage}.{#MyAppVersion}.Installer
SetupIconFile=..\bin\Debug\logo.ico
UninstallDisplayIcon={app}\logo.ico
UninstallDisplayName={#MyAppName}.{#DevStage}.{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
OutPutdir=.\Output
LicenseFile=..\bin\Debug\LicenseAgreement.rtf

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone

[Dirs]
Name: "{userappdata}\CustomCalculator";
Name: "{userappdata}\CustomCalculator\Data";

[Files]
Source: "..\bin\Debug\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Data\Data.db"; DestDir: "{userappdata}\CustomCalculator\Data"; Flags: ignoreversion onlyifdoesntexist uninsneveruninstall

[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\SupportedTypes"; ValueType: string; ValueName: ".myp"; ValueData: ""

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent unchecked

[code]
var 
  globalInstallPath: String;
 
//初始化时把路径设置到编辑框
procedure InitializeWizard;
begin
  WizardForm.DirEdit.Text := globalInstallPath;
end;

function IsAppRunning(const FileName: string): Boolean;
var
  FWMIService: Variant;
  FSWbemLocator: Variant;
  FWbemObjectSet: Variant;
begin
  Result := false;
  FSWbemLocator := CreateOleObject('WBEMScripting.SWBEMLocator');
  FWMIService := FSWbemLocator.ConnectServer('', 'root\CIMV2', '', '');
  FWbemObjectSet := FWMIService.ExecQuery(Format('SELECT Name FROM Win32_Process Where Name="%s"',[FileName]));
  Result := (FWbemObjectSet.Count > 0);
  FWbemObjectSet := Unassigned;
  FWMIService := Unassigned;
  FSWbemLocator := Unassigned;
end;
 
//获取历史安装路径，Inno Setup保存的一些信息可自己在注册表中查看
//64位会映射到：计算机\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall
function GetInstallString(): String;
var
  InstallPath: String;
begin
  InstallPath := '{#MyAppInstallPath}';
  if RegValueExists(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{{#MyUid}}_is1', 'Inno Setup: App Path') then
    begin
      RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{{#MyUid}}_is1', 'Inno Setup: App Path', InstallPath)
    end;
  result := InstallPath;
end;
 
//准备安装
function InitializeSetup(): Boolean;  
var  
  ResultStr: String;  
  ResultCode: Integer;  
begin     
  globalInstallPath := GetInstallString();
  result := IsAppRunning('{#MyAppExeName}');
  if result then
    begin
      MsgBox('检测到{#MyAppName}正在运行，请先关闭程序后重试! ', mbError, MB_OK); 
      result:=false;
    end
  else if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{{#MyUid}}_is1', 'UninstallString', ResultStr) then
    begin  
      MsgBox('本机已安装{#MyAppName}，请先卸载再安装。', mbError, MB_OK)
      result:=false;
      //if  MsgBox('是否卸载已安装的{#MyAppName}，并保留历史数据？', mbConfirmation, MB_YESNO) = IDYES then
      //  begin  
      //    ResultStr := RemoveQuotes(ResultStr);  
      //   Exec(ResultStr, '/silent', '', SW_HIDE, ewWaitUntilTerminated, ResultCode); 
      //  end;
      //result:=true;      
    end
  else
    begin
        result:=true; 
    end;
end;
 
//准备卸载
function InitializeUninstall(): Boolean;
begin
  result := IsAppRunning('{#MyAppExeName}');
  if result then
    begin
      MsgBox('检测到{#MyAppName}正在运行，请先关闭程序后重试! ', mbError, MB_OK); 
      result:=false;
    end
  else
    begin
      result:=true;
    end
end;
