# Makefile for Unix systems.
# Requires mcs, the Mono C# compiler.
# Author: Jo Vermeulen <jo@lumumba.uhasselt.be>

SRC_DIR = Cassowary

all: test

test:
	mcs -warn:4 -target:exe -out:ClTests.exe ${SRC_DIR}/*.cs ${SRC_DIR}/Utils/*.cs

clean:
	rm *.exe 
