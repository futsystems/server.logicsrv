PREFIX = /opt
PROJ = build.proj
FLAGS = /property:OperatingPlatform=Unix /property:NetFramework=Mono
XBUILD = /opt/mono/bin/xbuild /tv:4.0

VERSION =
BUILD =
REVISION =
MATURITY =
VERSTR = $(VERSION).$(BUILD).$(REVISION)

VERSIONINFO = src/Shared/VersionInfo.cs

PACK = tar -czf clrzmq-mono-$(VERSTR).tar.gz
PACKFILES = build/clrzmq.* README.md AUTHORS LICENSE

.PHONY=all release package clean

#build base server and so on ...
all:clean base server contrib connector account rule exsrv


release:
	ifdef VERSION
		mv $(VERSIONINFO) $(VERSIONINFO).bak
		echo using System.Reflection; > $(VERSIONINFO)
		echo. >> $(VERSIONINFO)
		echo [assembly: AssemblyVersion("$(VERSION).0.0")] >> $(VERSIONINFO)
		echo [assembly: AssemblyFileVersion("$(VERSTR)")] >> $(VERSIONINFO)
		echo [assembly: AssemblyInformationalVersion("$(VERSTR) $(MATURITY)")] >> $(VERSIONINFO)
		echo [assembly: AssemblyConfiguration("$(MATURITY)")] >> $(VERSIONINFO)

		$(XBUILD) /target:Package $(FLAGS) /Property:Configuration=Release /Property:SignAssembly=true $(PROJ)

		mv $(VERSIONINFO).bak $(VERSIONINFO)
	else
		$(error Invalid VERSION==$(VERSION) - specify package version. E.g., `make VERSION=3.0 BUILD=12345 REVISION=1 MATURITY=Beta')
	endif

package1: release
	$(PACK) $(PACKFILES)

clean:
	$(XBUILD) /target:Clean $(FLAGS) $(PROJ)

base:
	$(XBUILD) /target:Base $(FLAGS) $(PROJ)

server:
	$(XBUILD) /target:Server $(FLAGS) $(PROJ)

contrib:
	$(XBUILD) /target:Contrib $(FLAGS) $(PROJ)

connector:
	$(XBUILD) /target:Connector $(FLAGS) $(PROJ)

account:
	$(XBUILD) /target:Account $(FLAGS) $(PROJ)

rule:
	$(XBUILD) /target:RuleSet $(FLAGS) $(PROJ)

exsrv:
	$(XBUILD) /target:ExServer $(FLAGS) $(PROJ)

publish:
	$(XBUILD) /target:Publish $(FLAGS) $(PROJ)

publishclean:
	$(XBUILD) /target:PublishClean $(FLAGS) $(PROJ)

install:
	
	rm -rf /home/qianbo/opt/logicsrv1
	mkdir  /home/qianbo/opt/logicsrv1
	cp -rf Platform/TradingSrv/config /home/qianbo/opt/logicsrv1/
	cp -rf Platform/TradingSrv/TraddingSrvCLI.exe /home/qianbo/opt/logicsrv1/
	cp -rf lib/TradingLib/* /home/qianbo/opt/logicsrv1/
	cp -rf lib/libzmq/clrzmq-nix/* /home/qianbo/opt/logicsrv1/
	cp -rf lib/MySql.Data.dll /home/qianbo/opt/logicsrv1/




