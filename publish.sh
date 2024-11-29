#!/bin/sh -x
dotnet clean
rm -dfr bin
rm -dfr obj
rm -dfr dist
dotnet publish -o dist # -p:GHPages=true
