@echo off
rem
rem	4Sep20 JAF
rem	this script executes the xUnit test suite and collects code coverage metrics

rem
rem	execute the test suite and collect coverage data into a cobertura xml file
rem	Arguments:
rem		-r <dir> : specify results output directory
rem		-l html;LogFileName=<file_name> : log test results to HTML file to specified <file_name>
rem		/p:CollectCoverage : turn on/off collect coverage data
rem		/p:CoverletOutputFormat : specify coverage output file format
rem		/p:Exclude : specify assemblies and namespaces to exclude from code coverage analysis
dotnet test -r TestResults -l html;LogFileName=SWENG894.TestResults.html --collect:"XPlat Code Coverage" --settings run_settings.xml

rem
rem	generate an html report from the cobertura xml
rem	-- requires ReportGenerator installed as a .NET global tool
rem	-- dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:CoverageReport" -reporttypes:Html

rem
rem	clean up intermediate files
del coverage.cobertura.xml

rem
rem	end of file