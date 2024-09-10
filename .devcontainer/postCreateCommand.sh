#!/bin/bash
dotnet dev-certs https
dotnet dev-certs https --trust
dotnet new tool-manifest
dotnet tool install csharpier
