# JasperSite
JasperSite is a web content management system built on top of ASP.NET Core MVC, designed and implemented by Ing. Jaromír Kejda. 

This project is a part of his master's thesis: _Design and Implementation of Content Management System in ASP.NET Core_.
This master's thesis was successfully defended in 2018, Czech University of Life Sciences and supervised by Ing. Jiří Brožek, Ph.D., Department of Information Engineering.

## Legacy branch
__This branch contains the exact version of JasperSite that was published together with the master's thesis. This branch is, therefore, no longer subject of further updates.__ 

## Supported languages
Understandably, the source code of this program is written and commented in English. Unfortunately, the ADMIN section of the CMS supports only _Czech Language_. For additional languages and further info, please refer to the main development branch. 

## Installation
Follow the next steps according to your needs:

### Development Installation
1. Clone this repository using GIT or download directly the source files.
2. Use Visual Studio, Visual Studio Code, or any other IDE to open the JasperSite.sln solution file.
3. Make sure that you have .NET Core 2.1 SDK present on your machine.
4. Make sure that _jasper.json_ configuration file in the root directory has property _installationCompleted_ set to False (or anything other than True): `"installationCompleted": "False"`
5. Run the application using Visual Studio, or navigate in CMD to the _JasperSite_ subfolder and run `dotnet run` or on Windows machine run the `run.bat` file.
6. If you didn't use Visual Studio in the previous step, navigate to the http://localhost:5000 in your browser.
7. Follow the instructions displayed onscreen.

### Production Installation
For the installation on a production server follow either the same steps as in development installation and use built-in publish feature of Visual Studio, or download the already compiled project from the releases section, which is ready to be uploaded to the production server, for instance through FTP.
If you want to run this compiled project locally, you can navigate to the root folder and run `dotnet JasperSite.dll` via CMD. 


## History
Although the beginning of this project is dated 8/8/2017, an older preparatory version of this CMS was used as its foundation.
The major part of this old version built on ASP.NET MVC was rewritten and the whole project was ported to ASP.NET Core MVC. On 5/9/2018 the CMS (formerly known as __JasperSiteCore__) was renamed to shorter __JasperSite__. Later on, the whole project was upgraded to .NET Core 2.1.