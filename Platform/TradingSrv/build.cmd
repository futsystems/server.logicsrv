"dotNET_Reactor.exe" -project "logicsrv-dll.nrproj"
"dotNET_Reactor.exe" -project "logicsrv-exe.nrproj"
"dotNET_Reactor.exe" -project "front-exe.nrproj"
copy FrontSrv.exe.config Secure
copy LogicSrv.exe.config Secure
copy ZeroMQ.dll Secure
copy TLBrokerXAPI.dll Secure
xcopy Connector\* Secure\Connector /Y
xcopy Contrib\* Secure\Contrib /Y
xcopy RuleSet\* Secure\RuleSet /Y