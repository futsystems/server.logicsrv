PREFIX = /opt
PROJ = build.proj
FLAGS = /property:OperatingPlatform=Unix /property:NetFramework=Mono
XBUILD = /opt/mono/bin/xbuild /tv:4.0

VERSION =
BUILD =
REVISION =
MATURITY =
VERSTR = $(VERSION).$(BUILD).$(REVISION)

VERSIONINFO = Shared/VersionInfo.cs

PACK = tar -czf clrzmq-mono-$(VERSTR).tar.gz
PACKFILES = build/clrzmq.* README.md AUTHORS LICENSE

.PHONY=all release package clean

#build base server and so on ...
all:clean release base server contrib connector account rule exsrv


release:
	$(shell ./version.sh)

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

exsrv:
	$(XBUILD) /target:DataServer $(FLAGS) $(PROJ)
	
publish:
	$(XBUILD) /target:Publish $(FLAGS) $(PROJ)
	$(shell ./build.sh)
	@echo "\033[32;49;1mLogicSrv Package Build Success Version:$(shell git describe) \033[39;49;0m"


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




