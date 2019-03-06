# JasperSite
JasperSite is a web content management system built on top of ASP.NET Core MVC. The founder of this project is Bc. Jaromír Kejda. This project is a part of his master's thesis: _Design and Implementation of Content Management System in ASP.NET Core_ - for more information refer to the __CZU__ branch of this repository or visit [jaromirkejda.net](https://www.jaromirkejda.net) (Czech only). 



<p float="left">
  <img  src="https://github.com/kejdajar/JasperSite/blob/master/resources/readme_resources/dashboard.jpg" width="32%" height="200px" />
  <img  src="https://github.com/kejdajar/JasperSite/blob/master/resources/readme_resources/articles_overview.jpg" width="32%" height="200px" /> 
  <img  src="https://github.com/kejdajar/JasperSite/blob/master/resources/readme_resources/edit_article.jpg" width="32%" height="200px" />
</p>

## Set-up
__For a detailed installation guide visit [Production installation](https://github.com/kejdajar/JasperSite/wiki/Production-installation) page.__
 
In order to deploy this CMS to your webhosting service, you need to make sure that your provider supports the .NET Core 2.1 Framework. Firstly, download the ready-to-be-deployed version of this CMS from the Releases section and upload all these un-zipped files directly to the root folder of your webhosting. You will also need a connection string to your MSSQL or a MySQL database. The last step is to visit your page, eg. http://mydomain.com/ in order to complete the installation. If done correctly, you should be prompted to insert a connection string to your database. A login screen should appear automatically, once the installation has been completed. Use your credentials to log into the system.

<p float="left">
<img  src="https://github.com/kejdajar/JasperSite/blob/master/resources/readme_resources/installation_page.jpg" width="40%" height="200px" />
  <img  src="https://github.com/kejdajar/JasperSite/blob/master/resources/readme_resources/login_page.jpg" width="35%" height="200px" />
</p>

Now if you visit your webpage, eg. http://mydomain.com/ you will be greeted with a welcome page with further instructions. A great way how to start with JasperSite is to pick a different theme from the *Themes* section in your administration page.

## Supported languages
JasperSite supports _English and Czech Language_. The language will be automatically selected based on your environment and can be changed later on.

## Branches
This repository consists of three main branches:

1. __master__ - the most stable releases of JasperSite
2. __develop__ - experimental features and unstable code
3. __czu__ - legacy version of JasperSite, published as a part of master's thesis

## Installation for development purposes
Follow the next steps according to your needs:

### Development Installation
__For a detailed development installation visit [Development installation](https://github.com/kejdajar/JasperSite/wiki/Development-installation) page.__
 
1. Clone this repository using GIT or download directly the source files.
2. Use Visual Studio, Visual Studio Code, or any other IDE to open the JasperSite.sln solution file or solution folder.
3. Make sure that you have .NET Core 2.1 SDK present on your machine.
4. Make sure that _jasper.json_ configuration file in the root directory has property _installationCompleted_ set to False (or anything other than True): `"installationCompleted": "False"`
5. Run the application using Visual Studio, or navigate in CMD to the _JasperSite_ subfolder and run `dotnet run` or on a Windows machine run the `run.bat` file.
6. If you didn't use Visual Studio in the previous step, navigate to the `http://localhost:5000` in your browser.
7. Follow the instructions displayed onscreen.


### Running compiled version locally
If you want to run already compilled ready-to-deploy JasperSite (downloaded from the Releases section), you can navigate to the root folder and run `dotnet JasperSite.dll` via CMD (make sure you have .NET Core 2.1 SDK installed). 


## History
Although the beginning of this project is dated 8/8/2017, an older preparatory version of this CMS was used as its foundation.
The major part of this old version built on ASP.NET MVC was rewritten and the whole project was ported to ASP.NET Core MVC. On 5/9/2018 the CMS (formerly known as __JasperSiteCore__) was renamed to shorter __JasperSite__. Later on, the whole project was upgraded to .NET Core 2.1.
