# Makefile for Unix systems.
# Requires mcs, the Mono C# compiler.
# Author: Jo Vermeulen <jo.vermeulen@uhasselt.be>

SRC_DIR = Cassowary
WARN_LEVEL = 1

all: lib tests parser 

lib:
	@echo "building cassowary library"
	mcs -warn:${WARN_LEVEL} -target:library -out:Cassowary.dll ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs
	@echo "done"

tests: cltests layout_test

cltests: lib
	@echo "building cltests"
	mcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.ClTests -out:ClTests.exe -r:Cassowary.dll ${SRC_DIR}/Tests/ClTests.cs ${SRC_DIR}/Tests/Timer.cs
	@echo "done"

layout_test: lib
	@echo "building layout test"
	mcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.LayoutTest -out:LayoutTest.exe -r:Cassowary.dll ${SRC_DIR}/Tests/LayoutTest.cs
	@echo "done"

parser:
	@echo "building constraint parser"
	cococs -frames /usr/lib/coco-cs/ ${SRC_DIR}/Parsing/constraint_grammar.atg
	@echo "done"

clean:
	rm -f *.dll *.exe 
	rm -f ${SRC_DIR}/Parsing/Scanner.cs ${SRC_DIR}/Parsing/Parser.cs
