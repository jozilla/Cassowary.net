# Makefile for Unix systems.
# Requires mcs, the Mono C# compiler.
# Author: Jo Vermeulen <jo@lumumba.uhasselt.be>

SRC_DIR = Cassowary
WARN_LEVEL = 1

all: lib test

lib:
	mcs -warn:${WARN_LEVEL} -target:library -out:Cassowary.dll ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs
test:
	mcs -warn:${WARN_LEVEL} -target:exe -main:ClTests -out:ClTests.exe ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs ${SRC_DIR}/Tests/*.cs

clean:
	rm *.exe 
