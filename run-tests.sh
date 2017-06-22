#!/bin/sh
cd /src/test/Automaty.Samples.Test/
dotnet restore
dotnet test --logger "trx;LogFileName=testresult.xml"