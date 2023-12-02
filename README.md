# GitHub Advanced Security for Azure DevOps (GHAzDO) Hands-on Lab

## Initial Setup
### Git Repository

Perform the following action to setup resource groups and secrets fo the GitHub Actions Workflows

- Clone the starter solution branch to your local machine

  ```powershell
  git clone -b starter-solution --single-branch https://github.com/4tecture/ghazdo-demo.git
  cd ghazdo-demo
  git branch -m starter-solution main
  git remote remove origin
  ```	

- Create a new empty repository (uncheck "add a README" and set add a .gitignore to "None") with the name "ghazdo-demo" in your Azure DevOps Project

- Copy the URL of the new repository

- Push your local repository to the new remote repository

  ```powershell
  git remote set-url origin <URL of your new repository>
  git push -u origin starter-solution
  ```

### Analyse the solution

- Please make yourself familiar with the solution. The solution is a simple ASP.NET Core Web API with a SQL Server database (in memory). 
- Analyse the Azure Pipelines YAML file in the .azure-pipelines folder. It is a simple pipeline compiling the solution.

### Setup the CI pipeline

In your Azure DevOps Project, create a new Pipeline based on your Git repository.
- Go to pipelines and click "New Pipeline"
- Choose "Azure Repos Git"
- Select your repository
- Choose "Existing Azure Pipelines YAML file"
- Select the branch "main"
- Select the file path ".azure-pipelines/azure-pipelines.yml"
- Click "Continue"
- Click "Run"

## Enable GitHub Advanced Security

In your Azure DevOps Project, navigate to "Manage Repositories" and enable GitHub Advanced Security for your repository.
- Go to "Project Settings"
- Click "Repositories"
- Select the `ghazdo-demo` repository
- In settings, switch "Advanced Security" to "On"
- Also check the "Block secrets on push" option.

### Enhance the CI pipeline with GitHub Advanced Security

- As a first step in your pipeline, add the GHAzDO Initialize task:

  ```yaml
  - task: AdvancedSecurity-Codeql-Init@1
    inputs:
      languages: 'csharp'
      querysuite: 'security-extended'
      displayName: 'Initialize CodeQL'
  ```
- After the compilation steps, add the following tasks to your pipeline:

  ```yaml
  # Run Advanced Security Dependency Scanning
  - task: AdvancedSecurity-Dependency-Scanning@1
    displayName: 'Dependency Scanning'

  # Run CodeQL Analysis
  - task: AdvancedSecurity-Codeql-Analyze@1
    displayName: 'CodeQL Analysis'

  # Publish CodeQL Analysis Results
  - task: AdvancedSecurity-Publish@1
    displayName: 'Publish CodeQL Results'
  ```

Run the pipeline and check the results.

Under "Repos" there is a new "Advanced Security" menu item. Check the results of the security analysis. You should see some dependency issues.

## Add security issues to your code

Please note that we add security issues to our code base for demonstration purposes only. In a real world scenario, you would of course fix the issues.

### Add a SQL Injection vulnerability

In the controllers folder of your ASP.NET Core application, add a new file named `SqlInjectionController.cs` with the following content:

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelloWorld.Database;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace HelloWorld.Controllers
{
    public class SqlInjectionController : Controller
    {
        private DemoContext _context;

        public SqlInjectionController(DemoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("SqlInjection/SearchPersonUnsecure/{name}")]
        public async Task<IActionResult> SearchPersonUnsecure(string name)
        {
            var conn = _context.Database.GetDbConnection();
            var query = "SELECT Id, FirstName, LastName FROM Person WHERE FirstName Like '%" + name + "%'";
            IEnumerable<Person> persons;

            try
            {
                await conn.OpenAsync();
                persons = await conn.QueryAsync<Person>(query);
            }

            finally
            {
                conn.Close();
            }
            return Ok(persons);
        }
    }
}

```

Run the pipeline again and check the results. You should see a new SQL Injection issue.

### Try to push secrets

In the Controllers folder of your solution, add a new file named `ThirdPartyAccessController.cs` with the following content:

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThirdPartyAccessController : ControllerBase
    {

        private string api_key = "qqvu5wi56wwyifvnqisqoyx34j2nmcxy7c3bl265kthy2aztqfva";  //This is a fake API key for demonstration purposes

        //Simulated sensitive data (AWS credentials)
        private string aws_access_key_id = "AKIAIOSFODNN7EXAMPLE";
        private string aws_secret_access_key = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";


        [HttpGet("DoSomething")]
        public IActionResult GetSecret()
        {
            var myConnectionString = $"https://myservice?apikey={api_key}";
            return Ok("Used some secrets for fake purposes");
        }
        
    }
}
```

Commit and push your code. You should see an error while pushing the code. Azure DevOps blocks the push because of the secret.

In order to see the secrets in your security issues, you can disable the "Block secrets on push" option in the repository settings. Push again. After the successful push, you can re-enable the option again. Run your CI build again and check the results. You should see a new secret issue.

### Add Gated PR features to your pipeline.

Enable the CI build as a PR build.
- Go to "Project Settings"
- Click "Repositories"
- Select the `ghazdo-demo` repository
- Select "Policies"
- Go to branch policies and select the main branch.
- Enable the "Build validation" policy and select the CI build.

Add a powershell script from [GHAzDO-Resources](https://github.com/microsoft/GHAzDO-Resources)
- Copy the content from the following [file](https://github.com/microsoft/GHAzDO-Resources/blob/main/src/pr-gating/CIGate.ps1)
- Create a new file name `CIGate.ps1` in the `.azure-pipelines` folder of your repository and paste the content.

Extend the CI pipeline with the following tasks:

```yaml
# Conditional step for Pull Requests: Run a PowerShell script for additional PR validation
- ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
  - task: PowerShell@2
    displayName: 'PR Gating - Additional Checks for Pull Requests'
    inputs:
      targetType: filePath
      filePath: $(Build.SourcesDirectory)/.azure-pipelines/CIGate.ps1
      pwsh: true
    env:
      MAPPED_ADO_PAT: $(GHAzDO_PAT)
```

Create a PAT to access the GHazDO API:
- Create a new PAT in your Azure DevOps Project
- Please make sure that the PAT has the following permissions:
  - Advanced Security - Read
  - Code - Read
  - Pull Request Threads - Read & write
- In your Pipeline, add a new secret variable name `GHAzDO_PAT` and copy the PAT as value.

### Check the PR build gates

- Create a new feature branch (i.e feature/encryption)
- In the controllers folder of your solution, add a new file named `EncryptionController.cs` with the following content:
  
  ````csharp
  using HelloWorld.Services;
  using Microsoft.AspNetCore.Mvc;

  namespace HelloWorld.Controllers
  {
      
      public class EncryptionController : Controller
      {
          private IBadEncryptionService _badEncryptionService;

          public EncryptionController(IBadEncryptionService badEncryptionService)
          {
              _badEncryptionService = badEncryptionService;
          }

          [HttpGet("Encryption/EncryptString/{input}")]
          public IActionResult EncryptString(string input)
          {
              return Ok(_badEncryptionService.Encrypt(input));
          }
      }
  }
  ```
- Push the branch and create a PR to the main branch.
- New issues should be reported in the PR.