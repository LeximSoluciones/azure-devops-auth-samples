using System;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace ClientLibraryConsoleAppSample
{
    class Program
    {
        internal const string azureDevOpsOrganizationUrl = "https://dev.azure.com/lexim1/"; //change to the URL of your Azure DevOps account; NOTE: This must use HTTPS

        static void Main(string[] args)
        {
            var connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), new VssClientCredentials());

            var git = connection.GetClient<GitHttpClient>();

            var repos = git.GetRepositoriesAsync().Result;

            var repo = repos.First(x => x.Name.ToLower() == "ecipsa");

            var master = git.GetBranchAsync(repo.ProjectReference.Id, repo.Id, "master").Result;

            //var branch = git.GetBranchAsync(repo.ProjectReference.Id, repo.Id, "AplicacionCobranza2.0").Result;
            //var branch2 = git.GetBranchAsync(repo.ProjectReference.Id, repo.Id, "DEV-2018-07-ControlLiquidacionMasiva").Result;
            var branch = git.GetBranchAsync(repo.ProjectReference.Id, repo.Id, "2018-12.2").Result;
            var branch2 = git.GetBranchAsync(repo.ProjectReference.Id, repo.Id, "TEST").Result;

            var prs = git.GetPullRequestsAsync(repo.ProjectReference.Id, repo.Id, new GitPullRequestSearchCriteria()).Result;

            var branchName = "refs/heads/" + branch.Name;

            var pr = new GitPullRequest
            {
                //RemoteUrl = repo.RemoteUrl,
                Repository = repo,
                SourceRefName = "refs/heads/" + branch.Name,
                TargetRefName = "refs/heads/" + branch2.Name,
                Title = $"CI: {branch.Name} -> {branch2.Name}",
            };

            //var r = git.CreatePullRequestAsync(pr, repo.Id).Result;
            //r = git.GetPullRequestByIdAsync(r.PullRequestId).Result;

            var _ref = git.GetRefsAsync(repo.Id).Result.First(x => x.Name == branchName);

            // create a new branch from the source
            GitRefUpdateResult refCreateResult = git.UpdateRefsAsync(
                new GitRefUpdate[] { new GitRefUpdate() {
                    OldObjectId = new string('0', 40),
                    NewObjectId = _ref.ObjectId,
                    Name = $"refs/heads/vsts-api-sample/prueba-branch-ci",
                } },
                repositoryId: repo.Id).Result.First();

            Console.ReadLine();

        }
    }
}

