# .NET 5 User-Owns-Data Embedding Tutorial
This tutorial teaches developers how to create a .NET 5 MVC web application that implements Power BI Embedding.

## Developing with Power BI Embedding using .NET 5

In this lab, you will create a new .NET 5 project for a custom web application and then you will go through the steps required to implement Power BI embedding. You will use the new Microsoft authentication library named _Microsoft.Identity.Web_ to provide an interactive login experience and to acquire access tokens which you will need to call the Power BI Service API. After that, you will write the server-side C# code and the client-side JavaScript code to embed a simple Power BI report on a custom Web page. In the later exercises of this lab, you will add project-level support for Node.js, TypeScript and webpack so that you can migrate the client-side code from JavaScript to TypeScript so that your code receives the benefits of strong typing, IntelliSense and compile-time type checks.

To complete this lab, your developer workstation must configure to allow the execution of PowerShell scripts. Your developer workstation must also have the following software and developer tools installed.

1) **PowerShell cmdlet library for AzureAD** – [[download](https://docs.microsoft.com/en-us/powershell/azure/active-directory/install-adv2?view=azureadps-2.0)]

2) **.NET 5 SDK** – [[download](https://dotnet.microsoft.com/download/dotnet/5.0)]

3) **Node.js** – [[download](https://nodejs.org/en/download/)]

4) **Visual Studio Code** – [[download](https://code.visualstudio.com/Download)]

5) **Visual Studio 2019** (optional) – [[download](https://visualstudio.microsoft.com/downloads/)]

Please refer to this [setup document](https://github.com/PowerBiDevCamp/Camp-Sessions/raw/master/Create%20Power%20BI%20Development%20Environment.pdf) if you need more detail on how to configure your developer workstation to work on this tutorial.

### Exercise 1: Create a New Azure AD Application

In this exercise, you will begin by copying the student files into a local folder on your student workstation. After that, you will use the .NET 5 CLI to create a new .NET 5 project for an MVC web application with support for authentication.

1. Download the student lab files to a local folder on your developer workstation.
  1. Create a new top-level folder on your workstation named **DevCamp** at a location such as **c:\DevCamp**.
  2. Download the ZIP archive with the student lab files from GitHub by clicking the following link.

**[https://github.com/PowerBiDevCamp/DOTNET5-UserOwnsData-Tutorial/raw/main/StudentLabFiles.zip](https://github.com/PowerBiDevCamp/DOTNET5-UserOwnsData-Tutorial/raw/main/StudentLabFiles.zip)**

  1. Extract the **StudentLabFiles** folder from **StudentLabFiles.zip** into a to a local folder such as **c:\DevCamp\StudentLabFiles**.
  2. The **StudentLabFiles** folder should contain the set of files shown in the following screenshot.

![](RackMultipart20201204-4-1di9arb_html_7b61e9c0f98938d.png)

The files in the **StudentLabFiles** folder contain code that you will be required to write during the exercises of this lab. These files are provides to make it easier to copy and paste.

1. Walk through the PowerShell code in **Create-AzureAD-Application.ps1** tounderstand what it does.
  1. Using Windows Explorer, look in the **StudentLabFiles** folder and locate the script named **Create-AzureAD-Application.ps1.**
  2. Open **Create-AzureAD-Application.ps1** in a text editor such asNotepad or the PowerShell ISE.
  3. The script begins by calling **Connect-AzureAD** to establish a connection with Azure AD.

**$authResult = Connect-AzureAD**

  1. The script contains two variables to set the application name and a reply URL of **https://localhost:5001/signin-oidc**.

**$appDisplayName = &quot;User-Owns-Data Sample App&quot;**

**$replyUrl = &quot;https://localhost:5001/signin-oidc&quot;**

When you register a reply URL with **localhost** with a port number such as **5001** , Azure AD will allow you to perform testing with reply URLs that use localhost and any other port number. For example, you can use a reply URL of **https://localhost:** 5001 **/signin-oidc**.

  1. The script also contains the code below which creates a new **PasswordCredential** object for an app secret.

**# create app secret**

**$newGuid = New-Guid**

**$appSecret = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(($newGuid))))+&quot;=&quot;**

**$startDate = Get-Date**

**$passwordCredential = New-Object -TypeName Microsoft.Open.AzureAD.Model.PasswordCredential**

**$passwordCredential.StartDate = $startDate**

**$passwordCredential.EndDate = $startDate.AddYears(1)**

**$passwordCredential.KeyId = $newGuid**

**$passwordCredential.Value = $appSecret**

  1. Down below, you can see the call to the **New-AzureADApplication** cmdlet which creates a new Azure AD application.

**# create Azure AD Application**

**$aadApplication = New-AzureADApplication `**

**-DisplayName $appDisplayName `**

**-PublicClient $false `**

**-AvailableToOtherTenants $false `**

**-ReplyUrls @($replyUrl) `**

**-Homepage $replyUrl `**

**-PasswordCredentials $passwordCredential**

Note you must execute this script using Windows PowerShell 5 and not PowerShell 7 due to incompatibilities between PowerShell7 and the PowerShell module named **AzureAD**.

1. Execute the PowerShell script named **Create-Azure-ADApplication.ps1**
  1. Execute the PowerShell script named **Create-Azure-ADApplication.ps1**.
  2. When prompted for credentials, log in with an Azure AD user account in the same tenant where you are using Power BI.
  3. When the PowerShell scriptruns successfully, it will create and open a text file named **UserOwnsDataSampleApp.txt**.

![](RackMultipart20201204-4-1di9arb_html_e5ef655874f128df.png)

You should leave the text file **UserOwnsDataSampleApp.txt** open for now. This file contains JSON configuration data that you will copy and paste into the **appsettings.json**.file of the new .NET 5 project you will create the next exercise.

### Exercise 2: Create a .NET 5 Project for a Secure Web Application

In this exercise, you will use the .NET CLI to create a new .NET 5 project for an MVC web application with support for authentication using the new Microsoft authentication library named Microsoft.Identity.Web..

1. Create a new .NET 5 project for an ASP.NET MVC web application.

  1. Using Windows Explorer, create a child folder inside the **C:\DevCamp** folder named **UserOwnsData**.

![](RackMultipart20201204-4-1di9arb_html_262dc34b672c9f45.png)

  1. Launch Visual Studio Code.
  2. Use the **Open Folder…** command in Visual Studio Code to open the **UserOwnsData** folder.

![](RackMultipart20201204-4-1di9arb_html_aa0fa1520bcc4d4e.png)

  1. Once you have open the **UserOwnsData** folder, close the **Welcome** page.

![](RackMultipart20201204-4-1di9arb_html_b1f4f2e4b9ba9d96.png)

1. Use the Terminal console to verify the current version of .NET
  1. Use the **Terminal \&gt; New Terminal** command or the [**Ctrl+Shift+`**] keyboard shortcut to open the Terminal console.

![](RackMultipart20201204-4-1di9arb_html_4e1013ca3f272fc1.png)

  1. You should now see a Terminal console with a cursor where you can type and execute command-line instructions.

![](RackMultipart20201204-4-1di9arb_html_c2adf354b5070b4c.png)

  1. Type the following **dotnet** command-line instruction into the console and press **Enter** to execute it.

**dotnet --version**

  1. When you run the command, the **dotnet** CLI should respond by display the .NET version number.

![](RackMultipart20201204-4-1di9arb_html_c7d1807cf5c318b7.png)

Make sure you .NET version number is **5.0.100** at a minimum. If you are working with .NET CORE version 3.1 or early, the project template files for creating new applications are much different and these lab instructions will not work as expected. If you do not have .NET 5 installed, you must install the .NET 5 SDK before you can move past this point.

1. Run .NET CLI commands to create a new ASP.NET MVC project.
  1. In the Terminal console, type and execute the following command to generate a new .NET console application.

**dotnet new mvc --auth SingleOrg**

The **--auth SingleOrg** parameter instructs the .NET 5 CLI to generate the new web application with extra code with authentication support using Microsoft&#39;s new authentication library named Microsoft.Identity.Web.

  1. After running the **dotnet new** command, you should see new files have been added to the project.

![](RackMultipart20201204-4-1di9arb_html_2b66fdae3b5d3bee.png)

1. Copy the JSON in **UserOwnsDataSampleApp.txt** into the **appsettings.json** file in your project.
  1. Return to the **UserOwnsData** project in Visual Studio Code and open the **appsettings.json** file.
  2. The **appsettings.json** file should initially appear like the screenshot below.

![](RackMultipart20201204-4-1di9arb_html_e535a568ee899676.png)

  1. Delete the contents of **appsettings.json** and replace it by copying and pasting the contents of **UserOwnsDataSampleApp.txt**

![](RackMultipart20201204-4-1di9arb_html_3688f0ff62e92dcc.png)

Note the **PowerBi:ServiceRootUrl** parameter has been added as a custom configuration value to track the base URL to the Power BI Service. When you are programming against the Power BI Service in Microsoft public cloud, the URL is [https://api.powerbi.com/](https://api.powerbi.com/). However, the root URL for the Power BI Service will be different in other clouds such as the government cloud. Therefore, this value will be stored as a project configuration value so it is easy to change whenever required.

1. Configure Visual Studio Code with the extensions needed for C# development with .NET 5.
  1. Click on the button at the bottom of the left navigation menu to display the **EXTENSION** pane.
  2. You should be able to see what extensions are currently installed.
  3. You should also be able to search to find new extensions you&#39;d like to install.

![](RackMultipart20201204-4-1di9arb_html_38d428d171135b3c.png)

  1. Find and install the **C#** extension from Microsoft if it is not already installed.

![](RackMultipart20201204-4-1di9arb_html_cacf3886eb883639.png)

  1. Find and install the **Debugger for Chrome** extension from Microsoft if it is not already installed.
  2. You should be able to confirm that the **C#** extension and the **Debugger for Chrome** extensions are now installed.

![](RackMultipart20201204-4-1di9arb_html_568722c4ffc53d46.png)

It is OK if you have other Visual Studio Code extensions installed as well. It&#39;s just important that you install these two extensions in addition to whatever other extensions you may have installed.

1. Generate the project required for building your project and running it in the .NET 5 debugger.
  1. Open the Visual Studio Code Command Palette by using the **Ctrl** + **Shift** + **P**  keyboard combination.
  2. Type .NET into the Command Palette search box.
  3. Select the command titled **.NET: Generate Assets for Build and Debug**.

![](RackMultipart20201204-4-1di9arb_html_4871d0d7c1c7fd9c.png)

  1. When the command runs successfully, in generates the files **launch.json** and **tasks.json** a new folder named **.vscode**.

![](RackMultipart20201204-4-1di9arb_html_512b6c1133c89130.png)

It&#39;s not uncommon for the configuration of the C# extension (i.e. Omnisharp) in Visual Studio Code to cause errors when running the command to generate the assets for build and debug. If this is the case, it can be tricky fix this problem. If the previous step to generate build and debug assets did not successfully create the **launch.json** file and the **tasks.json** file in the **.vscode** folder, you can copy these two essential files from **Troubleshooting/.vscode** folder located inside the **StudentFiles** folder.

1. Run and test the **UserOwnsData** web application using Visual Studio Code and the .NET 5 debugger.
  1. Start the .NET 5 debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.

![](RackMultipart20201204-4-1di9arb_html_2457f1bbe243190d.png)

The UserOwnsData web application is currently configured authenticate the user before the user is able to view the home page. Therefore, you will be prompted to log in as soon as you launch the application in the .NET debugger.

  1. When prompted to Sign in, log in using your organizational account.

![](RackMultipart20201204-4-1di9arb_html_24f23497a0a1e63f.png)

  1. Once you have signed in, you will be prompted to by the **Permissions requested** dialog to grant consent to the application.
  2. Click the **Accept** button to continue.

![](RackMultipart20201204-4-1di9arb_html_41b96f5bc7d87f89.png)

  1. You should now see the home page for the **UserOwnsData** web application which should match the following screenshot.

![](RackMultipart20201204-4-1di9arb_html_8633bc058188012a.png)

  1. You&#39;re done testing. Close the browser, return to Visual Studio Code and stop the debug session using the debug toolbar.

![](RackMultipart20201204-4-1di9arb_html_5b06e8bb5bc28586.png)

You now got to the point where the **UserOwnsData** web application is up and running and it can successfully authenticate the user. Over the next few steps you will add some custom HTML and CSS styles to make the application a bit more stylish. You will also configure the home page so it does not require authentication to improve the user login experience.

1. Copy and paste a set of pre-written CSS styles into the **UserOwnsData** project&#39;s **site.css** file.
  1. Expand the **wwwroot** folder and then expand the **css** folder inside to examine the contents of the **wwwroot/css** folder.
  2. Open the CSS file named **site.css** and delete any existing content inside.

![](RackMultipart20201204-4-1di9arb_html_5d949a10cdddd712.png)

  1. Using the Windows Explorer, look inside the **StudentLabFiles** folder and locate the file named **Exercise 2 - site.css.txt**.
  2. Open **Exercise 2 - site.css.txt** up in a text editor and copy all of its contents into the Windows clipboard.
  3. Return to Visual Studio Code and paste the contents of **Exercise 2 - site.css.txt** into **sites.css**.

![](RackMultipart20201204-4-1di9arb_html_e3bb6d0720efca45.png)

  1. Save your changes and close **site.css**.
1. Copy a custom **favicon.ico** file to the **wwwroot** folder.
  1. Using the Windows Explorer, look inside the **StudentLabFiles** folder and locate the file named **favicon.ico**.
  2. Copy the **favicon.ico** file into the **wwwroot** folder of your project.

![](RackMultipart20201204-4-1di9arb_html_28a2d90e99d07209.png)

Any file you add the **wwwroot** folder will appear at the root folder of the website created by the **UserOwnsData** project. By adding the **favicon.ico** file, this web application will now display a custom **favicon.ico** in the browser page tab.

1. Modify the partial razor view file named **\_LoginPartial.cshtml** to integrate with the **Microsoft.Identity.Web** authentication library..
  1. Expand the **Views \&gt; Shared** folder and locate the partial view named **\_LoginPartial.cshtml**.
  2. Open **\_LoginPartial.cshtml** in an editor window.
  3. In the existing code, locate the code **Hello @User.Identity.Name!** which is used to display information about the user.

![](RackMultipart20201204-4-1di9arb_html_f24f857845c1154e.png)

  1. Replace **Hello @User.Identity.Name!** with **Hello @User.FindFirst(&quot;name&quot;).Value** as shown in the following screenshot.

![](RackMultipart20201204-4-1di9arb_html_98ae5905b64ee86f.png)

With this update, the application will display the user&#39;s display name in the welcome message instead of using the user principal name. In other words, the user greeting will now display a message like **Hello Betty White** instead of **Hello BettyW@Contoso.com!**.

  1. Save your changes and close **\_LoginPartial.cshtml**.
1. Modify the HTML in **Index.cshtml** to display differently depending on whether the user has logged in or not.
  1. Expand the **Views \&gt; Home** folder and locate the view file named **Index.cshtml**.
  2. Open **Index.cshtml** in an editor window.
  3. Delete the contents of **Index.cshtml** and replace it with the code shown in the following code listing.

**@using System.Security.Principal**

**@if (User.Identity.IsAuthenticated) {**

**\&lt;div class=&quot;jumbotron&quot;\&gt;**

**\&lt;h2\&gt;Welcome @User.FindFirst(&quot;name&quot;).Value\&lt;/h2\&gt;**

**\&lt;p\&gt;You have now logged into this application.\&lt;/p\&gt;**

**\&lt;/div\&gt;**

**}**

**else {**

**\&lt;div class=&quot;jumbotron&quot;\&gt;**

**\&lt;h2\&gt;Welcome to the User-Owns-Data Tutorial\&lt;/h2\&gt;**

**\&lt;p\&gt;Click the \&lt;strong\&gt;sign in\&lt;/strong\&gt; link in the upper right to get started\&lt;/p\&gt;**

**\&lt;/div\&gt;**

**}**

  1. Once you have copied the code from above, save your changes and close **Index.cshtml**.

![](RackMultipart20201204-4-1di9arb_html_8669391fce46bef2.png)

When you create a new .NET 5 project which supports authentication, the underlying project template creates a home page that requires authentication. To support a more natural login experience for the user, it often makes sense to configure your application so that an anonymous user can access the home page. In the next step you will modify the **Home** controller so the home page is accessible to the anonymous user.

1. Modify the **Index** action method in **HomeController.cs** to support anonymous access.
  1. Inside the **Controllers** folder, locate **HomeControllers.cs** and open this file in an editor window.
  2. Locate the **Index** method inside the **HomeController** class.

![](RackMultipart20201204-4-1di9arb_html_8c1c191aeed482f8.png)

  1. Add the **[AllowAnonymous]** attribute to the **Index** method as shown in the following code listing.

**[AllowAnonymous]**

**public IActionResult Index()**

**{**

**return View();**

**}**

  1. Save your changes and close **HomeController.cs**.

You have now modified the project to the point where you can run the web application in the .NET 5 debugger. In the next step, you will start the debugger so you can test your web application as it runs in the browser.

1. Test the **UserOwnsData** project by running it in the .NET 5 debugging environment.
  1. Start the .NET 5 debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.
  2. Once the debugging session has initialized, the browser should display the home page using anonymous access.
  3. Click the **Sign in** link to test put the user experience when authenticating with Azure AD.

![](RackMultipart20201204-4-1di9arb_html_41130789c6b0db9f.png)

  1. Once you have signed in, you should be able to see the text on the home page changes because the user is authenticated.

![](RackMultipart20201204-4-1di9arb_html_6a8838428fa60f37.png)

At this point, the user should be authenticated. For example, you should see the logged in user name to the left of the **Sign out** link in the top right corner. You should also see that the home page displays text that welcomes the user by name.

If the web page does not appear with a yellow background as shown in the screenshot above, it&#39;s possible your browser has cached the original version of the **site.css** file. If this is the case, you must clear the browser cache so it loads the latest version of **site.css**.

1. Test the user experience for logging out.
  1. Click the **Sign out** link to begin the logout experience.

![](RackMultipart20201204-4-1di9arb_html_6ff4da7090629c7f.png)

  1. After logging out, you&#39;ll be directed to the **Microsoft.Identity.Web** logout page at **/MicrosoftIdentity/Account/SignedOut**.

![](RackMultipart20201204-4-1di9arb_html_a7b6756ba1badefe.png)

  1. You&#39;re done testing. Close the browser, return to Visual Studio Code and stop the debug session using the debug toolbar.

In the next step, you will add a new controller action and view named **Embed**. However, instead of creating a new controller action and view, you will simply the rename the controller action and view named **Privacy** that were automatically added by the project template.

1. Create a new controller action named **Embed**.
  1. Locate the **HomeController.cs** file in the **Controllers** folder and open it in an editor window.
  2. Look inside the **HomeController** class and locate the method named **Privacy**.

**[AllowAnonymous]**

**public IActionResult Index() {**

**return View();**

**}**

**public IActionResult Privacy() {**

**return View();**

**}**

  1. Rename of the **Privacy** method to **Embed**. No changes to the method body are required.

**[AllowAnonymous]**

**public IActionResult Index() {**

**return View();**

**}**

**public IActionResult Embed() {**

**return View();**

**}**

Note that, unlike the **Index** method, the **Embed** method does not have the **AllowAnonymous** attribute. That means only authenticated users will be able to navigate to this page. One really nice aspect of the ASP.NET MVC architecture is that it will automatically trigger an interactive login whenever an anonymous user attempts to navigate to a secured page such as **Embed**.

1. Create a new MVC view for the **Home** controller named **Embed.cshtml**.
  1. Look inside the **Views \&gt; Home** folder and locate the razor view file named **Privacy.cshtml**.

![](RackMultipart20201204-4-1di9arb_html_2dde20c2f583d4f2.png)

  1. Rename the **Privacy.cshtml** razor file to **Embed.cshtml**..

![](RackMultipart20201204-4-1di9arb_html_1219ce92870d9dba.png)

  1. Open **Embed.cshtml** in a code editor.
  2. Delete the existing contents of **Embed.cshtml** and replace it with the follow line of HTML code.

**\&lt;h2\&gt;TODO: Embed Report Here\&lt;/h2\&gt;**

  1. Save your changes and close **Embed.cshtml**.

In a standard .NET 5 web application that uses MVC, there is a shared page layout defined in a file named **\_Layouts.cshtml** which is located in the **Views \&gt; Shared** folder. In the next step you will modify the shared layout in the **\_Layouts.cshtml** file so that you can add a link to the **Embed** page into the top navigation menu.

1. Modify the shared layout in **\_Layout.cshtml** to include a link to the **Embed** page.
  1. Inside the **Views \&gt; Shared** folder, locate **\_Layouts.cshtml** and open this shared view file in an editor window.
  2. Using Windows Explorer, look inside the **StudentLabFiles** folder and locate the file named **\_Layout.cshtml**.
  3. Open **Exercise 2 - \_Layout.cshtml.txt** in the **StudentLabFiles** folder copy its contents to the Windows clipboard.
  4. Return to Visual Studio Code and paste the contents of the Windows clipboard into the **\_Layouts.cshtml** file.

![](RackMultipart20201204-4-1di9arb_html_402a23f01e727163.png)

  1. Save your changes and close **\_Layouts.cshtml**
1. Run the web application in the Visual Studio Code debugger to test the new **Embed** page.
  1. Start the Visual Studio Code debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.
  2. The **UserOwnsData** web application should display the home page as shown to an anonymous user.
  3. Click on the **Embed** link in the top nav menu to navigate to the **Embed** page.

![](RackMultipart20201204-4-1di9arb_html_e4b842c9dcd5e2f4.png)

  1. When you attempt to navigate to the **Embed** page as an anonymous user, you&#39;ll be prompted to pick an account and log in.
  2. Log in using your user name and password.

![](RackMultipart20201204-4-1di9arb_html_9f64bd3435ab822f.png)

  1. Once you have logged in, you should be automatically redirected to the **Embed** page.

![](RackMultipart20201204-4-1di9arb_html_d4b5f97f867584c.png)

  1. You&#39;re done testing. Close the browser, return to Visual Studio Code and stop the debug session using the debug toolbar.

The next step is an _optional step_ for those campers that prefer developing with Visual Studio 2019 instead of Visual Studio Code.
 If you are happy developing with Visual Studio Code and are not interested in developing .NET 5 projects using Visual Studio 2019, you can skip the next step and move ahead to _Exercise 3: Call the Power BI Service API_.

1. Open and test the **UserOwnsData** project using Visual Studio 2019.
  1. Launch Visual Studio 2019 – You can use any edition including the Enterprise edition, Pro edition or Community edition.
  2. From the **File** menu, select the **Open \&gt; Project/Solution…** command.

![](RackMultipart20201204-4-1di9arb_html_7776e875c3b2e158.png)

  1. In the **Open Project/Solution** dialog, select the **UserOwnsData.csproj** file in the **UserOwnsData** folder and click **Open**.
  2. The **UserOwnsData** project should now be open in Visual Studio 2019 as shown in the following screenshot.

![](RackMultipart20201204-4-1di9arb_html_af1182fa8f9f3a3.png)

There is one big difference between developing with Visual Studio Code and Visual Studio 2019. Visual Studio Code only requires project files (\*.csproj). However, Visual Studio 2019 requires that you work with both project files and solution files (\*.sln). In the next step you will save a new project file for the **UserOwnsData** solution to make it easier to develop this project with Visual Studio 2019.

  1. In the **Solution Explorer** on the right, select the top node in the tree with the caption **Solution &quot;UserOwnsData&quot;**.
  2. From the **File** menu, select the **Save UserOwnsData.sln** menu command.

![](RackMultipart20201204-4-1di9arb_html_d38fa37a022ed39d.png)

  1. Save the solution file **UserOwnsData.sln** in the **UserOwnsData** project folder

![](RackMultipart20201204-4-1di9arb_html_8166fb70a258b8ba.png)

Remember that the **UserOwnsData.sln** file is only used by Visual Studio 2019 and it not used at all in Visual Studio Code.

### Exercise 3: Call the Power BI Service API

In this exercise, you will begin by ensuring you have a Power BI app workspace and a report for testing. After that, you will add support to the **UserOwnsData** web application to acquire access tokens from Azure AD and to call the Power BI Service API. By the end of this exercise, your code will be able to call to the Power BI Service API to retrieve data about a report required for embedding.

1. Using the browser, log into the Power BI Service with the same user account you used to create the Azure AD application earlier.

  1. Navigate the Power BI portal at [https://app.powerbi.com](https://app.powerbi.com/) and if prompted, log in using your credentials.

1. Create a new app workspace named **Dev Camp Demos**.
  1. Click the **Workspace** flyout menu in the left navigation.

![](RackMultipart20201204-4-1di9arb_html_47c47e90633f3390.png)

  1. Click the **Create app workspace** button to display the **Create an app workspace** dialog.

![](RackMultipart20201204-4-1di9arb_html_17b74df43efc3002.png)

  1. In the **Create an app workspace** pane, enter a workspace name such as **Dev Camp Demos**.
  2. Click the **Save** button to create the new app workspace.

![](RackMultipart20201204-4-1di9arb_html_b3407f31da40a5f6.png)

  1. When you click **Save** , the Power BI service should create the new app workspace and then switch your current Power BI session to be running within the context of the new **Dev Camp Demos** workspace.

![](RackMultipart20201204-4-1di9arb_html_4a99a3fdf35065e8.png)

Now that you have created the new app workspace, the next step is to upload a PBIX project file created with Power BI Desktop. You are free to use your own PBIX file as long as the PBIX file does not have row-level security (RLS) enabled. If you don&#39;t have your own PBIX file, you can download the sample PBIX file named [**COVID-19 US.pbix**](https://github.com/PowerBiDevCamp/pbix-samples/raw/main/COVID-19%20US.pbix) and use that instead.

1. Upload a PBIX file to create a new report and dataset.
  1. Click **Add content** to navigate to the **Get Data** page.
  2. Click the **Get** button in the **Files** section.

![](RackMultipart20201204-4-1di9arb_html_81441a8696b0274b.png)

  1. Click on **Local File** in order to select a PBIX file that you have on your local hard drive.

![](RackMultipart20201204-4-1di9arb_html_97a619b2b13148e4.png)

  1. Select the PBIX file and click the **Open** button to upload it to the Power BI Service.

![](RackMultipart20201204-4-1di9arb_html_4d7a46a3a715ad85.png)

  1. The Power BI Service should have created a report and a dashboard from the PBIX file you uploaded.
  2. If the Power BI Service created a dashboard as well, delete this dashboard as you will not need it.

![](RackMultipart20201204-4-1di9arb_html_6b70e228f3b7d303.png)

1. Open the report to see what it looks like when displayed in the Power BI Service.
  1. Click on the report to open it.

![](RackMultipart20201204-4-1di9arb_html_f8d522fadb873ace.png)

  1. You should now be able to see the report.

![](RackMultipart20201204-4-1di9arb_html_d37c32e9d95f4067.png)

In the next step, you will find and record the GUID-based IDs for the report and its hosting workspace. You will then use these IDs later in this exercises when you first write the code to embed a report in the **UserOwnsData** web application.

1. Get the Workspace ID and the Report ID from the report URL.
  1. Locate and copy the app workspace ID from the report URL by copying the GUID that comes after **/groups/**.

![](RackMultipart20201204-4-1di9arb_html_75d0bf667f31f991.png)

  1. Open up a new text file in a program such as Notepad and paste in the value of the workspace ID.
  2. Locate and copy the report ID from the URL by copying the GUID that comes after **/reports/**.

![](RackMultipart20201204-4-1di9arb_html_c324f3be48b78b8f.png)

  1. Copy the report ID into the text file Notepad.

![](RackMultipart20201204-4-1di9arb_html_2f513b592d88ebda.png)

Leave the text file open for now. In a step later in this exercise, you will copy and paste these IDs into your C# code.

1. Add the NuGet package for the **Power BI .NET SDK**.

  1. Move back to the Terminal so you can execute another dotnet CLI command.
  2. Type and execute the following **dotnet add package** command to add the NuGet package for the **Power BI .NET SDK**.

**dotnet add package Microsoft.PowerBi.Api**

![](RackMultipart20201204-4-1di9arb_html_e5644dcd3a7e4e66.png)

  1. Open the **UserOwnsData.csproj** file. You should now see this file contains a package reference to **Microsoft.PowerBi.Api**.

![](RackMultipart20201204-4-1di9arb_html_e924e0c55ba63030.png)

  1. Close the the **UserOwnsData.csproj** file without saving any changes.

Your project now includes the NuGet package for the Power BI .NET SDK so you can begin to program against the classes from this package in the **Microsoft.PowerBI.Api** namespace.

1. Create a new C# class named **PowerBiServiceApi** in which you will add code for calling the Power BI Service API.
  1. Return to the **UserOwnsData** project in Visual Studio Code.
  2. Create a new top-level folder in the **UserOwnsData** project named **Services**.

![](RackMultipart20201204-4-1di9arb_html_830bb8a61075201f.png)

  1. Inside the **Services** folder, create a new C# source file named **PowerBiServiceApi.cs**.

![](RackMultipart20201204-4-1di9arb_html_df9763c7b6effc0b.png)

  1. Copy and paste the following code into **PowerBiServiceApi.cs** to provide a starting point.

**using System;**

**using System.Linq;**

**using System.Threading.Tasks;**

**using Microsoft.Extensions.Configuration;**

**using Microsoft.Identity.Web;**

**using Microsoft.Rest;**

**using Microsoft.PowerBI.Api;**

**using Newtonsoft.Json;**

**namespace UserOwnsData.Services {**

**public class EmbeddedReportViewModel {**

**//TODO: implement this class**

**}**

**public class PowerBiServiceApi {**

**//TODO: implement this class**

**}**

**}**

  1. Implement the **EmbeddedReportViewModel** class using the following code.

**public class EmbeddedReportViewModel {**

**public string Id;**

**public string Name;**

**public string EmbedUrl;**

**public string Token;**

**}**

The **EmbeddedReportViewModel** class is designed as a view model class to pass the data needed to embed a single report. You will use this class later in this lab to pass report embedding data from an MVC controller to an MVC view.

  1. Begin your implementation by adding two private fields named **tokenAcquisition** and **urlPowerBiServiceApiRoot**.

**public class PowerBiServiceApi {**

**private ITokenAcquisition tokenAcquisition { get; }**

**private string urlPowerBiServiceApiRoot { get; }**

**}**

  1. Add the following constructor to initialize the two private fields named **tokenAcquisition** and **urlPowerBiServiceApiRoot**.

**public class PowerBiServiceApi {**

**private ITokenAcquisition tokenAcquisition { get; }**

**private string urlPowerBiServiceApiRoot { get; }**

**public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition tokenAcquisition) {**

**this.urlPowerBiServiceApiRoot = configuration[&quot;PowerBi:ServiceRootUrl&quot;];**

**this.tokenAcquisition = tokenAcquisition;**

**}**

**}**

This code uses the .NET dependency injection pattern. When your class needs to use a service, you can simply add a constructor parameter for that service and the .NET runtime takes care of passing the service instance at run time. In this case, the constructor is injecting an instance of the .NET configuration service using the **IConfiguration** parameter which is used to retrieve the **PowerBi:ServiceRootUrl** configuration value from **appsettings.json**. The **ITokenAcquisition** parameter which is named **tokenAcquisition** holds a reference to the Microsoft authentication service provided by the **Microsoft.Identity.Web** library and will be used to acquire access tokens from Azure AD.

  1. Place your cursor at the bottom of the **PowerBiServiceApi** class and add another new line so you can add more members.
  2. At the bottom off the **PowerBiServiceApi** class, add the following static read-only field named **RequiredScopes**.

**public static readonly string[] RequiredScopes = new string[] {**

**&quot;https://analysis.windows.net/powerbi/api/Group.Read.All&quot;,**

**&quot;https://analysis.windows.net/powerbi/api/Report.ReadWrite.All&quot;,**

**&quot;https://analysis.windows.net/powerbi/api/Dataset.ReadWrite.All&quot;,**

**&quot;https://analysis.windows.net/powerbi/api/Content.Create&quot;,**

**&quot;https://analysis.windows.net/powerbi/api/Workspace.ReadWrite.All&quot;**

**};**

The **RequiredScopes** field is a string array with a set of delegated permissions supported by the Power BI Service API. Your application will pass these permissions when it calls to Azure AD to acquire an access token.

  1. Move down in the **PowerBiServiceApi** class below the **RequiredScopes** field and add the **GetAccessToken** method.

**public string GetAccessToken() {**

**return this.tokenAcquisition.GetAccessTokenForUserAsync(RequiredScopes).Result;**

**}**

  1. Move down below the **GetAccessToken** method and add the **GetPowerBiClient** method.

**public PowerBIClient GetPowerBiClient() {**

**var tokenCredentials = new TokenCredentials(GetAccessToken(), &quot;Bearer&quot;);**

**return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);**

**}**

  1. Move down below the **GetPowerBiClient** method and add the **GetReport** method.

**public async Task\&lt;EmbeddedReportViewModel\&gt; GetReport(Guid WorkspaceId, Guid ReportId) {**

**PowerBIClient pbiClient = GetPowerBiClient();**

**// call to Power BI Service API to get embedding data**

**var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId, ReportId);**

**// return report embedding data to caller**

**return new EmbeddedReportViewModel {**

**Id = report.Id.ToString(),**

**EmbedUrl = report.EmbedUrl,**

**Name = report.Name,**

**Token = GetAccessToken()**

**};**

**}**

  1. Save and close **PowerBIServiceApi.cs**.

Note that **Exercise 3 - PowerBiServiceApi.cs.txt** in the **StudentLabFiles** folder contains the final code for **PowerBiServiceApi.cs**.

1. Modify the code in **Startup.cs** to properly register the services required for user authentication and access token acquisition.
  1. Open the **Startup.cs** file in an editor window.
  2. Underneath the existing **using** statements, add the following **using** statement;

**using UserOwnsData.Services;**

  1. Look inside the **ConfigureServices** method and locate the following line of code.

**public void ConfigureServices(IServiceCollection services) {**

**services.AddMicrosoftIdentityWebAppAuthentication(Configuration);**

  1. Replace the call to **services.**** AddMicrosoftIdentityWebAppAuthentication** with the following code.

**services**

**.AddMicrosoftIdentityWebAppAuthentication(Configuration)**

**.EnableTokenAcquisitionToCallDownstreamApi(PowerBiServiceApi.RequiredScopes)**

**.AddInMemoryTokenCaches();**

  1. Move below the call to **AddInMemoryTokenCaches** and add the following code.

**services.AddScoped(typeof(PowerBiServiceApi));**

  1. At this point, the **ConfigureService** method in **Startup.cs** should match the following code listing.

**public void ConfigureServices(IServiceCollection services) {**

**services**

**.AddMicrosoftIdentityWebAppAuthentication(Configuration)**

**.EnableTokenAcquisitionToCallDownstreamApi(PowerBiServiceApi.RequiredScopes)**

**.AddInMemoryTokenCaches();**

**services.AddScoped(typeof(PowerBiServiceApi));**

**var mvcBuilder = services.AddControllersWithViews(options =\&gt; {**

**var policy = new AuthorizationPolicyBuilder()**

**.RequireAuthenticatedUser()**

**.Build();**

**options.Filters.Add(new AuthorizeFilter(policy));**

**});**

**mvcBuilder.AddMicrosoftIdentityUI();**

**services.AddRazorPages();**

**}**

The code in **ConfigureServices** accomplishes several important things. The call to **AddMicrosoftWebAppCallsWebApi** configures the Microsoft authentication library to acquire access tokens. Next, the call to **AddInMemoryTokenCaches** configures a token cache that the Microsoft authentication library will use to cache access tokens and refresh tokens behind the scenes. Finally, the call to **services.AddScoped(typeof(PowerBiServiceApi))** configures the **PowerBiServiceApi** class as a service class that can be added to other classes using dependency injection.

1. Modify the **HomeController** class to program against the **PowerBiServiceApi** class.
  1. Inside the **Controllers** folder, locate **HomeController.cs** and open it in an editor window.
  2. Underneath the existing **using** statements, add a **using** statement to import the **UserOwnsData.Services** namespace.

**using UserOwnsData.Services;**

  1. At the top of the **HomeController** class locate the **\_logger** field and the constructor as shown in the following code listing.

**[Authorize]**

**public class HomeController : Controller {**

**private readonly ILogger\&lt;HomeController\&gt; \_logger;**

**public HomeController(ILogger\&lt;HomeController\&gt; logger) {**

**\_logger = logger;**

**}**

  1. Remove the **\_logger** field and the existing constructor and replace them with the following code.

**[Authorize]**

**public class HomeController : Controller {**

**private PowerBiServiceApi powerBiServiceApi;**

**public HomeController(PowerBiServiceApi powerBiServiceApi) {**

**this.powerBiServiceApi = powerBiServiceApi;**

**}**

This is another example of using dependency injection. Since you registered the **PowerBiServiceApi** class as a service by calling **services.AddScoped** in the **ConfigureServices** method, you can simply add a **PowerBiServiceApi** parameter to the constructor and the .NET 5 runtime will take care of creating a **PowerBiServiceApi** instance and passing it to the constructor.

  1. Locate the **Embed** method implementation in the **HomeController** class and replace it with the following code.

**public async Task\&lt;IActionResult\&gt; Embed() {**

**Guid workspaceId = new Guid(&quot;912f2b34-7daa-4589-83df-35c75944d864&quot;);**

**Guid reportId = new Guid(&quot;cd496c1c-8df0-48e7-8b92-e2932298743e&quot;);**

**var viewModel = await powerBiServiceApi.GetReport(workspaceId, reportId);**

**return View(viewModel);**

**}**

1. Modify the HTML and razor code in the view file named **Embed.cshtml**.
  1. Locate the **Embed.cshtml** razor file inside the **Views \&gt; Home** folder and open this file in an editor window.
  2. Delete the contents of **Embed.cshtml** and replace it with the following code which creates a table to display report data.

**@model UserOwnsData.Services.EmbeddedReportViewModel;**

**\&lt;style\&gt;**

**table td {**

**min-width: 120px;**

**word-break: break-all;**

**overflow-wrap: break-word;**

**font-size: 0.8em;**

**}**

**\&lt;/style\&gt;**

**\&lt;h3\&gt;Report View Model Data\&lt;/h3\&gt;**

**\&lt;table class=&quot;table table-bordered table-striped table-sm&quot; \&gt;**

**\&lt;tr\&gt;\&lt;td\&gt;Report Name\&lt;/td\&gt;\&lt;td\&gt;@Model.Name\&lt;/td\&gt;\&lt;/tr\&gt;**

**\&lt;tr\&gt;\&lt;td\&gt;Report ID\&lt;/td\&gt;\&lt;td\&gt;@Model.Id\&lt;/td\&gt;\&lt;/tr\&gt;**

**\&lt;tr\&gt;\&lt;td\&gt;Embed Url\&lt;/td\&gt;\&lt;td\&gt;@Model.EmbedUrl\&lt;/td\&gt;\&lt;/tr\&gt;**

**\&lt;tr\&gt;\&lt;td\&gt;Token\&lt;/td\&gt;\&lt;td\&gt;@Model.Token\&lt;/td\&gt;\&lt;/tr\&gt;**

**\&lt;/table\&gt;**

  1. The code in **Embed.cshtml** should now match the following screenshot..

![](RackMultipart20201204-4-1di9arb_html_8c0e9b9d09484fa8.png)

  1. Save your changes and close **Embed.cshtml**.
1. Run the web application in the Visual Studio Code debugger to test the new **Embed** page.
  1. Start the Visual Studio Code debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.
  2. The **UserOwnsData** web application should display the home page as shown to an anonymous user.
  3. Click on the **Embed** link in the top nav menu to navigate to the **Embed** page.

![](RackMultipart20201204-4-1di9arb_html_29473cf9819beb12.png)

  1. If you are prompted to enter your credentials, enter your user name and password and log in.
  2. After you have authenticated for the first time, you should be prompted with a **Permissions Requested** dialog.
  3. Click the **Accept** button to consent to the application using the requested delegated permissions on your behalf.

![](RackMultipart20201204-4-1di9arb_html_d852deca96130689.png)

The **Permissions requested** dialog is only shown to each user during the first successful login. Once a user clicks **Accept** , Azure AD remembers that the user has consented to the required permissions and does not need to prompt the user about permission requests.

  1. Once you navigate to the **Embed** page, it should display a table containing the embedding data for your Power BI report.

![](RackMultipart20201204-4-1di9arb_html_b26030226b6355fa.png)

  1. You&#39;re done testing. Close the browser, return to Visual Studio Code and stop the debug session using the debug toolbar.

You are now half way in your development efforts to embed a Power BI report. You have written the code to call the Power BI Service API and retrieve the data required to embed a report. In the next exercise, you will complete the Power BI embedding implementation by adding client-side JavaScript code which programs against the Power BI JavaScript API.

### Exercise 4: Embedding a Report using powerbi.js

In this exercise, you will modify the view named **Embed.cshtml** to embed a Power BI report on a web page. Your work will involve adding a new a JavaScript file named **embed.js** in which you will write the minimal client-side code required to embed a report.

1. Modify the razor view file named **Embed.cshtml**.

  1. Inside the **Views \&gt; Home** folder, locate and open **Embed.cshtml** in an editor window.
  2. Replace the contents of **Embed.cshtml** with the following code.

**@model UserOwnsData.Services.EmbeddedReportViewModel;**

**\&lt;div id=&quot;embed-container&quot; style=&quot;height:800px;&quot;\&gt;\&lt;/div\&gt;**

**@section Scripts {**

**}**

Note that the div element with the ID of **embed-container** will be used as the embed container.

Over the next few steps, you will add three **script** tags into the **Scrips** section. The benefit of adding script tags into the **Scripts** section is that they will load after the JavaScript libraries such as jquery which are loaded from the shared view **\_Layout.cshtml**.

  1. Place your cursor inside the **Scripts** section and paste in the following **script** tag to import **powerbi.min.js** from a CDN.

**\&lt;script src=&quot;https://cdn.jsdelivr.net/npm/powerbi-client@2.13.3/dist/powerbi.min.js&quot;\&gt;\&lt;/script\&gt;**

**powerbi.min.js** is the JavaScript file that loads the client-side library named the **Power BI JavaScript API**.

  1. Underneath the **script** tag for the Power BI JavaScript API, add a second **script** tag using the following code.

**\&lt;script\&gt;**

**var viewModel = {**

**reportId: &quot;@Model.Id&quot;,**

**embedUrl: &quot;@Model.EmbedUrl&quot;,**

**token: &quot;@Model.Token&quot;**

**};**

**\&lt;/script\&gt;**

This **script** tag is creates a JavaScript object named **viewModel** which is accessible to the JavaScript code you&#39;ll write later in this lab.

  1. Underneath the other two **script** tags, add a third **script** tag to load a custom JavaScript file named **embed.js**.

**\&lt;script src=&quot;~/js/embed.js&quot;\&gt;\&lt;/script\&gt;**

Note that the JavaScript file named **embed.js** does not exist yet. You will create the **embed.js** file in the next step.

  1. When you are done, the contents you have in **Embed.cshtml** should match the following code listing.

**@model UserOwnsData.Services.EmbeddedReportViewModel;**

**\&lt;div id=&quot;embed-container&quot; style=&quot;height:800px;&quot;\&gt;\&lt;/div\&gt;**

**@section Scripts {**

**\&lt;script src=&quot;https://cdn.jsdelivr.net/npm/powerbi-client@2.13.3/dist/powerbi.min.js&quot;\&gt;\&lt;/script\&gt;**

**\&lt;script\&gt;**

**var viewModel = {**

**reportId: &quot;@Model.Id&quot;,**

**embedUrl: &quot;@Model.EmbedUrl&quot;,**

**token: &quot;@Model.Token&quot;**

**};**

**\&lt;/script\&gt;**

**\&lt;script src=&quot;~/js/embed.js&quot;\&gt;\&lt;/script\&gt;**

**}**

  1. Save your changes and close **Embed.cshtml**.

The final step is to add a new JavaScript file named **embed.js** with the code required to embed a report.

1. Add a new JavaScript file named **embed.js**.
  1. Locate the top-level folder named **wwwroot** and expand it.
  2. Locate the **js** folder inside the **wwwroot** folder and expand that.
  3. Currently, there should be one file inside the **wwwroot \&gt; js** folder named **site.js**.

![](RackMultipart20201204-4-1di9arb_html_55a8d944a4dc7875.png)

  1. Rename **site.js** to **embed.js**.

![](RackMultipart20201204-4-1di9arb_html_278ceb881f227770.png)

1. Add the JavaScript code to **embed.js** to embed a report.
  1. Open **embed.js** in an editor window.
  2. Delete whatever content exists inside **embed.js**.
  3. Paste the following code into **embed.js** to provide a starting point.

**$(function(){**

**// 1 - get DOM object for div that is report container**

**// 2 - get report embedding data from view model**

**// 3 - embed report using the Power BI JavaScript API.**

**// 4 - add logic to resize embed container on window resize event**

**});**

You will now copy and paste four sections of JavaScript code into **embed.js** to complete the implementation. Note that you can copy and paste all the code at once from copying the contents of **Exercise 4 - embed.js.txt** in the **StudentLabFiles** folder.

  1. Add the following JavaScript code to create a variable named **reportContainer** which holds a reference to **embed-container**.

**// 1 - get DOM object for div that is report container**

**var reportContainer = document.getElementById(&quot;embed-container&quot;);**

  1. Add code to create 3 variables named **reportId** , **embedUrl** and **token** which are initialized from the global **viewModel** object.

**// 2 - get report embedding data from view model**

**var reportId = window.viewModel.reportId;**

**var embedUrl = window.viewModel.embedUrl;**

**var token = window.viewModel.token**

Now this JavaScript code has retrieved the three essential pieces of data from **window.viewModel** to embed a Power BI report.

  1. Add the following code to embed a report by calling the **powerbi.embed** function provided by the Power BI JavaScript API.

**// 3 - embed report using the Power BI JavaScript API.**

**var models = window[&#39;powerbi-client&#39;].models;**

**var config = {**

**type: &#39;report&#39;,**

**id: reportId,**

**embedUrl: embedUrl,**

**accessToken: token,**

**permissions: models.Permissions.All,**

**tokenType: models.TokenType.Aad,**

**viewMode: models.ViewMode.View,**

**settings: {**

**panes: {**

**filters: { expanded: false, visible: true },**

**pageNavigation: { visible: false }**

**}**

**}**

**};**

**// Embed the report and display it within the div container.**

**var report = powerbi.embed(reportContainer, config);**

Note that the variable named **models** is initialized using a call to **window[&#39;powerbi-client&#39;].models**. The **models** variable is used to set configuration values such as **models.Permissions.All** , **models.TokenType.Aad** and **models.ViewMode.View**.

A key point is that you need to create a configuration object in order to call the **powerbi.embed** function. You can learn a great deal more about creating the configuration object for Power BI embedding in [this wiki](https://github.com/Microsoft/PowerBI-JavaScript/wiki) for the Power BI JavaScript API.

  1. Add the following JavaScript code to resize the **embed-container** div element whenever the window resize event fires.

**// 4 - add logic to resize embed container on window resize event**

**var heightBuffer = 12;**

**var newHeight = $(window).height() - ($(&quot;header&quot;).height() + heightBuffer);**

**$(&quot;#embed-container&quot;).height(newHeight);**

**$(window).resize(function () {**

**var newHeight = $(window).height() - ($(&quot;header&quot;).height() + heightBuffer);**

**$(&quot;#embed-container&quot;).height(newHeight);**

**});**

  1. Your code in **embed.js** should match the following screenshot.

![](RackMultipart20201204-4-1di9arb_html_836e9a506a8e724f.png)

Remember you can copy and paste all the code at once by using the text in **Exercise 4 - embed.js.txt** in the **StudentLabFiles** folder.

  1. Save your changes and close **embed.js**.
1. Run the web application in the Visual Studio Code debugger to test your work on the **Embed** page.
  1. Start the Visual Studio Code debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.
  2. The **UserOwnsData** web application should display the home page as shown to an anonymous user.
  3. Click on the **Embed** link in the top nav menu to navigate to the **Embed** page and login when prompted.
  4. You should now be able to navigate to the **Embed** page and see the Power BI report displayed on the page.

![](RackMultipart20201204-4-1di9arb_html_edf6ab4db9715346.png)

  1. Try resizing the browser window. The embedded report should continually adapt to the size of the window.

![](RackMultipart20201204-4-1di9arb_html_8d03038d3c193f20.png)

  1. You&#39;re done testing. Close the browser, return to Visual Studio Code and stop the debug session using the debug toolbar.

You have now reached an important milestone. You can now tell all your peers that you have embedded a Power BI report. However, there is more that you can do to improve the developer experience for writing client-side code against the Power BI JavaScript API. In the next exercise, you will add support to your project so that you can program client-side code using TypeScript instead of JavaScript. By moving to TypeScript you can benefit from strongly-typed programming, compile-time type checking and much better IntelliSense.

### Exercise 5: Adding TypeScript Support to a .NET 5 Project

In this exercise, you will add support for developing your client-side code with TypeScript instead of JavaScript. It is assumed that you have already installed Node.js and that the Node Package Manager application named **npm** is available at the commend line. You will begin by adding several Node.js configuration files to the root folder of the **UserOwnsData** project. After that you will restore a set of Node.js packages and use the webpack utility to compile TypeScript code into an output file named **embed.js**.

1. Copy three essential node.js development configuration files into the root folder of the **UserOwnsData** project.
  1. Locate these three files in the **StudentLabFiles** folder.
    1. **package.json** – the standard project file for all Node.js projects.
    2. **tsconfig.json** – a configuration file used by the TypeScript compiler (TSC).
    3. **webpack.config.js** – a configuration file used by the webpack utility.
  2. Copy **package.json** , **tsconfig.json** and **webpack.config.js** into the root folder of the **UserOwnsData** project.

![](RackMultipart20201204-4-1di9arb_html_90c70fbda1e4fb6a.png)

Visual Studio Code makes it difficult to add existing files to a project folder. You can use the Windows Explorer to copy these three files from the **StudentLabFiles** folder to the **UserOwnsData** project folder.

1. Restore the Node.js packages which are referenced in **package.json**.
  1. Open **package.json** and review the Node.js packages referenced in **devDependencies** section.

![](RackMultipart20201204-4-1di9arb_html_1639bd75c7cfb428.png)

  1. Open the Visual Studio Code terminal by clicking the **View \&gt; Terminal** menu command or by using **Ctrl+`** keyboard shortcut.
  2. Run the **npm install** command to restore the list of Node.js packages.

![](RackMultipart20201204-4-1di9arb_html_ece7d4ef23b7a4e0.png)

  1. When you run the **npm install** command, **npm** will download all the Node.js packages into the **node\_modules** folder.

![](RackMultipart20201204-4-1di9arb_html_7a4e308043ec2de2.png)

1. Take a quick look at the **tsconfig.json** file.
  1. Open the **tsconfig.json** file in an editor window and examine the TypeScript compiler settings inside.
  2. When you are done, close **tsconfig.json** without saving any changes.
2. Take a quick look at the **webpack.config.js** file.
  1. Open the **webpack.config.js** file in an editor window and examine its content.

**const path = require(&#39;path&#39;);**

**module.exports = {**

**entry: &#39;./Scripts/embed.ts&#39;,**

**output: {**

**filename: &#39;embed.js&#39;,**

**path: path.resolve(\_\_dirname, &#39;wwwroot/js&#39;),**

**},**

**resolve: {**

**extensions: [&#39;.js&#39;, &#39;.ts&#39;]**

**},**

**module: {**

**rules: [**

**{ test: /\.(ts)$/, loader: &#39;awesome-typescript-loader&#39; }**

**],**

**},**

**mode: &quot;development&quot;,**

**devtool: &#39;source-map&#39;**

**};**

Note the **entry** property of **model.exports** object is set to **./Scripts/embed.ts**. The **path** and **filename** of the **output** object combine to a file path of **wwwroot/js/embed.js**. When the webpack utility runs, it will look for a file named **embed.ts** in the **Scripts** folder as its main entry point for the TypeScript compiler (tsc.exe) and produce an output file in named **embed.js** in the **wwwroot/js** folder.

  1. When you are done, close **webpack.config.js** without saving any changes.
1. Add a new TypeScript source file named **embed.ts**.
  1. In the **UserOwnsData** project folder, create a new top-level folder named **Scripts**.
  2. Create a new file inside the **Scripts** folder named **embed.ts**.

![](RackMultipart20201204-4-1di9arb_html_21c8cea9f121572d.png)

  1. In Windows Explorer, locate the **Exercise 5 - embed.ts.txt** file in the **StudentLabFiles** folder.
  2. Open **Exercise 5 - embed.ts.txt** in a text editor such as Notepad and copy all its contents to the Windows clipboard.
  3. Return to Visual Studio Code and paste the content of **Exercise 5 - embed.ts.txt** into **Embed.ts.**

![](RackMultipart20201204-4-1di9arb_html_db3110829347bb0a.png)

  1. Save your changes and close **embed.ts**.
1. Use the webpack utility to compile **embed.ts** into **embed.js**.
  1. Locate the original **embed.js** file in the **wwwroot/js** folder and delete it.

![](RackMultipart20201204-4-1di9arb_html_4abe6b41213335e0.png)

  1. Open the Visual Studio Code terminal by clicking the **View \&gt; Terminal** menu command or by using **Ctrl+`** keyboard shortcut.
  2. Run the **npm run build** command to run the webpack utility.
  3. When you run **npm run build** , webpack should automatically generate a new version of **embed.js** in the **wwwroot/js** folder.

![](RackMultipart20201204-4-1di9arb_html_92fd7c7f842d1fdb.png)

  1. Open the new version of **embed.js**. You should see it is a source file generated by the webpack utility.

![](RackMultipart20201204-4-1di9arb_html_f8266fed97ca02ed.png)

  1. Close **embed.js** without saving any changes.
1. Update **UserOwnsData.csproj** to add the **npm run build** command as part of the dotnet build process.
  1. Open the .NET 5 project file **UserOwnsData.csproj** in an editor window.

![](RackMultipart20201204-4-1di9arb_html_242c7415dc7d9348.png)

  1. Add a new **Target** element named **PostBuild** to run the **npm run build** command as shown in the following code listing.

**\&lt;Project Sdk=&quot;Microsoft.NET.Sdk.Web&quot;\&gt;**

**\&lt;PropertyGroup\&gt;**

**\&lt;TargetFramework\&gt;netcoreapp3.1\&lt;/TargetFramework\&gt;**

**\&lt;UserSecretsId\&gt;aspnet-UserOwnsData-87660A42-54AC-4CF9-8583-B31608FED004\&lt;/UserSecretsId\&gt;**

**\&lt;WebProject\_DirectoryAccessLevelKey\&gt;0\&lt;/WebProject\_DirectoryAccessLevelKey\&gt;**

**\&lt;/PropertyGroup\&gt;**

**\&lt;ItemGroup\&gt;**

**\&lt;PackageReference Include=&quot;Microsoft.Identity.Web&quot; Version=&quot;0.3.0-preview&quot; /\&gt;**

**\&lt;PackageReference Include=&quot;Microsoft.Identity.Web.UI&quot; Version=&quot;0.3.0-preview&quot; /\&gt;**

**\&lt;PackageReference Include=&quot;Microsoft.PowerBi.Api&quot; Version=&quot;3.14.0&quot; /\&gt;**

**\&lt;/ItemGroup\&gt;**

**\&lt;Target Name=&quot;PostBuild&quot; AfterTargets=&quot;PostBuildEvent&quot;\&gt;**

**\&lt;Exec Command=&quot;npm run build&quot; /\&gt;**

**\&lt;/Target\&gt;**

**\&lt;/Project\&gt;**

  1. Save your changes and close **UserOwnsData.csproj**.
  2. Return to the terminal and run the **dotnet build** command.

![](RackMultipart20201204-4-1di9arb_html_ea2ba1a852076980.png)

  1. When you run the **dotnet build** command, the output window should show you that the webpack command is running.

![](RackMultipart20201204-4-1di9arb_html_e44986bd8aa88677.png)

Now whenever you start a debug session with the **{F5}** key, the TypeScript in **embed.ts** will be automatically compiled into **embed.js**.

1. Run the web application in the Visual Studio Code debugger to test your work on the **Embed** page.
  1. Start the Visual Studio Code debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.
  2. Click on the **Embed** link in the top nav menu to navigate to the **Embed** page and login when prompted.
  3. You should now be able to navigate to the **Embed** page and see the Power BI report displayed on the page.

![](RackMultipart20201204-4-1di9arb_html_edf6ab4db9715346.png)

When you test the **UserOwnsData** web application, it should behave just as it did when you tested it in Exercise 4. The difference is that now the client-side behavior is now implemented with TypeScript instead of JavaScript.

### Exercise 6: Creating a View Model for App Workspaces

Up to this point, you have implemented the **UserOwnsData** project to embed a single report by hard-coding the IDs of that report and its hosting workspace. In this exercise, you will remove the hard-coded IDs and extend the **Embed** page of the **UserOwnsData** project to dynamically discover what workspaces and reports are available to the current user.

1. Extend the **PowerBiServiceApi** class with a new method named **GetEmbeddedViewModel**.
  1. Locate the **PowerBiServiceApi.cs** in the **Service** folder and open it in an editor window.
  2. Add the following method named **GetEmbeddedViewModel** to the end of **PowerBiServiceApi** class.

**public async Task\&lt;string\&gt; GetEmbeddedViewModel(string appWorkspaceId = &quot;&quot;) {**

**var accessToken = this.tokenAcquisition.GetAccessTokenForUserAsync(RequiredScopes).Result;**

**var tokenCredentials = new TokenCredentials (accessToken, &quot;Bearer&quot;);**

**PowerBIClient pbiClient = new PowerBIClient (new Uri (urlPowerBiServiceApiRoot), tokenCredentials);**

**Object viewModel;**

**if (string.IsNullOrEmpty (appWorkspaceId)) {**

**viewModel = new {**

**currentWorkspace = &quot;My Workspace&quot;,**

**workspaces = ( await pbiClient.Groups.GetGroupsAsync() ).Value,**

**datasets = ( await pbiClient.Datasets.GetDatasetsAsync() ).Value,**

**reports = ( await pbiClient.Reports.GetReportsAsync() ).Value,**

**token = accessToken**

**};**

**} else {**

**Guid workspaceId = new Guid (appWorkspaceId);**

**var workspaces = (await pbiClient.Groups.GetGroupsAsync ()).Value;**

**var currentWorkspace = workspaces.First ((workspace) =\&gt; workspace.Id == workspaceId);**

**viewModel = new {**

**workspaces = workspaces,**

**currentWorkspace = currentWorkspace.Name,**

**currentWorkspaceIsReadOnly = currentWorkspace.IsReadOnly,**

**datasets = (await pbiClient.Datasets.GetDatasetsInGroupAsync (workspaceId)).Value,**

**reports = (await pbiClient.Reports.GetReportsInGroupAsync (workspaceId)).Value,**

**token = accessToken**

**};**

**}**

**return JsonConvert.SerializeObject(viewModel);**

**}**

The **GetEmbeddedViewModel** method accepts an **appWorkspaceId** parameter and returns a string value with JSON-formatted data. If the **appWorkspaceId** parameter is blank, the **GetEmbeddedViewModel** method returns a view model for the current user&#39;s personal workspace. If the **appWorkspaceId** parameter contains a GUID, the **GetEmbeddedViewModel** method returns a view model for the app workspace associated with that GUID.

You can copy and paste this method from the **Exercise 6 - PowerBiServiceApi.cs.txt** file in the **StudentLabFiles** folder.

  1. To enhance your conceptual understanding, examine a sample of JSON returned by the **GetEmbeddedViewModel** method.

![](RackMultipart20201204-4-1di9arb_html_2c92de2a4f714f69.png)

  1. Save your work and close **PowerBiServiceApi.cs**.
1. Modify **Embed** method in **HomeController** to call the **GetEmbeddedViewModel** method.
  1. Locate the **HomeController.cs** file and open it in an editor window.
  2. Locate the **Embed** method which should currently match this **Embed** method implementation.

**public async Task\&lt;IActionResult\&gt; Embed() {**

**Guid workspaceId = new Guid(&quot;912f2b34-7daa-4589-83df-35c75944d864&quot;);**

**Guid reportId = new Guid(&quot;cd496c1c-8df0-48e7-8b92-e2932298743e&quot;);**

**var viewModel = await powerBiServiceApi.GetReport(workspaceId, reportId);**

**return View(viewModel);**

**}**

  1. Delete the **Embed** method implementation and replace it the following code.

**public async Task\&lt;IActionResult\&gt; Embed(string workspaceId) {**

**var viewModel = await powerBiServiceApi.GetEmbeddedViewModel(workspaceId);**

**// cast string value to object type in order to pass string value as MVC view model**

**return View(viewModel as object);**

**}**

  1. Save your work and close **HomeController.cs**.

There are a few things to note about the new implementation of the **Embed** controller action method. First, the method now takes a string parameter named **workspaceId**. When this controller method is passed a workspace ID in the **workspaceId** query string parameter, it passes that workspace ID along to the **PowerBiServiceApi** class when it calls the **GetEmbeddedViewModel** method.

The second thing to note about this example if that the string-based **viewModel** variable is cast to a type of **object** in the **return** statement using the syntax **View(viewModel as object)**. This is a required workaround because passing a string parameter to **View()** would fail because the string value would be treated as a view name instead of a view model being passed to the underlying view.

1. Replace the code in **Embed.cshtml** with a better implementation.
  1. Locate **Embed.cshtml** file in the **Views \&gt; Home** folder, open it in an editor window and delete all the content inside.
  2. In Windows Explorer, locate the **Exercise 6 - Embed.cshtml.txt** file in the **StudentLabFiles** folder.
  3. Open **Exercise 6 - Embed.cshtml.txt** in a text editor such as Notepad and copy all its contents to the Windows clipboard.
  4. Return to Visual Studio Code and paste the content of **Exercise 6 - Embed.cshtml.txt** into **Embed.cshtml.**

![](RackMultipart20201204-4-1di9arb_html_e57e09ad7f1c66e5.png)

  1. Save your changes and close **Embed.cshtml.**
1. Replace the code in **Embed.ts** with a better implementation.
  1. Locate **Embed.ts** file in the **Scripts** folder, open it in an editor window and delete all the content inside.
  2. In Windows Explorer, locate the **Exercise 6 - Embed.ts.txt** file in the **StudentLabFiles** folder.
  3. Open **Exercise 6 - Embed.ts.txt** in a text editor such as Notepad and copy all its contents to the Windows clipboard.
  4. Return to Visual Studio Code and paste the content of **Exercise 6 - Embed.ts.txt** into **Embed.ts.**

![](RackMultipart20201204-4-1di9arb_html_7c174b7162d64056.png)

  1. Save your changes and close **Embed.cshtml.**
1. Run the web application in the Visual Studio Code debugger to test your work on the **Embed** page.
  1. Start the Visual Studio Code debugger by selecting **Run \&gt; Start Debugging** or by pressing the **{F5}** keyboard shortcut.
  2. Click on the **Embed** link in the top nav menu to navigate to the **Embed** page and login when prompted.
  3. The **Embed** page should appear much differently than before as shown in the following screenshot.

![](RackMultipart20201204-4-1di9arb_html_acdda6350d90ce0f.png)

Note there is a dropdown list for the **Current Workspace** that you can use to navigate across workspaces.

  1. Navigate to the workspace you created earlier in this lab.

![](RackMultipart20201204-4-1di9arb_html_bcbdcf100cfc0845.png)

  1. Click on a report in the **Open Report** section.

![](RackMultipart20201204-4-1di9arb_html_4ebeb68d6695b7b8.png)

  1. The report should open in read-only mode.

![](RackMultipart20201204-4-1di9arb_html_6afcd68c3d8c06b4.png)

  1. Click the **Toggle Edit Mode** button to move the report into edit mode.

![](RackMultipart20201204-4-1di9arb_html_c23bd68eb16be2c5.png)

  1. Note that when the report goes into edit mode, there isn&#39;t much space to work on the report while editing.

![](RackMultipart20201204-4-1di9arb_html_5c56b7aaf3d52ee6.png)

  1. Click the Full Screen button to enter full screen mode

![](RackMultipart20201204-4-1di9arb_html_a9fcd5c348cc0ca0.png)

You can invoke the **File \&gt; Save** command in a report that is in edit mode to save your changes.

  1. Press the **Esc** key in the keyboard to exit full screen mode.
  2. Click on a second report in the **Open Report** section to navigate between reports.

![](RackMultipart20201204-4-1di9arb_html_e2942691abfd41b8.png)

  1. Create a new report by clicking on a dataset name in the **New Report** section.

![](RackMultipart20201204-4-1di9arb_html_a14514b174aed6f9.png)

  1. Add a simple visual of any type to the new report.

![](RackMultipart20201204-4-1di9arb_html_507920d4b59f361e.png)

  1. Save the new report using the **File \&gt; Save as** menu command.

![](RackMultipart20201204-4-1di9arb_html_ef0b859a365fb917.png)

  1. Give your new report a name.

![](RackMultipart20201204-4-1di9arb_html_6cc7537239a42514.png)

  1. After you click save, the new report should show up in the Open Report section and be displayed in read-only mode.

![](RackMultipart20201204-4-1di9arb_html_7901bd2f4788470f.png)

  1. When you&#39;re done testing, close the browser, return to Visual Studio Code and stop the debug session.

© Power BI Dev Camp. 2020. All Rights Reserved 1