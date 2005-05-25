# Makefile for Unix systems.
# Requires mcs, the Mono C# compiler.
# Author: Jo Vermeulen <jo@lumumba.uhasselt.be>

SRC_DIR = Cassowary
WARN_LEVEL = 1

all: lib cltests layout_test

lib:
	mcs -warn:${WARN_LEVEL} -target:library -out:Cassowary.dll ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs

cltests:
	mcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.ClTests -out:ClTests.exe ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs ${SRC_DIR}/Tests/ClTests.cs

layout_test:
	mcs -warn:${WARN_LEVEL} -target:exe -main:Cassowary.Tests.LayoutTest -out:LayoutTest.exe ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs ${SRC_DIR}/Tests/LayoutTest.cs

clean:
	rm *.exe 
