Option Explicit

'Objeto para ingresar a la raiz del web server
Dim objRoot, MAIN_WEB_PATH, oWShell, sParam

CONST MSTV_PORTAL_VD = "MIMAPI"
CONST MSTV_APPPOOL_PORTAL = "MIMAPI"	


'leo el nombre del sitio
Dim siteName,result
siteName = LeeLlave()
result   = GetWebSiteID(siteName)
If (result <= 0) Then
    MsgBox "No fue posible hallar el WebSite " & siteName
    wscript.Quit
End If

dim IIS_ROOT
IIS_ROOT = "IIS://LocalHost/W3SVC/" + CStr(result) + "/ROOT"

ON ERROR RESUME NEXT

'Obtener la ruta de instalación del instalador
MAIN_WEB_PATH = Session.Property("CustomActionData")

CALL DELETE_VIRTUAL_DIRECTORY ( MSTV_PORTAL_VD )

IF (Err.Number <> 0) THEN

      MsgBox "No fue posible ELIMINAR el directorio virtual " & MSTV_PORTAL_VD & ". Err: " & Err.Description
      wscript.Quit

END IF

CALL DELETE_APP_POOLS( MSTV_APPPOOL_PORTAL )

IF (Err.Number <> 0) THEN

      MsgBox "No fue posible ELIMINAR el APPLICATION POOL " & MSTV_APPPOOL_PORTAL & ". Err: " & Err.Number & " " & Err.Description
      wscript.Quit

END IF

call BorraLlave()

SET objRoot = Nothing
SET oWShell = Nothing


PUBLIC FUNCTION DELETE_VIRTUAL_DIRECTORY ( sVirtualDirName )

'Objeto que contendra la información de los directorios virutales
DIM objVDir, objRoot

ON ERROR RESUME NEXT

SET objVDir = GetObject(IIS_ROOT & "/" & sVirtualDirName )

if ( IsObject(objVDir) ) then

    SET objRoot = GetObject(IIS_ROOT)

    objRoot.Delete "IIsWebVirtualDir", sVirtualDirName

end if 


if (Err.Number <> -2147024893 and Err.Number <> 0 ) then

      MsgBox "El directorio virtual " & sVirtualDirName & " no pudo ser eliminado. Err: " & Err.number & " " & Err.Description
      wscript.Quit
else
	Err.Clear
end if

END FUNCTION


PUBLIC FUNCTION DELETE_APP_POOLS( sAppPoolName )

'Objectos para la creacion de los app pools
DIM objAppPools, objAppPool

ON ERROR RESUME NEXT

Set objAppPools = GetObject("IIS://localhost/W3SVC/AppPools")

objAppPools.Delete "IIsApplicationPool", sAppPoolName

IF (Err.Number <> -2147024893 and Err.Number <> 0) THEN
    MsgBox "El application pool " & sAppPoolName & " no pudo ser eliminado. Err: " & Err.Description
    wscript.Quit
ELSE
    Err.Clear
END IF 


SET objAppPool = Nothing
SET objAppPools = Nothing

END FUNCTION

Function GetWebSiteID(siteName)
    On Error Resume Next

    Dim result
    result = -1

    Dim SiteNumber, siteFound, noMoreWebSites
    siteFound      = False
    noMoreWebSites = False
    SiteNumber     = 1

    Do While (Not siteFound And Not noMoreWebSites)
        Dim site
        Set site = GetObject("IIS://LocalHost/W3SVC/" & CStr(SiteNumber))

        If (Err.Number = 0) Then
            If (siteName = site.ServerComment) Then
                result    = SiteNumber
                siteFound = True
            End If
        Else
            Call Err.Clear()
            noMoreWebSites = True
        End If

        Set site   = Nothing
        SiteNumber = SiteNumber + 1
    Loop

    GetWebSiteID = result
End Function

Function LeeLlave()
    dim strComputer,strKeyPath,KeyPath,strValueName,strValue,objReg
    const HKEY_LOCAL_MACHINE = &H80000002

    strComputer = "."
    Set objReg=GetObject("winmgmts:{impersonationLevel=impersonate}!\\"&_ 
        strComputer & "\root\default:StdRegProv")

    strKeyPath = "SOFTWARE\SAT\"
    KeyPath = "Software\SAT"
    strValueName = MSTV_PORTAL_VD '"SITIOBUZON"

    objReg.GetStringValue HKEY_LOCAL_MACHINE, strKeyPath, strValueName, strValue 

    LeeLlave = strValue

    If LeeLlave="" then
    MsgBox "No se encuentra la llave " & strValueName & Err.number
    End if
End Function

Function BorraLlave()
    const HKEY_LOCAL_MACHINE = &H80000002
    dim strComputer,strKeyPath,KeyPath,strValueName,strValue,objReg,result

    strComputer = "."
    Set objReg=GetObject("winmgmts:{impersonationLevel=impersonate}!\\"&_ 
        strComputer & "\root\default:StdRegProv")

    strKeyPath = "SOFTWARE\SAT\"
    KeyPath = "Software\SAT"
    strValueName = MSTV_PORTAL_VD '"SITIOBUZON"
    strValue = "SitioBuzon"

    ' Create key to use
    result = objReg.DeleteValue(HKEY_LOCAL_MACHINE,strKeyPath,strValueName)

    If (result <> 0) Or (Err.Number <> 0) Then
         MsgBox "Registry value not deleted" & VBNewLine _
                      & "Error = " & Err.Number
    End If
End Function