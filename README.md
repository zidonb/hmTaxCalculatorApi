# Introduction 
tax calculator using microsoft rule engine - the current calculator is developed with cobol and using mainframe

# Getting Started
1. install Visual Studio 2022 - file://sh/root/setup/Visual%20Studio%20core%208

install, choose individual component -> .net8 and restart

	pull code from http://shtfsshaam:8080/tfs/DefaultCollection/Hamama/_git/taxCalculation 
2. dependencies: 
	go to tools-> nuGet package manager -> package manager settings:
	go to source package and add:
	Name: Nexus
	Source: http://hamama-nexus.sh.shaam.gov.il/repository/nuget-hosted/
	Name: ArtifactoryJfrog
	Source : http://sh-artifactory.sh.shaam.gov.il:8082/artifactory/api/nuget/v3/nuget
	make sure to check the checkbox
3. APIs are:
	http://localhost:7166/swagger/index.html is the default url for all APIs
	/api/calcTax is the main api the gets the main object and runs all workflows
	

# Build and Test
run build solution and after completed succesfully, run http, it opens a swagger page with all APIs

# Contribute
users can run different tests by putting different values in the object for calcTax API
developers can add unit tests in the code for each workflow
#   h m T a x C a l c u l a t o r A p i  
 