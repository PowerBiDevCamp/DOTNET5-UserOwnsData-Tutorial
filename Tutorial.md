Developing with Power BI Embedding using .NET 5
-----------------------------------------------
In this lab, you will create a new .NET 5 project for a custom web
application and then you will go through the steps required to implement
Power BI embedding. You will use the new Microsoft authentication
library named *Microsoft.Identity.Web* to provide an interactive login
experience and to acquire access tokens which you will need to call the
Power BI Service API. After that, you will write the server-side C\#
code and the client-side JavaScript code to embed a simple Power BI
report on a custom Web page. In the later exercises of this lab, you
will add project-level support for Node.js, TypeScript and webpack so
that you can migrate the client-side code from JavaScript to TypeScript
so that your code receives the benefits of strong typing, IntelliSense
and compile-time type checks.

To complete this lab, your developer workstation must configure to allow
the execution of PowerShell scripts. Your developer workstation must
also have the following software and developer tools installed.

 - **PowerShell cmdlet library for AzureAD** --
\[[download](https://docs.microsoft.com/en-us/powershell/azure/active-directory/install-adv2?view=azureadps-2.0)\]

- **.NET 5 SDK** --
\[[download](https://dotnet.microsoft.com/download/dotnet/5.0)\]

- **Node.js** -- \[[download](https://nodejs.org/en/download/)\]

- **Visual Studio Code** --
\[[download](https://code.visualstudio.com/Download)\]

- **Visual Studio 2019** (optional) --
\[[download](https://visualstudio.microsoft.com/downloads/)\]

Please refer to this [setup
document](https://github.com/PowerBiDevCamp/Camp-Sessions/raw/master/Create%20Power%20BI%20Development%20Environment.pdf)
if you need more detail on how to configure your developer workstation
to work on this tutorial.

### Exercise 1: Create a New Azure AD Application
In this exercise, you will begin by copying the student files into a
local folder on your student workstation. After that, you will use the
.NET 5 CLI to create a new .NET 5 project for an MVC web application
with support for authentication.

1.  Download the student lab files to a local folder on your developer
    workstation.

2.  Create a new top-level folder on your workstation named **DevCamp**
    at a location such as **c:\\DevCamp**.

3.  Download the ZIP archive with the student lab files from GitHub by
    clicking the following link.
```
<https://github.com/PowerBiDevCamp/DOTNET5-UserOwnsData-Tutorial/raw/main/StudentLabFiles.zip>
```
4.  Extract the **StudentLabFiles** folder from **StudentLabFiles.zip**
    into a to a local folder such as **c:\\DevCamp\\StudentLabFiles**.

5.  The **StudentLabFiles** folder should contain the set of files shown
    in the following screenshot.

<img src="/media/image1.png" width="840">

The files in the **StudentLabFiles** folder contain code that you will
be required to write during the exercises of this lab. These files are
provides to make it easier to copy and paste.

6.  Walk through the PowerShell code in
    **Create-AzureAD-Application.ps1** to understand what it does.

7.  Using Windows Explorer, look in the **StudentLabFiles** folder and
    locate the script named **Create-AzureAD-Application.ps1.**

8.  Open **Create-AzureAD-Application.ps1** in a text editor such as
    Notepad or the PowerShell ISE.

9.  The script begins by calling **Connect-AzureAD** to establish a
    connection with Azure AD.
```
$authResult = Connect-AzureAD
```
10. The script contains two variables to set the application name and a
    reply URL of **https://localhost:5001/signin-oidc**.

```
$appDisplayName = User-Owns-Data Sample App"
$replyUrl = "https://localhost:5001/signin-oidc\"
```

> When you register a reply URL with **localhost** with a port number such
as **5001**, Azure AD will allow you to perform testing with reply URLs
that use localhost and any other port number. For example, you can use a
reply URL of **https://localhost:** 5001**/signin-oidc**.

11. The script also contains the code below which creates a new
    **PasswordCredential** object for an app secret.
```PowerShell
# create app secret
$newGuid = New-Guid

$appSecret =
(\[System.Convert\]::ToBase64String(\[System.Text.Encoding\]::UTF8.GetBytes((\$newGuid))))+\"=\"

$startDate = Get-Date

$passwordCredential = New-Object -TypeName
Microsoft.Open.AzureAD.Model.PasswordCredential

$passwordCredential.StartDate = $startDate

$passwordCredential.EndDate = $startDate.AddYears(1)

$passwordCredential.KeyId = $newGuid

$passwordCredential.Value = $appSecret
```
12. Down below, you can see the call to the **New-AzureADApplication**
    cmdlet which creates a new Azure AD application.

\# create Azure AD Application

\$aadApplication = New-AzureADApplication \`

-DisplayName \$appDisplayName \`

-PublicClient \$false \`

-AvailableToOtherTenants \$false \`

-ReplyUrls @(\$replyUrl) \`

-Homepage \$replyUrl \`

-PasswordCredentials \$passwordCredential

Note you must execute this script using Windows PowerShell 5 and not
PowerShell 7 due to incompatibilities between PowerShell7 and the
PowerShell module named **AzureAD**.

13. Execute the PowerShell script named
    **Create-Azure-ADApplication.ps1**

14. Execute the PowerShell script named
    **Create-Azure-ADApplication.ps1**.

15. When prompted for credentials, log in with an Azure AD user account
    in the same tenant where you are using Power BI.

16. When the PowerShell script runs successfully, it will create and
    open a text file named **UserOwnsDataSampleApp.txt**.

![](/media/image2.png){width="2.768640638670166in"
height="2.0269346019247596in"}

You should leave the text file **UserOwnsDataSampleApp.txt** open for
now. This file contains JSON configuration data that you will copy and
paste into the **appsettings.json**.file of the new .NET 5 project you
will create the next exercise.

### Exercise 2: Create a .NET 5 Project for a Secure Web Application

In this exercise, you will use the .NET CLI to create a new .NET 5
project for an MVC web application with support for authentication using
the new Microsoft authentication library named Microsoft.Identity.Web..

1.  Create a new .NET 5 project for an ASP.NET MVC web application.

```{=html}
<!-- -->
```
17. Using Windows Explorer, create a child folder inside the
    **C:\\DevCamp** folder named **UserOwnsData**.

![](/media/image3.png){width="2.6809208223972005in"
height="0.8044652230971129in"}

18. Launch Visual Studio Code.

19. Use the **Open Folder...** command in Visual Studio Code to open the
    **UserOwnsData** folder.

![](/media/image4.png){width="3.5007403762029745in"
height="0.86873687664042in"}

a.  Once you have open the **UserOwnsData** folder, close the
    **Welcome** page.

![](/media/image5.png){width="4.417763560804899in"
height="0.740650699912511in"}

20. Use the Terminal console to verify the current version of .NET

21. Use the **Terminal \> New Terminal** command or the
    \[**Ctrl+Shift+\`**\] keyboard shortcut to open the Terminal
    console.

![](/media/image6.png){width="4.987938538932633in"
height="0.9740332458442694in"}

a.  You should now see a Terminal console with a cursor where you can
    type and execute command-line instructions.

![](/media/image7.png){width="5.102812773403325in"
height="1.9967104111986003in"}

22. Type the following **dotnet** command-line instruction into the
    console and press **Enter** to execute it.

dotnet \--version

a.  When you run the command, the **dotnet** CLI should respond by
    display the .NET version number.

![](/media/image8.png){width="3.2072364391951007in"
height="0.8105282152230971in"}

Make sure you .NET version number is **5.0.100** at a minimum. If you
are working with .NET CORE version 3.1 or early, the project template
files for creating new applications are much different and these lab
instructions will not work as expected. If you do not have .NET 5
installed, you must install the .NET 5 SDK before you can move past this
point.

23. Run .NET CLI commands to create a new ASP.NET MVC project.

24. In the Terminal console, type and execute the following command to
    generate a new .NET console application.

dotnet new mvc \--auth SingleOrg

The **\--auth SingleOrg** parameter instructs the .NET 5 CLI to generate
the new web application with extra code with authentication support
using Microsoft\'s new authentication library named
Microsoft.Identity.Web.

25. After running the **dotnet new** command, you should see new files
    have been added to the project.

![](/media/image9.png){width="4.9032469378827646in"
height="2.014255249343832in"}

1.  Copy the JSON in **UserOwnsDataSampleApp.txt** into the
    **appsettings.json** file in your project.

    a.  Return to the **UserOwnsData** project in Visual Studio Code and
        open the **appsettings.json** file.

    b.  The **appsettings.json** file should initially appear like the
        screenshot below.

![](/media/image10.png){width="4.828990594925634in"
height="2.2423239282589678in"}

26. Delete the contents of **appsettings.json** and replace it by
    copying and pasting the contents of **UserOwnsDataSampleApp.txt**

![](/media/image11.png){width="4.554823928258967in"
height="2.277411417322835in"}

Note the **PowerBi:ServiceRootUrl** parameter has been added as a custom
configuration value to track the base URL to the Power BI Service. When
you are programming against the Power BI Service in Microsoft public
cloud, the URL is <https://api.powerbi.com/>. However, the root URL for
the Power BI Service will be different in other clouds such as the
government cloud. Therefore, this value will be stored as a project
configuration value so it is easy to change whenever required.

27. Configure Visual Studio Code with the extensions needed for C\#
    development with .NET 5.

28. Click on the button at the bottom of the left navigation menu to
    display the **EXTENSION** pane.

29. You should be able to see what extensions are currently installed.

30. You should also be able to search to find new extensions you\'d like
    to install.

![](/media/image12.png){width="4.072691382327209in"
height="2.619517716535433in"}

31. Find and install the **C\#** extension from Microsoft if it is not
    already installed.

![](/media/image13.png){width="6.101865704286964in"
height="1.5290015310586176in"}

32. Find and install the **Debugger for Chrome** extension from
    Microsoft if it is not already installed.

33. You should be able to confirm that the **C\#** extension and the
    **Debugger for Chrome** extensions are now installed.

![](/media/image14.png){width="3.325833333333333in"
height="1.1137270341207348in"}

It is OK if you have other Visual Studio Code extensions installed as
well. It\'s just important that you install these two extensions in
addition to whatever other extensions you may have installed.

34. Generate the project required for building your project and running
    it in the .NET 5 debugger.

35. Open the Visual Studio Code Command Palette by using the
    **Ctrl**+**Shift**+**P** keyboard combination.

36. Type .NET into the Command Palette search box.

37. Select the command titled **.NET: Generate Assets for Build and
    Debug**.

![](/media/image15.png){width="6.321271872265966in"
height="0.817496719160105in"}

38. When the command runs successfully, in generates the files
    **launch.json** and **tasks.json** a new folder named **.vscode**.

![](/media/image16.png){width="6.2925in" height="1.7902744969378828in"}

It\'s not uncommon for the configuration of the C\# extension (i.e.
Omnisharp) in Visual Studio Code to cause errors when running the
command to generate the assets for build and debug. If this is the case,
it can be tricky fix this problem. If the previous step to generate
build and debug assets did not successfully create the **launch.json**
file and the **tasks.json** file in the **.vscode** folder, you can copy
these two essential files from **Troubleshooting/.vscode** folder
located inside the **StudentFiles** folder.

39. Run and test the **UserOwnsData** web application using Visual
    Studio Code and the .NET 5 debugger.

40. Start the .NET 5 debugger by selecting **Run \> Start Debugging** or
    by pressing the **{F5}** keyboard shortcut.

![](/media/image17.png){width="4.170838801399825in"
height="1.3458333333333334in"}

The UserOwnsData web application is currently configured authenticate
the user before the user is able to view the home page. Therefore, you
will be prompted to log in as soon as you launch the application in the
.NET debugger.

41. When prompted to Sign in, log in using your organizational account.

![](/media/image18.png){width="1.6301629483814524in" height="1.1325in"}

42. Once you have signed in, you will be prompted to by the
    **Permissions requested** dialog to grant consent to the
    application.

43. Click the **Accept** button to continue.

![](/media/image19.png){width="1.7791666666666666in"
height="2.1166622922134732in"}

44. You should now see the home page for the **UserOwnsData** web
    application which should match the following screenshot.

![](/media/image20.png){width="5.071428258967629in"
height="2.158643919510061in"}

45. You\'re done testing. Close the browser, return to Visual Studio
    Code and stop the debug session using the debug toolbar.

![](/media/image21.png){width="2.4907972440944883in"
height="0.6692639982502188in"}

You now got to the point where the **UserOwnsData** web application is
up and running and it can successfully authenticate the user. Over the
next few steps you will add some custom HTML and CSS styles to make the
application a bit more stylish. You will also configure the home page so
it does not require authentication to improve the user login experience.

46. Copy and paste a set of pre-written CSS styles into the
    **UserOwnsData** project\'s **site.css** file.

47. Expand the **wwwroot** folder and then expand the **css** folder
    inside to examine the contents of the **wwwroot/css** folder.

48. Open the CSS file named **site.css** and delete any existing content
    inside.

![](/media/image22.png){width="1.9725in" height="2.1143022747156603in"}

49. Using the Windows Explorer, look inside the **StudentLabFiles**
    folder and locate the file named **Exercise 2 - site.css.txt**.

50. Open **Exercise 2 - site.css.txt** up in a text editor and copy all
    of its contents into the Windows clipboard.

51. Return to Visual Studio Code and paste the contents of **Exercise
    2 - site.css.txt** into **sites.css**.

![](/media/image23.png){width="6.758397856517935in"
height="2.3858333333333333in"}

52. Save your changes and close **site.css**.

53. Copy a custom **favicon.ico** file to the **wwwroot** folder.

54. Using the Windows Explorer, look inside the **StudentLabFiles**
    folder and locate the file named **favicon.ico**.

55. Copy the **favicon.ico** file into the **wwwroot** folder of your
    project.

![](/media/image24.png){width="2.2968733595800526in" height="1.3125in"}

Any file you add the **wwwroot** folder will appear at the root folder
of the website created by the **UserOwnsData** project. By adding the
**favicon.ico** file, this web application will now display a custom
**favicon.ico** in the browser page tab.

56. Modify the partial razor view file named **\_LoginPartial.cshtml**
    to integrate with the **Microsoft.Identity.Web** authentication
    library..

57. Expand the **Views \> Shared** folder and locate the partial view
    named **\_LoginPartial.cshtml**.

58. Open **\_LoginPartial.cshtml** in an editor window.

59. In the existing code, locate the code **Hello
    \@User.Identity.Name!** which is used to display information about
    the user.

![](/media/image25.png){width="3.6466666666666665in"
height="1.0869028871391075in"}

60. Replace **Hello \@User.Identity.Name!** with **Hello
    \@User.FindFirst(\"name\").Value** as shown in the following
    screenshot.

![](/media/image26.png){width="4.379166666666666in"
height="0.3981058617672791in"}

With this update, the application will display the user\'s display name
in the welcome message instead of using the user principal name. In
other words, the user greeting will now display a message like **Hello
Betty White** instead of **Hello BettyW\@Contoso.com!**.

61. Save your changes and close **\_LoginPartial.cshtml**.

62. Modify the HTML in **Index.cshtml** to display differently depending
    on whether the user has logged in or not.

63. Expand the **Views \> Home** folder and locate the view file named
    **Index.cshtml**.

64. Open **Index.cshtml** in an editor window.

65. Delete the contents of **Index.cshtml** and replace it with the code
    shown in the following code listing.

\@using System.Security.Principal

\@if (User.Identity.IsAuthenticated) {

\<div class=\"jumbotron\"\>

\<h2\>Welcome \@User.FindFirst(\"name\").Value\</h2\>

\<p\>You have now logged into this application.\</p\>

\</div\>

}

else {

\<div class=\"jumbotron\"\>

\<h2\>Welcome to the User-Owns-Data Tutorial\</h2\>

\<p\>Click the \<strong\>sign in\</strong\> link in the upper right to
get started\</p\>

\</div\>

}

66. Once you have copied the code from above, save your changes and
    close **Index.cshtml**.

![](/media/image27.png){width="4.2125in" height="1.4408683289588802in"}

When you create a new .NET 5 project which supports authentication, the
underlying project template creates a home page that requires
authentication. To support a more natural login experience for the user,
it often makes sense to configure your application so that an anonymous
user can access the home page. In the next step you will modify the
**Home** controller so the home page is accessible to the anonymous
user.

67. Modify the **Index** action method in **HomeController.cs** to
    support anonymous access.

68. Inside the **Controllers** folder, locate **HomeControllers.cs** and
    open this file in an editor window.

69. Locate the **Index** method inside the **HomeController** class.

![](/media/image28.png){width="3.819166666666667in"
height="2.0563943569553804in"}

70. Add the **\[AllowAnonymous\]** attribute to the **Index** method as
    shown in the following code listing.

\[AllowAnonymous\]

public IActionResult Index()

{

return View();

}

71. Save your changes and close **HomeController.cs**.

You have now modified the project to the point where you can run the web
application in the .NET 5 debugger. In the next step, you will start the
debugger so you can test your web application as it runs in the browser.

72. Test the **UserOwnsData** project by running it in the .NET 5
    debugging environment.

73. Start the .NET 5 debugger by selecting **Run \> Start Debugging** or
    by pressing the **{F5}** keyboard shortcut.

74. Once the debugging session has initialized, the browser should
    display the home page using anonymous access.

75. Click the **Sign in** link to test put the user experience when
    authenticating with Azure AD.

![](/media/image29.png){width="3.9591666666666665in"
height="1.0966721347331583in"}

76. Once you have signed in, you should be able to see the text on the
    home page changes because the user is authenticated.

![](/media/image30.png){width="4.3525in" height="1.1068689851268592in"}

At this point, the user should be authenticated. For example, you should
see the logged in user name to the left of the **Sign out** link in the
top right corner. You should also see that the home page displays text
that welcomes the user by name.

If the web page does not appear with a yellow background as shown in the
screenshot above, it\'s possible your browser has cached the original
version of the **site.css** file. If this is the case, you must clear
the browser cache so it loads the latest version of **site.css**.

77. Test the user experience for logging out.

78. Click the **Sign out** link to begin the logout experience.

![](/media/image31.png){width="6.458834208223972in"
height="0.5591666666666667in"}

79. After logging out, you\'ll be directed to the
    **Microsoft.Identity.Web** logout page at
    **/MicrosoftIdentity/Account/SignedOut**.

![](/media/image32.png){width="4.539576771653543in" height="0.8125in"}

80. You\'re done testing. Close the browser, return to Visual Studio
    Code and stop the debug session using the debug toolbar.

In the next step, you will add a new controller action and view named
**Embed**. However, instead of creating a new controller action and
view, you will simply the rename the controller action and view named
**Privacy** that were automatically added by the project template.

81. Create a new controller action named **Embed**.

82. Locate the **HomeController.cs** file in the **Controllers** folder
    and open it in an editor window.

83. Look inside the **HomeController** class and locate the method named
    **Privacy**.

\[AllowAnonymous\]

public IActionResult Index() {

return View();

}

public IActionResult Privacy() {

return View();

}

84. Rename of the **Privacy** method to **Embed**. No changes to the
    method body are required.

\[AllowAnonymous\]

public IActionResult Index() {

return View();

}

public IActionResult Embed() {

return View();

}

Note that, unlike the **Index** method, the **Embed** method does not
have the **AllowAnonymous** attribute. That means only authenticated
users will be able to navigate to this page. One really nice aspect of
the ASP.NET MVC architecture is that it will automatically trigger an
interactive login whenever an anonymous user attempts to navigate to a
secured page such as **Embed**.

85. Create a new MVC view for the **Home** controller named
    **Embed.cshtml**.

86. Look inside the **Views \> Home** folder and locate the razor view
    file named **Privacy.cshtml**.

![](/media/image33.png){width="2.046038932633421in" height="1.4525in"}

87. Rename the **Privacy.cshtml** razor file to **Embed.cshtml**..

![](/media/image34.png){width="1.768928258967629in" height="0.9525in"}

88. Open **Embed.cshtml** in a code editor.

89. Delete the existing contents of **Embed.cshtml** and replace it with
    the follow line of HTML code.

\<h2\>TODO: Embed Report Here\</h2\>

90. Save your changes and close **Embed.cshtml**.

In a standard .NET 5 web application that uses MVC, there is a shared
page layout defined in a file named **\_Layouts.cshtml** which is
located in the **Views \> Shared** folder. In the next step you will
modify the shared layout in the **\_Layouts.cshtml** file so that you
can add a link to the **Embed** page into the top navigation menu.

91. Modify the shared layout in **\_Layout.cshtml** to include a link to
    the **Embed** page.

92. Inside the **Views \> Shared** folder, locate **\_Layouts.cshtml**
    and open this shared view file in an editor window.

93. Using Windows Explorer, look inside the **StudentLabFiles** folder
    and locate the file named **\_Layout.cshtml**.

94. Open **Exercise 2 - \_Layout.cshtml.txt** in the **StudentLabFiles**
    folder copy its contents to the Windows clipboard.

95. Return to Visual Studio Code and paste the contents of the Windows
    clipboard into the **\_Layouts.cshtml** file.

![](/media/image35.png){width="6.571130796150481in" height="2.6125in"}

96. Save your changes and close **\_Layouts.cshtml**

97. Run the web application in the Visual Studio Code debugger to test
    the new **Embed** page.

98. Start the Visual Studio Code debugger by selecting **Run \> Start
    Debugging** or by pressing the **{F5}** keyboard shortcut.

99. The **UserOwnsData** web application should display the home page as
    shown to an anonymous user.

100. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page.

![](/media/image36.png){width="5.01084864391951in"
height="1.0258333333333334in"}

101. When you attempt to navigate to the **Embed** page as an anonymous
     user, you\'ll be prompted to pick an account and log in.

102. Log in using your user name and password.

![](/media/image37.png){width="1.4904101049868765in" height="1.16in"}

103. Once you have logged in, you should be automatically redirected to
     the **Embed** page.

![](/media/image38.png){width="4.842731846019247in"
height="0.8380621172353456in"}

104. You\'re done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

The next step is an *[optional step]{.ul}* for those campers that prefer
developing with Visual Studio 2019 instead of Visual Studio Code.\
If you are happy developing with Visual Studio Code and are not
interested in developing .NET 5 projects using Visual Studio 2019, you
can skip the next step and move ahead to *Exercise 3: Call the Power BI
Service API*.

105. Open and test the **UserOwnsData** project using Visual
     Studio 2019.

106. Launch Visual Studio 2019 -- You can use any edition including the
     Enterprise edition, Pro edition or Community edition.

107. From the **File** menu, select the **Open \> Project/Solution...**
     command.

![](/media/image39.png){width="4.491361548556431in"
height="0.7722648731408573in"}

108. In the **Open Project/Solution** dialog, select the
     **UserOwnsData.csproj** file in the **UserOwnsData** folder and
     click **Open**.

109. The **UserOwnsData** project should now be open in Visual Studio
     2019 as shown in the following screenshot.

![](/media/image40.png){width="2.9066666666666667in"
height="2.77251312335958in"}

There is one big difference between developing with Visual Studio Code
and Visual Studio 2019. Visual Studio Code only requires project files
(\*.csproj). However, Visual Studio 2019 requires that you work with
both project files and solution files (\*.sln). In the next step you
will save a new project file for the **UserOwnsData** solution to make
it easier to develop this project with Visual Studio 2019.

110. In the **Solution Explorer** on the right, select the top node in
     the tree with the caption **Solution \"UserOwnsData\"**.

111. From the **File** menu, select the **Save UserOwnsData.sln** menu
     command.

![](/media/image41.png){width="3.4525in" height="1.1167027559055118in"}

112. Save the solution file **UserOwnsData.sln** in the **UserOwnsData**
     project folder

![](/media/image42.png){width="2.14in" height="1.1468810148731408in"}

Remember that the **UserOwnsData.sln** file is only used by Visual
Studio 2019 and it not used at all in Visual Studio Code.

### Exercise 3: Call the Power BI Service API

In this exercise, you will begin by ensuring you have a Power BI app
workspace and a report for testing. After that, you will add support to
the **UserOwnsData** web application to acquire access tokens from Azure
AD and to call the Power BI Service API. By the end of this exercise,
your code will be able to call to the Power BI Service API to retrieve
data about a report required for embedding.

1.  Using the browser, log into the Power BI Service with the same user
    account you used to create the Azure AD application earlier.

```{=html}
<!-- -->
```
113. Navigate the Power BI portal at <https://app.powerbi.com> and if
     prompted, log in using your credentials.

```{=html}
<!-- -->
```
1.  Create a new app workspace named **Dev Camp Demos**.

    a.  Click the **Workspace** flyout menu in the left navigation.

![](/media/image43.png){width="1.0157195975503062in"
height="1.3458333333333334in"}

b.  Click the **Create app workspace** button to display the **Create an
    > app workspace** dialog.

![](/media/image44.png){width="1.6328652668416448in"
height="0.9791666666666666in"}

c.  In the **Create an app workspace** pane, enter a workspace name such
    > as **Dev Camp Demos**.

d.  Click the **Save** button to create the new app workspace.

![](/media/image45.png){width="2.3291983814523185in"
height="3.0981594488188975in"}

e.  When you click **Save**, the Power BI service should create the new
    > app workspace and then switch your current Power BI session to be
    > running within the context of the new **Dev Camp Demos**
    > workspace.

![](/media/image46.png){width="3.1477055993000875in"
height="1.6787751531058617in"}

Now that you have created the new app workspace, the next step is to
upload a PBIX project file created with Power BI Desktop. You are free
to use your own PBIX file as long as the PBIX file does not have
row-level security (RLS) enabled. If you don\'t have your own PBIX file,
you can download the sample PBIX file named [**COVID-19
US.pbix**](https://github.com/PowerBiDevCamp/pbix-samples/raw/main/COVID-19%20US.pbix)
and use that instead.

114. Upload a PBIX file to create a new report and dataset.

115. Click **Add content** to navigate to the **Get Data** page.

116. Click the **Get** button in the **Files** section.

![](/media/image47.png){width="3.878043525809274in"
height="1.6167191601049868in"}

117. Click on **Local File** in order to select a PBIX file that you
     have on your local hard drive.

![](/media/image48.png){width="2.6725in" height="0.8265540244969379in"}

118. Select the PBIX file and click the **Open** button to upload it to
     the Power BI Service.

![](/media/image49.png){width="2.5525in" height="1.137988845144357in"}

119. The Power BI Service should have created a report and a dashboard
     from the PBIX file you uploaded.

120. If the Power BI Service created a dashboard as well, delete this
     dashboard as you will not need it.

![](/media/image50.png){width="2.8725in" height="1.239857830271216in"}

121. Open the report to see what it looks like when displayed in the
     Power BI Service.

122. Click on the report to open it.

![](/media/image51.png){width="3.8278510498687663in"
height="1.188063210848644in"}

123. You should now be able to see the report.

![](/media/image52.png){width="5.039166666666667in"
height="1.6042629046369203in"}

In the next step, you will find and record the GUID-based IDs for the
report and its hosting workspace. You will then use these IDs later in
this exercises when you first write the code to embed a report in the
**UserOwnsData** web application.

124. Get the Workspace ID and the Report ID from the report URL.

125. Locate and copy the app workspace ID from the report URL by copying
     the GUID that comes after **/groups/**.

![](/media/image53.png){width="3.6791666666666667in"
height="1.0382305336832895in"}

126. Open up a new text file in a program such as Notepad and paste in
     the value of the workspace ID.

127. Locate and copy the report ID from the URL by copying the GUID that
     comes after **/reports/**.

![](/media/image54.png){width="3.8449890638670166in"
height="0.8991666666666667in"}

128. Copy the report ID into the text file Notepad.

![](/media/image55.png){width="2.745833333333333in"
height="0.568423009623797in"}

Leave the text file open for now. In a step later in this exercise, you
will copy and paste these IDs into your C\# code.

129. Add the NuGet package for the **Power BI .NET SDK**.

     a.  Move back to the Terminal so you can execute another dotnet CLI
         command.

     b.  Type and execute the following **dotnet add package** command
         to add the NuGet package for the **Power BI .NET SDK**.

dotnet add package Microsoft.PowerBi.Api

![](/media/image56.png){width="4.839166666666666in"
height="0.9972637795275591in"}

130. Open the **UserOwnsData.csproj** file. You should now see this file
     contains a package reference to **Microsoft.PowerBi.Api**.

![](/media/image57.png){width="4.318862642169729in"
height="1.2391666666666667in"}

131. Close the the **UserOwnsData.csproj** file without saving any
     changes.

Your project now includes the NuGet package for the Power BI .NET SDK so
you can begin to program against the classes from this package in the
**Microsoft.PowerBI.Api** namespace.

132. Create a new C\# class named **PowerBiServiceApi** in which you
     will add code for calling the Power BI Service API.

133. Return to the **UserOwnsData** project in Visual Studio Code.

134. Create a new top-level folder in the **UserOwnsData** project named
     **Services**.

![](/media/image58.png){width="1.2991666666666666in"
height="1.214035433070866in"}

135. Inside the **Services** folder, create a new C\# source file named
     **PowerBiServiceApi.cs**.

![](/media/image59.png){width="1.3658573928258968in"
height="0.7191666666666666in"}

136. Copy and paste the following code into **PowerBiServiceApi.cs** to
     provide a starting point.

using System;

using System.Linq;

using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Microsoft.Identity.Web;

using Microsoft.Rest;

using Microsoft.PowerBI.Api;

using Newtonsoft.Json;

namespace UserOwnsData.Services {

public class EmbeddedReportViewModel {

//TODO: implement this class

}

public class PowerBiServiceApi {

//TODO: implement this class

}

}

137. Implement the **EmbeddedReportViewModel** class using the following
     code.

public class EmbeddedReportViewModel {

public string Id;

public string Name;

public string EmbedUrl;

public string Token;

}

The **EmbeddedReportViewModel** class is designed as a view model class
to pass the data needed to embed a single report. You will use this
class later in this lab to pass report embedding data from an MVC
controller to an MVC view.

138. Begin your implementation by adding two private fields named
     **tokenAcquisition** and **urlPowerBiServiceApiRoot**.

public class PowerBiServiceApi {

private ITokenAcquisition tokenAcquisition { get; }

private string urlPowerBiServiceApiRoot { get; }

}

139. Add the following constructor to initialize the two private fields
     named **tokenAcquisition** and **urlPowerBiServiceApiRoot**.

public class PowerBiServiceApi {

private ITokenAcquisition tokenAcquisition { get; }

private string urlPowerBiServiceApiRoot { get; }

public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition
tokenAcquisition) {

this.urlPowerBiServiceApiRoot =
configuration\[\"PowerBi:ServiceRootUrl\"\];

this.tokenAcquisition = tokenAcquisition;

}

}

This code uses the .NET dependency injection pattern. When your class
needs to use a service, you can simply add a constructor parameter for
that service and the .NET runtime takes care of passing the service
instance at run time. In this case, the constructor is injecting an
instance of the .NET configuration service using the **IConfiguration**
parameter which is used to retrieve the **PowerBi:ServiceRootUrl**
configuration value from **appsettings.json**. The **ITokenAcquisition**
parameter which is named **tokenAcquisition** holds a reference to the
Microsoft authentication service provided by the
**Microsoft.Identity.Web** library and will be used to acquire access
tokens from Azure AD.

140. Place your cursor at the bottom of the **PowerBiServiceApi** class
     and add another new line so you can add more members.

141. At the bottom off the **PowerBiServiceApi** class, add the
     following static read-only field named **RequiredScopes**.

public static readonly string\[\] RequiredScopes = new string\[\] {

\"https://analysis.windows.net/powerbi/api/Group.Read.All\",

\"https://analysis.windows.net/powerbi/api/Report.ReadWrite.All\",

\"https://analysis.windows.net/powerbi/api/Dataset.ReadWrite.All\",

\"https://analysis.windows.net/powerbi/api/Content.Create\",

\"https://analysis.windows.net/powerbi/api/Workspace.ReadWrite.All\"

};

The **RequiredScopes** field is a string array with a set of delegated
permissions supported by the Power BI Service API. Your application will
pass these permissions when it calls to Azure AD to acquire an access
token.

142. Move down in the **PowerBiServiceApi** class below the
     **RequiredScopes** field and add the **GetAccessToken** method.

public string GetAccessToken() {

return
this.tokenAcquisition.GetAccessTokenForUserAsync(RequiredScopes).Result;

}

143. Move down below the **GetAccessToken** method and add the
     **GetPowerBiClient** method.

public PowerBIClient GetPowerBiClient() {

var tokenCredentials = new TokenCredentials(GetAccessToken(),
\"Bearer\");

return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot),
tokenCredentials);

}

144. Move down below the **GetPowerBiClient** method and add the
     **GetReport** method.

public async Task\<EmbeddedReportViewModel\> GetReport(Guid WorkspaceId,
Guid ReportId) {

PowerBIClient pbiClient = GetPowerBiClient();

// call to Power BI Service API to get embedding data

var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId,
ReportId);

// return report embedding data to caller

return new EmbeddedReportViewModel {

Id = report.Id.ToString(),

EmbedUrl = report.EmbedUrl,

Name = report.Name,

Token = GetAccessToken()

};

}

145. Save and close **PowerBIServiceApi.cs**.

Note that **Exercise 3 - PowerBiServiceApi.cs.txt** in the
**StudentLabFiles** folder contains the final code for
**PowerBiServiceApi.cs**.

146. Modify the code in **Startup.cs** to properly register the services
     required for user authentication and access token acquisition.

147. Open the **Startup.cs** file in an editor window.

148. Underneath the existing **using** statements, add the following
     **using** statement;

using UserOwnsData.Services;

149. Look inside the **ConfigureServices** method and locate the
     following line of code.

public void ConfigureServices(IServiceCollection services) {

services.AddMicrosoftIdentityWebAppAuthentication(Configuration);

150. Replace the call to **services.**
     **AddMicrosoftIdentityWebAppAuthentication** with the following
     code.

services

.AddMicrosoftIdentityWebAppAuthentication(Configuration)

.EnableTokenAcquisitionToCallDownstreamApi(PowerBiServiceApi.RequiredScopes)

.AddInMemoryTokenCaches();

151. Move below the call to **AddInMemoryTokenCaches** and add the
     following code.

services.AddScoped(typeof(PowerBiServiceApi));

152. At this point, the **ConfigureService** method in **Startup.cs**
     should match the following code listing.

public void ConfigureServices(IServiceCollection services) {

services

.AddMicrosoftIdentityWebAppAuthentication(Configuration)

.EnableTokenAcquisitionToCallDownstreamApi(PowerBiServiceApi.RequiredScopes)

.AddInMemoryTokenCaches();

services.AddScoped(typeof(PowerBiServiceApi));

var mvcBuilder = services.AddControllersWithViews(options =\> {

var policy = new AuthorizationPolicyBuilder()

.RequireAuthenticatedUser()

.Build();

options.Filters.Add(new AuthorizeFilter(policy));

});

mvcBuilder.AddMicrosoftIdentityUI();

services.AddRazorPages();

}

The code in **ConfigureServices** accomplishes several important things.
The call to **AddMicrosoftWebAppCallsWebApi** configures the Microsoft
authentication library to acquire access tokens. Next, the call to
**AddInMemoryTokenCaches** configures a token cache that the Microsoft
authentication library will use to cache access tokens and refresh
tokens behind the scenes. Finally, the call to
**services.AddScoped(typeof(PowerBiServiceApi))** configures the
**PowerBiServiceApi** class as a service class that can be added to
other classes using dependency injection.

153. Modify the **HomeController** class to program against the
     **PowerBiServiceApi** class.

154. Inside the **Controllers** folder, locate **HomeController.cs** and
     open it in an editor window.

155. Underneath the existing **using** statements, add a **using**
     statement to import the **UserOwnsData.Services** namespace.

using UserOwnsData.Services;

156. At the top of the **HomeController** class locate the **\_logger**
     field and the constructor as shown in the following code listing.

\[Authorize\]

public class HomeController : Controller {

private readonly ILogger\<HomeController\> \_logger;

public HomeController(ILogger\<HomeController\> logger) {

\_logger = logger;

}

157. Remove the **\_logger** field and the existing constructor and
     replace them with the following code.

\[Authorize\]

public class HomeController : Controller {

private PowerBiServiceApi powerBiServiceApi;

public HomeController(PowerBiServiceApi powerBiServiceApi) {

this.powerBiServiceApi = powerBiServiceApi;

}

This is another example of using dependency injection. Since you
registered the **PowerBiServiceApi** class as a service by calling
**services.AddScoped** in the **ConfigureServices** method, you can
simply add a **PowerBiServiceApi** parameter to the constructor and the
.NET 5 runtime will take care of creating a **PowerBiServiceApi**
instance and passing it to the constructor.

158. Locate the **Embed** method implementation in the
     **HomeController** class and replace it with the following code.

public async Task\<IActionResult\> Embed() {

Guid workspaceId = new Guid(\"912f2b34-7daa-4589-83df-35c75944d864\");

Guid reportId = new Guid(\"cd496c1c-8df0-48e7-8b92-e2932298743e\");

var viewModel = await powerBiServiceApi.GetReport(workspaceId,
reportId);

return View(viewModel);

}

159. Modify the HTML and razor code in the view file named
     **Embed.cshtml**.

160. Locate the **Embed.cshtml** razor file inside the **Views \> Home**
     folder and open this file in an editor window.

161. Delete the contents of **Embed.cshtml** and replace it with the
     following code which creates a table to display report data.

\@model UserOwnsData.Services.EmbeddedReportViewModel;

\<style\>

table td {

min-width: 120px;

word-break: break-all;

overflow-wrap: break-word;

font-size: 0.8em;

}

\</style\>

\<h3\>Report View Model Data\</h3\>

\<table class=\"table table-bordered table-striped table-sm\" \>

\<tr\>\<td\>Report Name\</td\>\<td\>\@Model.Name\</td\>\</tr\>

\<tr\>\<td\>Report ID\</td\>\<td\>\@Model.Id\</td\>\</tr\>

\<tr\>\<td\>Embed Url\</td\>\<td\>\@Model.EmbedUrl\</td\>\</tr\>

\<tr\>\<td\>Token\</td\>\<td\>\@Model.Token\</td\>\</tr\>

\</table\>

162. The code in **Embed.cshtml** should now match the following
     screenshot..

![](/media/image60.png){width="3.345833333333333in"
height="1.6792771216097988in"}

163. Save your changes and close **Embed.cshtml**.

164. Run the web application in the Visual Studio Code debugger to test
     the new **Embed** page.

165. Start the Visual Studio Code debugger by selecting **Run \> Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

166. The **UserOwnsData** web application should display the home page
     as shown to an anonymous user.

167. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page.

![](/media/image61.png){width="3.839166666666667in"
height="0.652020997375328in"}

168. If you are prompted to enter your credentials, enter your user name
     and password and log in.

169. After you have authenticated for the first time, you should be
     prompted with a **Permissions Requested** dialog.

170. Click the **Accept** button to consent to the application using the
     requested delegated permissions on your behalf.

![](/media/image62.png){width="1.0871412948381451in"
height="1.6458333333333333in"}

The **Permissions requested** dialog is only shown to each user during
the first successful login. Once a user clicks **Accept**, Azure AD
remembers that the user has consented to the required permissions and
does not need to prompt the user about permission requests.

171. Once you navigate to the **Embed** page, it should display a table
     containing the embedding data for your Power BI report.

![](/media/image63.png){width="5.937952755905512in"
height="3.2191666666666667in"}

172. You\'re done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

You are now half way in your development efforts to embed a Power BI
report. You have written the code to call the Power BI Service API and
retrieve the data required to embed a report. In the next exercise, you
will complete the Power BI embedding implementation by adding
client-side JavaScript code which programs against the Power BI
JavaScript API.

### Exercise 4: Embedding a Report using powerbi.js

In this exercise, you will modify the view named **Embed.cshtml** to
embed a Power BI report on a web page. Your work will involve adding a
new a JavaScript file named **embed.js** in which you will write the
minimal client-side code required to embed a report.

1.  Modify the razor view file named **Embed.cshtml**.

```{=html}
<!-- -->
```
173. Inside the **Views \> Home** folder, locate and open
     **Embed.cshtml** in an editor window.

174. Replace the contents of **Embed.cshtml** with the following code.

\@model UserOwnsData.Services.EmbeddedReportViewModel;

\<div id=\"embed-container\" style=\"height:800px;\"\>\</div\>

\@section Scripts {

}

Note that the div element with the ID of **embed-container** will be
used as the embed container.

Over the next few steps, you will add three **script** tags into the
**Scrips** section. The benefit of adding script tags into the
**Scripts** section is that they will load after the JavaScript
libraries such as jquery which are loaded from the shared view
**\_Layout.cshtml**.

175. Place your cursor inside the **Scripts** section and paste in the
     following **script** tag to import **powerbi.min.js** from a CDN.

\<script
src=\"https://cdn.jsdelivr.net/npm/powerbi-client\@2.13.3/dist/powerbi.min.js\"\>\</script\>

**powerbi.min.js** is the JavaScript file that loads the client-side
library named the **Power BI JavaScript API**.

176. Underneath the **script** tag for the Power BI JavaScript API, add
     a second **script** tag using the following code.

\<script\>

var viewModel = {

reportId: \"\@Model.Id\",

embedUrl: \"\@Model.EmbedUrl\",

token: \"\@Model.Token\"

};

\</script\>

This **script** tag is creates a JavaScript object named **viewModel**
which is accessible to the JavaScript code you\'ll write later in this
lab.

177. Underneath the other two **script** tags, add a third **script**
     tag to load a custom JavaScript file named **embed.js**.

\<script src=\"\~/js/embed.js\"\>\</script\>

Note that the JavaScript file named **embed.js** does not exist yet. You
will create the **embed.js** file in the next step.

178. When you are done, the contents you have in **Embed.cshtml** should
     match the following code listing.

\@model UserOwnsData.Services.EmbeddedReportViewModel;

\<div id=\"embed-container\" style=\"height:800px;\"\>\</div\>

\@section Scripts {

\<script
src=\"https://cdn.jsdelivr.net/npm/powerbi-client\@2.13.3/dist/powerbi.min.js\"\>\</script\>

\<script\>

var viewModel = {

reportId: \"\@Model.Id\",

embedUrl: \"\@Model.EmbedUrl\",

token: \"\@Model.Token\"

};

\</script\>

\<script src=\"\~/js/embed.js\"\>\</script\>

}

179. Save your changes and close **Embed.cshtml**.

The final step is to add a new JavaScript file named **embed.js** with
the code required to embed a report.

180. Add a new JavaScript file named **embed.js**.

181. Locate the top-level folder named **wwwroot** and expand it.

182. Locate the **js** folder inside the **wwwroot** folder and expand
     that.

183. Currently, there should be one file inside the **wwwroot \> js**
     folder named **site.js**.

![](/media/image64.png){width="2.277283464566929in"
height="1.0858333333333334in"}

184. Rename **site.js** to **embed.js**.

![](/media/image65.png){width="2.247969160104987in"
height="1.3191666666666666in"}

185. Add the JavaScript code to **embed.js** to embed a report.

186. Open **embed.js** in an editor window.

187. Delete whatever content exists inside **embed.js**.

188. Paste the following code into **embed.js** to provide a starting
     point.

\$(function(){

// 1 - get DOM object for div that is report container

// 2 - get report embedding data from view model

// 3 - embed report using the Power BI JavaScript API.

// 4 - add logic to resize embed container on window resize event

});

You will now copy and paste four sections of JavaScript code into
**embed.js** to complete the implementation. Note that you can copy and
paste all the code at once from copying the contents of **Exercise 4 -
embed.js.txt** in the **StudentLabFiles** folder.

189. Add the following JavaScript code to create a variable named
     **reportContainer** which holds a reference to **embed-container**.

// 1 - get DOM object for div that is report container

var reportContainer = document.getElementById(\"embed-container\");

190. Add code to create 3 variables named **reportId**, **embedUrl** and
     **token** which are initialized from the global **viewModel**
     object.

// 2 - get report embedding data from view model

var reportId = window.viewModel.reportId;

var embedUrl = window.viewModel.embedUrl;

var token = window.viewModel.token

Now this JavaScript code has retrieved the three essential pieces of
data from **window.viewModel** to embed a Power BI report.

191. Add the following code to embed a report by calling the
     **powerbi.embed** function provided by the Power BI JavaScript API.

// 3 - embed report using the Power BI JavaScript API.

var models = window\[\'powerbi-client\'\].models;

var config = {

type: \'report\',

id: reportId,

embedUrl: embedUrl,

accessToken: token,

permissions: models.Permissions.All,

tokenType: models.TokenType.Aad,

viewMode: models.ViewMode.View,

settings: {

panes: {

filters: { expanded: false, visible: true },

pageNavigation: { visible: false }

}

}

};

// Embed the report and display it within the div container.

var report = powerbi.embed(reportContainer, config);

Note that the variable named **models** is initialized using a call to
**window\[\'powerbi-client\'\].models**. The **models** variable is used
to set configuration values such as **models.Permissions.All**,
**models.TokenType.Aad** and **models.ViewMode.View**.

A key point is that you need to create a configuration object in order
to call the **powerbi.embed** function. You can learn a great deal more
about creating the configuration object for Power BI embedding in [this
wiki](https://github.com/Microsoft/PowerBI-JavaScript/wiki) for the
Power BI JavaScript API.

192. Add the following JavaScript code to resize the **embed-container**
     div element whenever the window resize event fires.

// 4 - add logic to resize embed container on window resize event

var heightBuffer = 12;

var newHeight = \$(window).height() - (\$(\"header\").height() +
heightBuffer);

\$(\"\#embed-container\").height(newHeight);

\$(window).resize(function () {

var newHeight = \$(window).height() - (\$(\"header\").height() +
heightBuffer);

\$(\"\#embed-container\").height(newHeight);

});

193. Your code in **embed.js** should match the following screenshot.

![](/media/image66.png){width="3.242325021872266in"
height="2.776030183727034in"}

Remember you can copy and paste all the code at once by using the text
in **Exercise 4 - embed.js.txt** in the **StudentLabFiles** folder.

194. Save your changes and close **embed.js**.

195. Run the web application in the Visual Studio Code debugger to test
     your work on the **Embed** page.

196. Start the Visual Studio Code debugger by selecting **Run \> Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

197. The **UserOwnsData** web application should display the home page
     as shown to an anonymous user.

198. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page and login when prompted.

199. You should now be able to navigate to the **Embed** page and see
     the Power BI report displayed on the page.

![](/media/image67.png){width="5.094639107611549in"
height="3.3410094050743657in"}

200. Try resizing the browser window. The embedded report should
     continually adapt to the size of the window.

![](/media/image68.png){width="4.354166666666667in"
height="3.1194575678040244in"}

201. You\'re done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

You have now reached an important milestone. You can now tell all your
peers that you have embedded a Power BI report. However, there is more
that you can do to improve the developer experience for writing
client-side code against the Power BI JavaScript API. In the next
exercise, you will add support to your project so that you can program
client-side code using TypeScript instead of JavaScript. By moving to
TypeScript you can benefit from strongly-typed programming, compile-time
type checking and much better IntelliSense.

### Exercise 5: Adding TypeScript Support to a .NET 5 Project

In this exercise, you will add support for developing your client-side
code with TypeScript instead of JavaScript. It is assumed that you have
already installed Node.js and that the Node Package Manager application
named **npm** is available at the commend line. You will begin by adding
several Node.js configuration files to the root folder of the
**UserOwnsData** project. After that you will restore a set of Node.js
packages and use the webpack utility to compile TypeScript code into an
output file named **embed.js**.

1.  Copy three essential node.js development configuration files into
    the root folder of the **UserOwnsData** project.

    a.  Locate these three files in the **StudentLabFiles** folder.

        i.  **package.json** -- the standard project file for all
            Node.js projects.

        ii. **tsconfig.json** -- a configuration file used by the
            TypeScript compiler (TSC).

        iii. **webpack.config.js** -- a configuration file used by the
             webpack utility.

    b.  Copy **package.json**, **tsconfig.json** and
        **webpack.config.js** into the root folder of the
        **UserOwnsData** project.

![](/media/image69.png){width="1.4270199037620297in"
height="1.316459973753281in"}

Visual Studio Code makes it difficult to add existing files to a project
folder. You can use the Windows Explorer to copy these three files from
the **StudentLabFiles** folder to the **UserOwnsData** project folder.

2.  Restore the Node.js packages which are referenced in
    **package.json**.

    a.  Open **package.json** and review the Node.js packages referenced
        in **devDependencies** section.

![](/media/image70.png){width="2.7951181102362206in"
height="1.1962849956255468in"}

202. Open the Visual Studio Code terminal by clicking the **View \>
     Terminal** menu command or by using **Ctrl+\`** keyboard shortcut.

203. Run the **npm install** command to restore the list of Node.js
     packages.

![](/media/image71.png){width="2.5681233595800523in"
height="0.4573961067366579in"}

204. When you run the **npm install** command, **npm** will download all
     the Node.js packages into the **node_modules** folder.

![](/media/image72.png){width="5.615047025371829in"
height="1.4392891513560806in"}

205. Take a quick look at the **tsconfig.json** file.

206. Open the **tsconfig.json** file in an editor window and examine the
     TypeScript compiler settings inside.

207. When you are done, close **tsconfig.json** without saving any
     changes.

208. Take a quick look at the **webpack.config.js** file.

209. Open the **webpack.config.js** file in an editor window and examine
     its content.

const path = require(\'path\');

module.exports = {

entry: \'./Scripts/embed.ts\',

output: {

filename: \'embed.js\',

path: path.resolve(\_\_dirname, \'wwwroot/js\'),

},

resolve: {

extensions: \[\'.js\', \'.ts\'\]

},

module: {

rules: \[

{ test: /\\.(ts)\$/, loader: \'awesome-typescript-loader\' }

\],

},

mode: \"development\",

devtool: \'source-map\'

};

Note the **entry** property of **model.exports** object is set to
**./Scripts/embed.ts**. The **path** and **filename** of the **output**
object combine to a file path of **wwwroot/js/embed.js**. When the
webpack utility runs, it will look for a file named **embed.ts** in the
**Scripts** folder as its main entry point for the TypeScript compiler
(tsc.exe) and produce an output file in named **embed.js** in the
**wwwroot/js** folder.

210. When you are done, close **webpack.config.js** without saving any
     changes.

211. Add a new TypeScript source file named **embed.ts**.

212. In the **UserOwnsData** project folder, create a new top-level
     folder named **Scripts**.

213. Create a new file inside the **Scripts** folder named **embed.ts**.

![](/media/image73.png){width="2.1165649606299213in"
height="0.9247233158355206in"}

214. In Windows Explorer, locate the **Exercise 5 - embed.ts.txt** file
     in the **StudentLabFiles** folder.

215. Open **Exercise 5 - embed.ts.txt** in a text editor such as Notepad
     and copy all its contents to the Windows clipboard.

216. Return to Visual Studio Code and paste the content of **Exercise
     5 - embed.ts.txt** into **Embed.ts.**

![](/media/image74.png){width="5.932592957130359in"
height="1.917817147856518in"}

217. Save your changes and close **embed.ts**.

218. Use the webpack utility to compile **embed.ts** into **embed.js**.

219. Locate the original **embed.js** file in the **wwwroot/js** folder
     and delete it.

![](/media/image75.png){width="1.1165649606299213in"
height="1.1683705161854767in"}

220. Open the Visual Studio Code terminal by clicking the **View \>
     Terminal** menu command or by using **Ctrl+\`** keyboard shortcut.

221. Run the **npm run build** command to run the webpack utility.

222. When you run **npm run build**, webpack should automatically
     generate a new version of **embed.js** in the **wwwroot/js**
     folder.

![](/media/image76.png){width="3.7460378390201226in"
height="1.646091426071741in"}

223. Open the new version of **embed.js**. You should see it is a source
     file generated by the webpack utility.

![](/media/image77.png){width="3.3779396325459317in"
height="1.1511909448818898in"}

224. Close **embed.js** without saving any changes.

225. Update **UserOwnsData.csproj** to add the **npm run build** command
     as part of the dotnet build process.

226. Open the .NET 5 project file **UserOwnsData.csproj** in an editor
     window.

![](/media/image78.png){width="6.611255468066492in"
height="2.3858333333333333in"}

227. Add a new **Target** element named **PostBuild** to run the **npm
     run build** command as shown in the following code listing.

\<Project Sdk=\"Microsoft.NET.Sdk.Web\"\>

\<PropertyGroup\>

\<TargetFramework\>netcoreapp3.1\</TargetFramework\>

\<UserSecretsId\>aspnet-UserOwnsData-87660A42-54AC-4CF9-8583-B31608FED004\</UserSecretsId\>

\<WebProject_DirectoryAccessLevelKey\>0\</WebProject_DirectoryAccessLevelKey\>

\</PropertyGroup\>

\<ItemGroup\>

\<PackageReference Include=\"Microsoft.Identity.Web\"
Version=\"0.3.0-preview\" /\>

\<PackageReference Include=\"Microsoft.Identity.Web.UI\"
Version=\"0.3.0-preview\" /\>

\<PackageReference Include=\"Microsoft.PowerBi.Api\" Version=\"3.14.0\"
/\>

\</ItemGroup\>

\<Target Name=\"PostBuild\" AfterTargets=\"PostBuildEvent\"\>

\<Exec Command=\"npm run build\" /\>

\</Target\>

\</Project\>

228. Save your changes and close **UserOwnsData.csproj**.

229. Return to the terminal and run the **dotnet build** command.

![](/media/image79.png){width="1.8871423884514436in"
height="0.37452318460192474in"}

230. When you run the **dotnet build** command, the output window should
     show you that the webpack command is running.

![](/media/image80.png){width="2.3472648731408574in"
height="1.9941863517060368in"}

Now whenever you start a debug session with the **{F5}** key, the
TypeScript in **embed.ts** will be automatically compiled into
**embed.js**.

231. Run the web application in the Visual Studio Code debugger to test
     your work on the **Embed** page.

232. Start the Visual Studio Code debugger by selecting **Run \> Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

233. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page and login when prompted.

234. You should now be able to navigate to the **Embed** page and see
     the Power BI report displayed on the page.

![](/media/image67.png){width="4.764442257217848in"
height="1.1014654418197725in"}

When you test the **UserOwnsData** web application, it should behave
just as it did when you tested it in Exercise 4. The difference is that
now the client-side behavior is now implemented with TypeScript instead
of JavaScript.

### Exercise 6: Creating a View Model for App Workspaces

Up to this point, you have implemented the **UserOwnsData** project to
embed a single report by hard-coding the IDs of that report and its
hosting workspace. In this exercise, you will remove the hard-coded IDs
and extend the **Embed** page of the **UserOwnsData** project to
dynamically discover what workspaces and reports are available to the
current user.

1.  Extend the **PowerBiServiceApi** class with a new method named
    **GetEmbeddedViewModel**.

    a.  Locate the **PowerBiServiceApi.cs** in the **Service** folder
        and open it in an editor window.

    b.  Add the following method named **GetEmbeddedViewModel** to the
        end of **PowerBiServiceApi** class.

public async Task\<string\> GetEmbeddedViewModel(string appWorkspaceId =
\"\") {

var accessToken =
this.tokenAcquisition.GetAccessTokenForUserAsync(RequiredScopes).Result;

var tokenCredentials = new TokenCredentials (accessToken, \"Bearer\");

PowerBIClient pbiClient = new PowerBIClient (new Uri
(urlPowerBiServiceApiRoot), tokenCredentials);

Object viewModel;

if (string.IsNullOrEmpty (appWorkspaceId)) {

viewModel = new {

currentWorkspace = \"My Workspace\",

workspaces = ( await pbiClient.Groups.GetGroupsAsync() ).Value,

datasets = ( await pbiClient.Datasets.GetDatasetsAsync() ).Value,

reports = ( await pbiClient.Reports.GetReportsAsync() ).Value,

token = accessToken

};

} else {

Guid workspaceId = new Guid (appWorkspaceId);

var workspaces = (await pbiClient.Groups.GetGroupsAsync ()).Value;

var currentWorkspace = workspaces.First ((workspace) =\> workspace.Id ==
workspaceId);

viewModel = new {

workspaces = workspaces,

currentWorkspace = currentWorkspace.Name,

currentWorkspaceIsReadOnly = currentWorkspace.IsReadOnly,

datasets = (await pbiClient.Datasets.GetDatasetsInGroupAsync
(workspaceId)).Value,

reports = (await pbiClient.Reports.GetReportsInGroupAsync
(workspaceId)).Value,

token = accessToken

};

}

return JsonConvert.SerializeObject(viewModel);

}

The **GetEmbeddedViewModel** method accepts an **appWorkspaceId**
parameter and returns a string value with JSON-formatted data. If the
**appWorkspaceId** parameter is blank, the **GetEmbeddedViewModel**
method returns a view model for the current user\'s personal workspace.
If the **appWorkspaceId** parameter contains a GUID, the
**GetEmbeddedViewModel** method returns a view model for the app
workspace associated with that GUID.

You can copy and paste this method from the **Exercise 6 -
PowerBiServiceApi.cs.txt** file in the **StudentLabFiles** folder.

c.  To enhance your conceptual understanding, examine a sample of JSON
    returned by the **GetEmbeddedViewModel** method.

![](/media/image81.png){width="4.212295494313211in"
height="1.7839851268591427in"}

d.  Save your work and close **PowerBiServiceApi.cs**.

```{=html}
<!-- -->
```
2.  Modify **Embed** method in **HomeController** to call the
    **GetEmbeddedViewModel** method.

    a.  Locate the **HomeController.cs** file and open it in an editor
        window.

    b.  Locate the **Embed** method which should currently match this
        **Embed** method implementation.

public async Task\<IActionResult\> Embed() {

Guid workspaceId = new Guid(\"912f2b34-7daa-4589-83df-35c75944d864\");

Guid reportId = new Guid(\"cd496c1c-8df0-48e7-8b92-e2932298743e\");

var viewModel = await powerBiServiceApi.GetReport(workspaceId,
reportId);

return View(viewModel);

}

c.  Delete the **Embed** method implementation and replace it the
    following code.

public async Task\<IActionResult\> Embed(string workspaceId) {

var viewModel = await
powerBiServiceApi.GetEmbeddedViewModel(workspaceId);

// cast string value to object type in order to pass string value as MVC
view model

return View(viewModel as object);

}

235. Save your work and close **HomeController.cs**.

There are a few things to note about the new implementation of the
**Embed** controller action method. First, the method now takes a string
parameter named **workspaceId**. When this controller method is passed a
workspace ID in the **workspaceId** query string parameter, it passes
that workspace ID along to the **PowerBiServiceApi** class when it calls
the **GetEmbeddedViewModel** method.

The second thing to note about this example if that the string-based
**viewModel** variable is cast to a type of **object** in the **return**
statement using the syntax **View(viewModel as object)**. This is a
required workaround because passing a string parameter to **View()**
would fail because the string value would be treated as a view name
instead of a view model being passed to the underlying view.

236. Replace the code in **Embed.cshtml** with a better implementation.

237. Locate **Embed.cshtml** file in the **Views \> Home** folder, open
     it in an editor window and delete all the content inside.

238. In Windows Explorer, locate the **Exercise 6 - Embed.cshtml.txt**
     file in the **StudentLabFiles** folder.

239. Open **Exercise 6 - Embed.cshtml.txt** in a text editor such as
     Notepad and copy all its contents to the Windows clipboard.

240. Return to Visual Studio Code and paste the content of **Exercise
     6 - Embed.cshtml.txt** into **Embed.cshtml.**

![](/media/image82.png){width="3.288343175853018in"
height="1.072723097112861in"}

241. Save your changes and close **Embed.cshtml.**

242. Replace the code in **Embed.ts** with a better implementation.

243. Locate **Embed.ts** file in the **Scripts** folder, open it in an
     editor window and delete all the content inside.

244. In Windows Explorer, locate the **Exercise 6 - Embed.ts.txt** file
     in the **StudentLabFiles** folder.

245. Open **Exercise 6 - Embed.ts.txt** in a text editor such as Notepad
     and copy all its contents to the Windows clipboard.

246. Return to Visual Studio Code and paste the content of **Exercise
     6 - Embed.ts.txt** into **Embed.ts.**

![](/media/image83.png){width="3.9093307086614173in"
height="1.2613746719160106in"}

247. Save your changes and close **Embed.cshtml.**

248. Run the web application in the Visual Studio Code debugger to test
     your work on the **Embed** page.

249. Start the Visual Studio Code debugger by selecting **Run \> Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

250. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page and login when prompted.

251. The **Embed** page should appear much differently than before as
     shown in the following screenshot.

![](/media/image84.png){width="4.802208005249343in"
height="1.3391666666666666in"}

Note there is a dropdown list for the **Current Workspace** that you can
use to navigate across workspaces.

252. Navigate to the workspace you created earlier in this lab.

![](/media/image85.png){width="3.4409962817147854in"
height="1.1275535870516185in"}

253. Click on a report in the **Open Report** section.

![](/media/image86.png){width="3.9275535870516185in"
height="1.031192038495188in"}

254. The report should open in read-only mode.

![](/media/image87.png){width="4.70400043744532in"
height="1.2903969816272967in"}

255. Click the **Toggle Edit Mode** button to move the report into edit
     mode.

![](/media/image88.png){width="2.900379483814523in"
height="1.1340048118985127in"}

256. Note that when the report goes into edit mode, there isn\'t much
     space to work on the report while editing.

![](/media/image89.png){width="3.051535433070866in"
height="1.3954483814523184in"}

257. Click the Full Screen button to enter full screen mode

![](/media/image90.png){width="2.8694892825896763in"
height="0.7425590551181103in"}

You can invoke the **File \> Save** command in a report that is in edit
mode to save your changes.

258. Press the **Esc** key in the keyboard to exit full screen mode.

259. Click on a second report in the **Open Report** section to navigate
     between reports.

![](/media/image91.png){width="3.4528510498687663in"
height="1.2100831146106736in"}

260. Create a new report by clicking on a dataset name in the **New
     Report** section.

![](/media/image92.png){width="4.146908355205599in"
height="1.17292104111986in"}

261. Add a simple visual of any type to the new report.

![](/media/image93.png){width="4.997918853893263in"
height="1.8243285214348206in"}

262. Save the new report using the **File \> Save as** menu command.

![](/media/image94.png){width="3.2620614610673666in"
height="1.0821883202099738in"}

263. Give your new report a name.

![](/media/image95.png){width="2.788377077865267in"
height="0.7810728346456693in"}

264. After you click save, the new report should show up in the Open
     Report section and be displayed in read-only mode.

![](/media/image96.png){width="5.615999562554681in"
height="2.972930883639545in"}

265. When you\'re done testing, close the browser, return to Visual
     Studio Code and stop the debug session.
