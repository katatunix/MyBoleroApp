#!/bin/sh -x

dotnet serve -d dist/wwwroot -p 80 -a any -o
