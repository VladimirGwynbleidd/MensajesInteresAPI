Option Explicit

'Objeto para ingresar a la raiz del web server
Dim objRoot, MAIN_WEB_PATH, oWShell, sParam, oParams, sUser, sPwd
Dim result, siteName, paramsCustom,CustomActionDataValues

CONST MSTV_WS = "MIMAPI"
CONST MSTV_APPPOOL = "MIMAPI"
CONST MSTV_APPPOOL_IDENTITY = "IDC\WebUser" 
CONST MSTV_APPPOOL_PASSWORD = ""

ON ERROR RESUME NEXT

'Obtener la ruta de instalación del instalador
CustomActionDataValues = Session.Property("CustomActionData")
paramsCustom = Split(CustomActionDataValues,"||")
MAIN_WEB_PATH = paramsCustom(0)
siteName = paramsCustom(1)

'valido website
result = GetWebSiteID(siteName)

If (result <= 0) Then
    MsgBox "No fue posible hallar el WebSite " & siteName
    wscript.Quit
End If

'Asignacion de valores.
sUser = MSTV_APPPOOL_IDENTITY
sPwd  = MSTV_APPPOOL_PASSWORD

SET objRoot = GetObject("IIS://LocalHost/W3SVC/" + CStr(result) + "/ROOT")

IF (Err.Number <> 0) THEN

      MsgBox "El servidor Web no se encuentra disponible. Err: " & Err.Description
      wscript.Quit
END IF

'****************WEB SERVICE********************
'CREAR EL DIRECTORIO VIRTUAL
CALL CREATE_VIRTUAL_DIRECTORY ( objRoot, MSTV_WS, MAIN_WEB_PATH)
'**************END WEB SERVICE******************
IF (Err.Number <> 0) THEN

      MsgBox "No fue posible crear el directorio virtual " & MSTV_WS & ". Err: " & Err.Description
      wscript.Quit

END IF

Set oWShell = CreateObject ("WScript.Shell")

IF (Err.Number <> 0) THEN

      MsgBox "No fue posible crear el objeto WScript.Shell(). Err: " & Err.Number & " " & Err.Description
      wscript.Quit

END IF

'CREAR EL APP POOL PARA LOS DIRECTORIOS VIRTUALES DE LA APLICACION HTTP DE BIZTALK
CALL CREATE_APP_POOLS( MSTV_APPPOOL , sUser, sPwd )

IF (Err.Number <> 0) THEN

      MsgBox "No fue posible crear el APPLICATION POOL " & MSTV_APPPOOL & ". Err: " & Err.Number & " " & Err.Description
      wscript.Quit

END IF


'ASIGNAR EL POOL CREADO A LOS DIRECTORIOS VIRTUALES
CALL ASSIGN_APP_POOL ( MSTV_WS , MSTV_APPPOOL )
IF (Err.Number <> 0) THEN

      MsgBox "Error al asignar el application pool '" & MSTV_APPPOOL & "' en el virtual directory '" & MSTV_RESET_VD & "'. Err: " & Err.Number & " " & Err.Description
      wscript.Quit

END IF

Call CreaLlave(siteName)

SET objRoot = Nothing
SET oWShell = Nothing


PUBLIC FUNCTION CREATE_VIRTUAL_DIRECTORY ( oRootWS, sVirtualDirName, sAppFolder )

'Objecto que contendra la información de los directorios virutales
DIM objVDir

ON ERROR RESUME NEXT

Set objVDir = oRootWS.Create("IIsWebVirtualDir", sVirtualDirName)

objVDir.Path = sAppFolder

objVDir.AccessRead = True

objVDir.AccessWrite = False

objVDir.AccessScript = True

objVDir.AccessExecute = True

objVDir.EnableDirBrowsing = False

objVDir.EnableDefaultDoc = True

objVDir.DefaultDoc = "Default.aspx"

objVDir.AspEnableParentPaths = true

'Integrated Security
'AuthFlags Settings 
'1 ---  Anonymous access 
'2 --- Basic Authentication (password is sent in clear text) 
'4 --- Integrated Windows Authentication 
'5 --- AuthNTLM + AuthAnonymous
'16 -- Digest authentication for Windows domain servers 
objVDir.AuthFlags = 1

objVDir.SetInfo

'CREAR LA APLICACION
CALL CREATE_APPLICATION ( sVirtualDirName )

if (Err.Number <> 0) then

      MsgBox "El directorio virtual " & sVirtualDirName & " no pudo ser creado. Err: " & Err.Description
      wscript.Quit

end if

SET objVDir = Nothing


END FUNCTION

PUBLIC FUNCTION CREATE_APP_POOLS( sAppPoolName, sUserName, sPassword )

'CREATE THE APP POOL TO USE

'Objectos para la creacion de los app pools
DIM objAppPools, objAppPool

ON ERROR RESUME NEXT

Set objAppPools = GetObject("IIS://localhost/W3SVC/AppPools")

Set objAppPool = objAppPools.Create("IIsApplicationPool", sAppPoolName )

'***********************************IDENTITY******************************
' 0 = Local System
' 1 = Local Service
' 2 = Network Service
' 3 = Custom Identity -> also set WAMUserName and WAMUserPass
objAppPool.AppPoolIdentityType = 3
Dim IIsObject
Set IIsObject = GetObject ("IIS://localhost/w3svc")
objAppPool.WAMUserName = sUserName
objAppPool.WAMUserPass = sPassword
objAppPool.LogonMethod = 1 
Set IIsObject = Nothing

'***********************************PERFORMANCE*****************************
objAppPool.IdleTimeout = 20
objAppPool.AppPoolQueueLength = 4000
objAppPool.MaxProcesses = 1
objAppPool.enable32BitAppOnWin64 = false

'***********************************HEALTH**********************************
objAppPool.PingInterval = 30
objAppPool.RapidFailProtection = true
objAppPool.RapidFailProtectionInterval = 5
objAppPool.RapidFailProtectionMaxCrashes = 5
objAppPool.ShutdownTimeLimit = 3600

'***********************************RECICLAJE**********************************
'objAppPool.PeriodicRestartRequests = 500
'objAppPool.PeriodicRestartTime = 30

objAppPool.SetInfo



IF (Err.Number <> 0) THEN
    MsgBox "El application pool " & sAppPoolName & " no pudo ser creado. Err: " & Err.Description
    wscript.Quit
END IF 

SET objAppPool = Nothing
SET objAppPools = Nothing

END FUNCTION


PUBLIC FUNCTION CREATE_APPLICATION( sApplicationName )

'Objecto que contrandrá la aplicacion
DIM objIIS

ON ERROR RESUME NEXT

SET objIIS = GetObject("IIS://localhost/W3SVC/" + CStr(result) + "/ROOT/" & sApplicationName)

'Create a process pooled process
objIIS.AppCreate2 (2)

objIIS.Put "AppFriendlyName", sApplicationName

objIIS.SetInfo

IF (Err.Number <> 0) THEN

	MsgBox "Error al crear la aplicacion en 'IIS://LocalHost/W3SVC/" + CStr(result) + "/ROOT/" & sApplicationName & "'. Err: " & Err.Description
	WScript.Quit

END IF

SET objIIS = Nothing

END FUNCTION


PUBLIC FUNCTION ASSIGN_APP_POOL ( sVDirName , sAppPoolName )
	DIM objVDir

	ON ERROR RESUME NEXT

	SET objVDir = GetObject("IIS://localhost/W3SVC/" + CStr(result) + "/ROOT/" & sVDirName )

	objVDir.AppPoolId = sAppPoolName

	objVDir.SetInfo

	IF (Err.Number <> 0) THEN

		MsgBox "Error al asignar el application pool '" & sAppPoolName & "' en el virtual directory '" & sVDirName & "'. Err: " & Err.Number & " " & Err.Description
		WScript.Quit
	END IF

	SET objVDir = Nothing
END FUNCTION

Function GetWebSiteID(siteName)
    On Error Resume Next

    Dim result2
    result2 = -1

    Dim SiteNumber, siteFound, noMoreWebSites
    siteFound      = False
    noMoreWebSites = False
    SiteNumber     = 1

    Do While (Not siteFound And Not noMoreWebSites)
        Dim site
        Set site = GetObject("IIS://LocalHost/W3SVC/" & CStr(SiteNumber))

        If (Err.Number = 0) Then
            If (siteName = site.ServerComment) Then
                result2    = SiteNumber
                siteFound = True
            End If
        Else
            ' Call WScript.Echo("Error " & Err.Number & "(0x" & Hex(Err.Number) & ") retrieving site properties: " & Err.Description & vbCrLf)
            Call Err.Clear()
            noMoreWebSites = True
        End If

        Set site   = Nothing
        SiteNumber = SiteNumber + 1
    Loop

    GetWebSiteID = result2
End Function

Function CreaLlave(Sitio)
    dim strComputer,strKeyPath,KeyPath,strValueName,strValue,objReg,result
    const HKEY_LOCAL_MACHINE = &H80000002

    strComputer = "."
    Set objReg=GetObject("winmgmts:{impersonationLevel=impersonate}!\\"&_ 
        strComputer & "\root\default:StdRegProv")

    strKeyPath = "SOFTWARE\SAT\"
    KeyPath = "Software\SAT"
    strValueName = MSTV_WS '"SITIOBUZON"
    strValue = Sitio '"SitioBuzon"

    ' Create key to use
    result = objReg.CreateKey(HKEY_LOCAL_MACHINE, KeyPath)
        ' write string value to key    
        result = objReg.SetStringValue( _
            HKEY_LOCAL_MACHINE,strKeyPath,strValueName,strValue)
        If (result <> 0) Or (Err.Number <> 0) Then 
            MsgBox "SetStringValue failed. Error = " & Err.Number
        End If
End Function
