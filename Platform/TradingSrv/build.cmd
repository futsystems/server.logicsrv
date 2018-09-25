"dotNET_Reactor.exe" -project "logicsrv-dll.nrproj"
"dotNET_Reactor.exe" -project "logicsrv-exe.nrproj"

copy TLBrokerXAPI.dll Secure
xcopy Connector\* Secure\Connector /Y
xcopy RuleSet\* Secure\RuleSet /Y