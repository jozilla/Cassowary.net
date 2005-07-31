# Makefile for Unix systems.
# Requires mcs, the Mono C# compiler.
# Author: Jo Vermeulen <jo@lumumba.uhasselt.be>

SRC_DIR = Cassowary
WARN_LEVEL = 1

all: lib tests 

lib:
	mcs -warn:${WARN_LEVEL} -target:library -out:Cassowary.dll ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs

tests: cltests layout_test

cltests: lib
	mcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.ClTests -out:ClTests.exe -r:Cassowary.dll ${SRC_DIR}/Tests/ClTests.cs ${SRC_DIR}/Tests/Timer.cs

layout_test: lib
	mcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.LayoutTest -out:LayoutTest.exe -r:Cassowary.dll ${SRC_DIR}/Tests/LayoutTest.cs

clean:
	rm -f *.dll *.exe 
