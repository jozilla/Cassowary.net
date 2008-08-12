# Makefile for Unix systems.
# Requires gmcs, the Mono C# compiler.
# For constraint parsing support, the Coco/R 
# parser generator is needed.
# 
# Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>

SRC_DIR = Cassowary
PROPS = Properties
WARN_LEVEL = 4
COCO_CS = cococs
COCO_CS_FRAMEDIR = ${SRC_DIR}/Parsing/

INSTALL=install
INSTALL_PROGRAM=$(INSTALL)
INSTALL_DATA=$(INSTALL) -m 644

prefix=/usr
exec_prefix=${prefix}
bindir=${exec_prefix}/bin
libdir=$(exec_prefix)/lib
monolibdir=${libdir}/mono/2.0/
#/usr/lib/mono/1.0/
PACKAGE=cassowarynet

GACUTIL=gacutil
GACUTIL_FLAGS=/package $(PACKAGE) /gacdir $(DESTDIR)$(prefix)/lib

all: lib tests parselib 

lib:
	@echo "building cassowary library"
	gmcs -warn:${WARN_LEVEL} -target:library -out:Cassowary.dll -keyfile:Cassowary.snk ${PROPS}/AssemblyInfo-Cassowary.cs ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs
	@echo "done"

tests: cltests layout_test

cltests: lib
	@echo "building cltests"
	gmcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.ClTests -out:ClTests.exe -r:Cassowary.dll ${SRC_DIR}/Tests/ClTests.cs ${SRC_DIR}/Tests/Timer.cs
	@echo "done"

layout_test: lib
	@echo "building layout test"
	gmcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.LayoutTest -out:LayoutTest.exe -r:Cassowary.dll ${SRC_DIR}/Tests/LayoutTest.cs
	@echo "done"

parselib: lib
	@echo "building constraint parsing library"
	${COCO_CS} -frames ${COCO_CS_FRAMEDIR} ${SRC_DIR}/Parsing/constraint_grammar.atg
	gmcs -warn:${WARN_LEVEL} -target:library -out:Cassowary.Parsing.dll -keyfile:Cassowary.Parsing.snk -r:Cassowary.dll ${PROPS}/AssemblyInfo-Cassowary.Parsing.cs ${SRC_DIR}/Parsing/*.cs
	@echo "done"

clean:
	rm -f *.dll *.exe 
	rm -f ${SRC_DIR}/Parsing/Scanner.cs.old ${SRC_DIR}/Parsing/Parser.cs.old


install:
	$(INSTALL) -d ${DESTDIR}${bindir}
	$(INSTALL) -d ${DESTDIR}${monolibdir}
	$(INSTALL_PROGRAM) ClTests.exe $(DESTDIR)$(bindir)/ClTests.exe
	$(INSTALL_PROGRAM) LayoutTest.exe $(DESTDIR)$(bindir)/LayoutTest.exe
	$(INSTALL_PROGRAM) Cassowary.dll $(DESTDIR)$(monolibdir)/Cassowary.dll
	$(INSTALL_PROGRAM) Cassowary.Parsing.dll $(DESTDIR)$(monolibdir)/Cassowary.Parsing.dll

register:
	$(GACUTIL) /i Cassowary.dll /f $(GACUTIL_FLAGS) || exit 1;
	$(GACUTIL) /i Cassowary.Parsing.dll /f $(GACUTIL_FLAGS) || exit 1;

