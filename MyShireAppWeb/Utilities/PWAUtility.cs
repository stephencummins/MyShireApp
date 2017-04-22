using MyShireAppWeb.Models;
using Microsoft.ProjectServer.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//using csom = Microsoft.ProjectServer.Client;


namespace MyShireAppWeb.Utilities
{
    public class PWAUtility
    {

        const int DEFAULTTIMEOUTSECONDS = 300;
        private static string taskCFName = "Shire Status";



        /// <summary>
        /// Creates the Project 
        /// </summary>
        /// <param name="ProjectName">Project Name</param>
        /// <returns>True, If Created</returns>
        public bool CreatePWAProject(ProjectContext projectcontext, ProjectLunchProvisionDetails objLaunchProvision)
        {

            try
            {
                Guid EPTID = new Guid(objLaunchProvision.LaunchTemplate);

                ProjectCreationInformation newProj = new ProjectCreationInformation()
                {
                    Id = Guid.NewGuid(),
                    Name = objLaunchProvision.ProjectName,
                    EnterpriseProjectTypeId = EPTID

                };
                PublishedProject newPublishedProj = projectcontext.Projects.Add(newProj);
                QueueJob qJob = projectcontext.Projects.Update();

                JobState jobState = projectcontext.WaitForQueue(qJob,/*timeout for wait*/ 50);

                if (jobState == JobState.Success)
                {
                    AddProjectResources(projectcontext, newPublishedProj, objLaunchProvision.Project_Team, objLaunchProvision.Local_Launch_Leader);


                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        /// <summary>
        /// Get Project By Name
        /// </summary>
        /// <param name="projectContext"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public PublishedProject GetProjectByName(ProjectContext projectContext, string ProjectName)
        {
            var ProjectColl = projectContext.Projects;

            projectContext.Load(ProjectColl, Proj => Proj
            .Include(PubProj => PubProj.Name, PubProj => PubProj.EnterpriseProjectType)
            .Where(PubProj => PubProj.Name == ProjectName));
            projectContext.ExecuteQuery();

            return ProjectColl.FirstOrDefault();
        }

        public JobState AddProjectResources(ProjectContext projectcontext, PublishedProject project, List<PWAResources> PWAResources, string TeamLeader)
        {


            DraftProject draft = project.CheckOut();

            foreach (PWAResources Res in PWAResources)
            {
                ProjectResourceCreationInformation r = new ProjectResourceCreationInformation();
                r.Id = new Guid(Res.ResourceID.ToString());//Enterprise Resource GUID
                r.Name = Res.ResourceName;
                draft.ProjectResources.Add(r);
            }

            User lead = GetResourceAsUser(projectcontext, TeamLeader);
            draft.Owner = lead;

            draft.Update();
            projectcontext.Load(draft);
            projectcontext.ExecuteQuery();
            projectcontext.Projects.Update();
            projectcontext.ExecuteQuery();
            draft.Publish(true);
            return projectcontext.WaitForQueue(projectcontext.Projects.Update(), 10);

            return JobState.Success;
        }
       

        /// <summary>
        /// Get the Lookup value entry from lookuptable list
        /// </summary>
        /// <param name="projectContext"></param>
        /// <param name="cffield"></param>
        /// <param name="CFValue"></param>
        /// <returns></returns>
        public string[] GetLookipValueEntry(ProjectContext projectContext, CustomField cffield, string CFValue)
        {
            try
            {
                projectContext.Load(cffield.LookupTable);
                projectContext.ExecuteQuery();

                Guid LTuid = cffield.LookupTable.Id;

                var LookupTableList = projectContext.LoadQuery(projectContext.LookupTables.Where(LT => LT.Id == LTuid));
                projectContext.ExecuteQuery();

                projectContext.LoadQuery(LookupTableList.First().Entries).Where(FE => FE.FullValue == CFValue);
                projectContext.ExecuteQuery();

                LookupEntry Entry = LookupTableList.First().Entries.FirstOrDefault();

                return new string[] { Entry.InternalName };

            }
            catch (CollectionNotInitializedException ex)
            {
                return null;
            }
        }

        public bool IsLookupCF(ProjectContext projectContext, CustomField cffield)
        {
            try
            {
                projectContext.Load(cffield.LookupTable, tbl => tbl.Id);
                projectContext.ExecuteQuery();

                var tblId = cffield.LookupTable.Id;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Method will retrun the Dictionary object with Custom Field Name as Key 
        /// & an string array Custom Field Lookup values as value
        /// </summary>
        /// <param name="projectContext">Project Context</param>
        /// <param name="CustomFieldNames">String[] of Custom Field Names of lookup type</param>
        /// <returns>Dictionary [Custom Fieldname, Lookup table values[]]</returns>
        public Dictionary<string, List<CustomFieldEntryInfo>> GetLookupFieldValues(ProjectContext projectContext, string[] LookUpCFNames)
        {

            Dictionary<string, List<CustomFieldEntryInfo>> CFValuesArray = new Dictionary<string, List<CustomFieldEntryInfo>>();

            //**Need to optimize the below load query to load only those fields which are in string[] LookUpCFNames
            var CustomFieldNames = projectContext.LoadQuery(projectContext.CustomFields);
            projectContext.ExecuteQuery();

            foreach (CustomField cffield in CustomFieldNames)
            {

                if (LookUpCFNames.Contains(cffield.Name))//**Work-around as load query is not optimized above
                {
                    projectContext.Load(cffield.LookupTable);
                    projectContext.ExecuteQuery();

                    Guid LTuid = cffield.LookupTable.Id;

                    var LookupTableList = projectContext.LoadQuery(projectContext.LookupTables.Where(LT => LT.Id == LTuid));
                    projectContext.ExecuteQuery();

                    projectContext.Load(LookupTableList.First().Entries);
                    projectContext.ExecuteQuery();

                    string[] CFValues = new string[LookupTableList.First().Entries.Count];
                    var valueindex = 0;

                    List<CustomFieldEntryInfo> CustomFieldEntries = new List<CustomFieldEntryInfo>();

                    foreach (LookupEntry lutEntry in LookupTableList.First().Entries)
                    {
                        CustomFieldEntryInfo CFEntryinfo = new CustomFieldEntryInfo();

                        CFEntryinfo.EntryID = lutEntry.Id.ToString();
                        CFEntryinfo.EntryValue = lutEntry.FullValue;
                        CFEntryinfo.EntryInternalName = lutEntry.InternalName.ToString();

                        CustomFieldEntries.Add(CFEntryinfo);
                        valueindex++;
                    }

                    CFValuesArray.Add(cffield.Name, CustomFieldEntries);
                }
            }
            return CFValuesArray;
        }

        public List<CustomFieldEntryInfo> GetLookupFieldValues(ProjectContext projectContext, string LookUpCFName)
        {

            List<CustomFieldEntryInfo> CFValuesArray = new List<CustomFieldEntryInfo>();

            //**Need to optimize the below load query to load only those fields which are in string[] LookUpCFNames
            projectContext.Load(projectContext.CustomFields);
            projectContext.ExecuteQuery();


            CustomField projCF = projectContext.CustomFields.FirstOrDefault(cffield => cffield.Name == LookUpCFName);

            projectContext.Load(projCF.LookupTable);
            projectContext.ExecuteQuery();

            Guid LTuid = projCF.LookupTable.Id;

            var LookupTableList = projectContext.LoadQuery(projectContext.LookupTables.Where(LT => LT.Id == LTuid));
            projectContext.ExecuteQuery();

            projectContext.Load(LookupTableList.First().Entries);
            projectContext.ExecuteQuery();

            string[] CFValues = new string[LookupTableList.First().Entries.Count];
            var valueindex = 0;


            foreach (LookupEntry lutEntry in LookupTableList.First().Entries)
            {
                CustomFieldEntryInfo CFEntryinfo = new CustomFieldEntryInfo();

                CFEntryinfo.EntryID = lutEntry.Id.ToString();
                CFEntryinfo.EntryValue = lutEntry.FullValue;
                CFEntryinfo.EntryInternalName = lutEntry.InternalName.ToString();

                CFValuesArray.Add(CFEntryinfo);
                valueindex++;
            }


            return CFValuesArray;
        }

        /// <summary>
        /// Get List of Enterprise Project Types
        /// </summary>
        /// <param name="projectcontext">Project Context</param>
        /// <returns>String of EPT Names</returns>
        public List<EPTInfo> GetPWAEPTList(ProjectContext projectcontext)
        {
            List<EPTInfo> EPTList = new List<EPTInfo>();
            var pwaepts = projectcontext.EnterpriseProjectTypes;
            //projectcontext.Load(epts, ept => ept.Include(pt => pt.Name, pt => pt.Id).Where(pt => pt.Name == EPT_Name));
            projectcontext.Load(pwaepts, ept => ept.Include(pt => pt.Name, pt => pt.Id));
            projectcontext.ExecuteQuery();

            string[] EPTs = new string[pwaepts.Count];
            var index = 0;
            foreach (EnterpriseProjectType EPT in pwaepts)
            {
                EPTInfo ept = new EPTInfo();
                ept.EPTId = EPT.Id.ToString();
                ept.EPTName = EPT.Name;

                EPTList.Add(ept);
            }



            return EPTList;
        }

        /// <summary>
        /// Get Project Server Resource
        /// </summary>
        /// <param name="projectContext"></param>
        /// <returns>Dictionary object with [ResourcesID,ResourceName]</returns>
        public List<PWAResources> GetResources(ProjectContext projectContext)
        {
            List<PWAResources> resources = new List<PWAResources>();

            projectContext.Load(projectContext.EnterpriseResources);
            projectContext.ExecuteQuery();
            if (projectContext.EnterpriseResources.Count() > 0)
            {
                var ResourcesColl = projectContext.LoadQuery(projectContext.EnterpriseResources).OrderBy(Res => Res.Name);
                projectContext.ExecuteQuery();
                PWAResources newRes;
                foreach (EnterpriseResource Res in ResourcesColl)
                {
                    newRes = new PWAResources();
                    newRes.ResourceID = Res.Id.ToString();
                    newRes.ResourceName = Res.Name;
                    newRes.ResourceEmail = Res.Email;
                    resources.Add(newRes);
                }
            }
            return resources;
        }

        public PWAResources GetResource(ProjectContext projectContext, string UserName)
        {
            PWAResources resources = new PWAResources();

            projectContext.Load(projectContext.EnterpriseResources);
            projectContext.ExecuteQuery();
            if (projectContext.EnterpriseResources.Count() > 0)
            {
                var ResourcesColl = projectContext.LoadQuery(projectContext.EnterpriseResources).Where(Res => Res.Name == UserName).OrderBy(Res => Res.Name);
                projectContext.ExecuteQuery();
                EnterpriseResource EntRes = ResourcesColl.FirstOrDefault();

                resources.ResourceID = EntRes.Id.ToString();
                resources.ResourceName = EntRes.Name;

            }
            return resources;
        }

        public User GetResourceAsUser(ProjectContext projectContext, string UserName)
        {

            projectContext.Load(projectContext.EnterpriseResources);
            projectContext.ExecuteQuery();
            var ResourcesColl = projectContext.LoadQuery(projectContext.EnterpriseResources).Where(Res => Res.Name == UserName).OrderBy(Res => Res.Name);
            projectContext.ExecuteQuery();
            EnterpriseResource EntRes = ResourcesColl.FirstOrDefault();
            projectContext.Load(EntRes.User);
            projectContext.ExecuteQuery();
            return EntRes.User;

        }

        /// <summary>
        /// Get Resource Assignment
        /// </summary>
        /// <param name="projectContext"></param>
        /// <returns>list containing assignment detail</returns>
        public List<PWAAssignment> ResourceAssignments(ProjectContext projContext)
        {
            try
            {
                // Get the user name  and their assignments
                EnterpriseResource self = EnterpriseResource.GetSelf(projContext);
                projContext.Load(self, r => r.Name, r => r.Assignments.IncludeWithDefaultProperties(assignment => assignment.Project, assignment => assignment.Task));
                projContext.ExecuteQuery();

                // Get the assignments and Project Name for the resource
                List<PWAAssignment> AssignmentList = new List<PWAAssignment>();
                String Username = self.Name;

                string name;
                string ProjName;               
                Guid id;
                PWAAssignment NwAssignment;
                string taskID;

                List<LookupEntry> lkpEntries = getAllLookupEntries(projContext, lkpTableNames);

                List<PWAAssignment> projectAssignment = new List<PWAAssignment>();
                List<ProjectName> Projects = new List<ProjectName>();
                ProjectName ProjectAssignment;
                for (int k = 0; k < self.Assignments.Count; k++)
                {
                    ProjectAssignment = new ProjectName();

                    if (k == 0)
                    {
                        ProjectAssignment.TaskProjectUID = self.Assignments.ElementAt(k).Project.Id;
                        Projects.Add(ProjectAssignment);
                    }

                    else if (self.Assignments.ElementAt(k).Project.Name != self.Assignments.ElementAt(k - 1).Project.Name)
                    {
                        ProjectAssignment.TaskProjectUID = self.Assignments.ElementAt(k).Project.Id;
                        Projects.Add(ProjectAssignment);

                    }

                }

                for (int l = 0; l < Projects.Count; l++)
                {
                    projectAssignment = ReadAllTasks(projContext, Projects[l].TaskProjectUID, Username, lkpEntries);
                    
                  for (int j = 0; j < self.Assignments.Count; j++)
                    {
                    name = self.Assignments.ElementAt(j).Project.Name + ": " + self.Assignments.ElementAt(j).Name;
                    taskID = self.Assignments.ElementAt(j).Task.Id.ToString();
                    ProjName = self.Assignments.ElementAt(j).Project.Name;

                    if (self.Assignments.ElementAt(j).PercentComplete<100 && self.Assignments.ElementAt(j).Project.Id == Projects[l].TaskProjectUID)
                    {
                        string TaskName = self.Assignments.ElementAt(j).Task.Name;
                        NwAssignment = new PWAAssignment();
                        NwAssignment.taskName = self.Assignments.ElementAt(j).Task.Name;
                        NwAssignment.taskID = self.Assignments.ElementAt(j).Task.Id;
                        NwAssignment.AssignmentID = self.Assignments.ElementAt(j).Id;
                        NwAssignment.ProjectID = self.Assignments.ElementAt(j).Project.Id; ;
                        NwAssignment.ID = projectAssignment.FirstOrDefault(x => x.taskName == TaskName).ID; ;
                        NwAssignment.StartDate = self.Assignments.ElementAt(j).Task.Start.ToShortDateString();
                        NwAssignment.EndDate = self.Assignments.ElementAt(j).Task.Finish.ToShortDateString();
                        //NwAssignment.LastModifiedDate = self.Assignments.ElementAt(j).Modified.Date.ToShortDateString();
                        NwAssignment.RecevedDate = self.Assignments.ElementAt(j).Start.Date.ToShortDateString();
                        NwAssignment.ProjectName = self.Assignments.ElementAt(j).Project.Name;
                        NwAssignment.customFields = projectAssignment.FirstOrDefault(x => x.taskName == TaskName).customFields;
                       // NwAssignment.ID= projectAssignment.FirstOrDefault(x => x.taskName == TaskName).ID;
                         AssignmentList.Add(NwAssignment);

                      }
                  }
            }

                return AssignmentList;
                //return self.Assignments
                // return 0;
            }
            catch (ClientRequestException cre)
            {
                string msg = string.Format("Error: \n\n{1}", cre.GetBaseException().ToString());
                throw new ArgumentException(msg);
            }

        }

        //public bool CheckIssueRelatedtoTask()
        //{
        //    string siteUrl = "https://shirepharma.sharepoint.com/sites/CEPWAST/Shire%20Test%2002/default.aspx";

        //    // List URL = "https://shirepharma.sharepoint.com/sites/CEPWAST/Shire%20Test%2002/Lists/Issues/AllItems.aspx";

        //    var ProjContext = ContextUtility.GetProjectContext();

        //    ClientContext clientContext = new ClientContext(siteUrl);
        //    Microsoft.SharePoint.Client.List oList = clientContext.Web.Lists.GetByTitle("Issues");
        //    // Microsoft.SharePoint.Client.List ListDetail = clientContext.Web.GetList(siteUrl);
        //    //var list = ctx.Web.GetList("sites/dev/lists/Addresses");

        //    clientContext.Load(oList,
        //        list => list.ItemCount);

        //    // clientContext.Load(ListDetail,
        //    //  list => list.Title);

        //    clientContext.ExecuteQuery();
        //    return false;
        //}




        /// <summary>
        /// Update Task custom field values
        /// </summary>
        public JobState UpdateCustomFieldValues(ProjectContext projContext, Guid ProjectUID,String taskName,String Status)
        {
            // Get the Project Context            
            PublishedProject project = projContext.Projects.GetByGuid(ProjectUID);
            if (project == null)
            {
                Console.WriteLine("No Project Found");              
            }

            DraftProject tempDraft = project.Draft;
            JobState state = projContext.WaitForQueue(tempDraft.CheckIn(true), 20);
            DraftProject draft = project.CheckOut();

            //JobState state = projContext.WaitForQueue(project.CheckIn(true), 20);
            //csom.DraftProject draft = project.CheckOut();

            // Retrieve project tasks 
            projContext.Load(draft.Tasks, dt => dt.Where(t => t.Name == taskName));           
            projContext.ExecuteQuery();

            // Check if task in present
            if (draft.Tasks.Count != 1)
            {
                Console.WriteLine("No Task present");                
            }
           
            // Take the selected Task
            DraftTask task = draft.Tasks.First();            

            // Retrieve custom fields by name
            projContext.Load(projContext.CustomFields);
            projContext.ExecuteQuery();
                      
            CustomField taskCF = projContext.CustomFields.FirstOrDefault(cf => cf.Name == taskCFName);
           

            // Get lookup table entry needs to be updated
            LookupEntry taskLookUpEntry = GetLookupEntries(projContext,taskCF, Status);
          
            task[taskCF.InternalName] = new[] { taskLookUpEntry.InternalName };

            // Update project and check in
            draft.Update();
            JobState jobState = projContext.WaitForQueue(draft.Publish(true), DEFAULTTIMEOUTSECONDS);
            JobStateLog(jobState, "Updating project customfield values");
            return projContext.WaitForQueue(projContext.Projects.Update(), 10);
        }

        private static LookupEntry GetLookupEntries(ProjectContext projContext,CustomField cf,String Status)
        {
            //initialize with the value needs to be updated
            string taskCFName = Status;
            projContext.Load(cf, c => c, c => c.LookupEntries.Where(v => v.FullValue == taskCFName));
            projContext.ExecuteQuery();
            try
            {
                Random r = new Random();
                int index = r.Next(0, cf.LookupEntries.Count);
                LookupEntry lookUpEntry = cf.LookupEntries[index];
                projContext.Load(lookUpEntry);
                projContext.ExecuteQuery();
                return lookUpEntry;
            }
            catch (CollectionNotInitializedException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Log to Console the job state for queued jobs
        /// </summary>
        /// <param name="jobState">csom jobstate</param>
        /// <param name="jobDescription">job description</param>
        private static void JobStateLog(JobState jobState, string jobDescription)
        {
            switch (jobState)
            {
                case JobState.Success:
                    Console.WriteLine(jobDescription + " is successfully done.");
                    break;
                case JobState.ReadyForProcessing:
                case JobState.Processing:
                case JobState.ProcessingDeferred:
                    Console.WriteLine(jobDescription + " is taking longer than usual.");
                    break;
                case JobState.Failed:
                case JobState.FailedNotBlocking:
                case JobState.CorrelationBlocked:
                    Console.WriteLine(jobDescription + " failed. The job is in state: " + jobState);
                    break;
                default:
                    Console.WriteLine("Unkown error, job is in state " + jobState);
                    break;
            }
        }

        //Ambure section
        public List<ProjectsInfo> GetCurrentResourceProjects(ProjectContext projContext,Guid resourceUID)
        {
            List<ProjectsInfo> projects = new List<ProjectsInfo>();

            projContext.Load(projContext.Projects, nm => nm.Include(ls => ls.Id, ls => ls.Name, ls => ls.ProjectSiteUrl));
            projContext.ExecuteQuery();

            foreach (PublishedProject p in projContext.Projects)
            {
                projContext.Load(p.ProjectResources.GetByGuid(resourceUID));
                projContext.ExecuteQuery();
                try
                {
                    if (p.ProjectResources.GetByGuid(resourceUID).Id == resourceUID)
                    {
                        ProjectsInfo proj = new ProjectsInfo();
                        proj.ProjectUid = p.Id.ToString();
                        proj.ProjectName = p.Name;
                        proj.ProjectSiteUrl = p.ProjectSiteUrl;
                        projects.Add(proj);
                    }
                }
                catch (Exception e)
                {

                }
            }
            return projects;

        }


        // including amarjeet code 

        //Lookup Tables used to store entries for task custom fields
        public string[] lkpTableNames;
        public PWAUtility()
        {
            lkpTableNames = new string[]
            {
                "Function LT","Health","Priority LT","Organisation LT","Phase LT","Recommended Start End LT",
                "Recommended Start End LT","Status LT"
            };
        }



        public List<PWAAssignment> ReadAllTasks(ProjectContext projContext, Guid ProjectGuid, string ResourceName, List<LookupEntry> lkpEntries)
        {
            //First we will get the lookup table entries from all the concern lookup tables 
           // List<LookupEntry> lkpEntries = getAllLookupEntries(projContext, lkpTableNames);

            //EnterpriseResource self = EnterpriseResource.GetSelf(projContext);
          //  projContext.Load(self, r => r.Name, r => r.Assignments.IncludeWithDefaultProperties(assignment => assignment.Project, assignment => assignment.Task));
            //projContext.ExecuteQuery();

            //Load the project's  Tasks specified by ProjectGuid
            projContext.Load(projContext.Projects,
            Pro => Pro.Include(projectDetail => projectDetail.Tasks.IncludeWithDefaultProperties(
            task => task.Assignments.Include(a => a.Resource.Name).Where(a => a.Resource.Name == ResourceName), task => task.CustomFields.Include(c => c.InternalName, c => c.Name)))
            .Where(PubProj => PubProj.Id == ProjectGuid));
            projContext.ExecuteQuery();

            List<PWAAssignment> TaskLIst = new List<PWAAssignment>();
            foreach (PublishedProject Project in projContext.Projects)
            {
                foreach (var task in Project.Tasks)
                {
                    if (task.PercentComplete < 100)
                    {
                        PWAAssignment NwAssignment = new PWAAssignment();
                        NwAssignment.ID = task.WorkBreakdownStructure;
                        NwAssignment.taskName = task.Name;
                        NwAssignment.StartDate = task.Start.ToShortDateString(); ;
                        NwAssignment.EndDate = task.Finish.ToShortDateString();
                        NwAssignment.LastModifiedDate = task.Modified.ToShortDateString();
                                                
                        foreach (var CusFld in task.CustomFields)
                        {
                            try
                            {
                                string cmp = ((string[])(task[CusFld.InternalName]))[0].ToString();
                                LookupEntry en = lkpEntries.FirstOrDefault(ent => "Entry_" + ent.Id.ToString().Replace("-", "") == cmp);
                                NwAssignment.customFields.Add(CusFld.Name, en.FullValue);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                        TaskLIst.Add(NwAssignment);
                    }

                }                  

            }
            return TaskLIst;
        }





        /// <summary>
        /// Get lookup table entries for all the lookup tables in the project.
        /// </summary>
        /// <param name="projContext"></param>
        /// <returns>List<LookupEntry></returns>
        public List<LookupEntry> getAllLookupEntries(ProjectContext projContext)
        {
            List<LookupEntry> lkpEntries = new List<LookupEntry>();

            //Get Collection of all Lookup tables in the project
            var projLutCollection = projContext.LoadQuery(projContext.LookupTables.Include(lut => lut.Entries));
            projContext.ExecuteQuery();

            if (projLutCollection.Count() > 0)
            {
                foreach (LookupTable lut in projLutCollection)
                {

                    // Get the collection of lookup table entries.
                    lkpEntries.AddRange(lut.Entries);

                }
            }

            return lkpEntries;

        }



        /// <summary>
        /// Get list of lookup table entries for all lookup tables whose names are passed as the parameter
        /// </summary>
        /// <param name="projContext"></param>
        /// <param name="lkpTableNames"></param>
        /// <returns>List<LookupEntry> for all the lookup tables mentioned in the lkpTableNames</LookupEntries></returns>
        public List<LookupEntry> getAllLookupEntries(ProjectContext projContext, string[] lkpTableNames)
        {
            List<LookupEntry> lkpEntries = new List<LookupEntry>();

            foreach (string lkpTableName in lkpTableNames)
            {
                try
                {
                    var projLutCollection = projContext.LoadQuery(
                    projContext.LookupTables.Include(lut => lut.Entries)
                        .Where(lut => lut.Name == lkpTableName));
                    projContext.ExecuteQuery();

                    if (projLutCollection.Count() > 0)
                    {
                        foreach (LookupTable lut in projLutCollection)
                        {
                            // Get the collection of lookup table entries.
                            lkpEntries.AddRange(lut.Entries);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Decide the method or location where we want to log the intenal Web API exceptions.
                    //Continue with the loop
                    Console.WriteLine("Error occured while retriving values from a lookup table");
                }
            }

            return lkpEntries;
        }

        /// <summary>
        /// Get All task lookup table entry values 
        /// </summary>
        /// <param name="projContext">ProjectContext</param>
        /// <param name="lkpTableNames">name of all the task lookup tables</param>
        /// <returns>dictionary of lookup table and their entry values.</returns>
        public Dictionary<string, List<string>> getAllLkpTablesWithEntries(ProjectContext projContext, string[] lkpTableNames)
        {
            Dictionary<string, List<string>> lkpEntries = new Dictionary<string, List<string>>();

            foreach (string lkpTableName in lkpTableNames)
            {
                try
                {
                    var projLutCollection = projContext.LoadQuery(
                    projContext.LookupTables.Include(lut => lut.Entries, lut => lut.Name)
                        .Where(lut => lut.Name == lkpTableName));
                    projContext.ExecuteQuery();

                    if (projLutCollection.Count() > 0)
                    {
                        foreach (LookupTable lut in projLutCollection)
                        {
                            // Get the collection of lookup table entries.
                            List<string> entries = new List<string>();
                            foreach (LookupEntry en in lut.Entries)
                            {
                                entries.Add(en.FullValue);
                            }
                            lkpEntries.Add(lut.Name, entries);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Decide the method or location where we want to log the intenal Web API exceptions.
                    //Continue with the loop
                    Console.WriteLine("Error occured while retriving values from a lookup table");
                }
            }

            return lkpEntries;
        }


        /// <summary>
        /// Update tasks to the project server
        /// </summary>
        /// <param name="projContext">PWA Context</param>
        /// <param name="ProjectGuid">Guid of the project in concern</param>
        /// <param name="tasks">tasks to be updated</param>
        /// <returns>Return job status.</returns>
        internal JobState UpdateTasks(ProjectContext projContext, Guid ProjectGuid, List<TaskDetail> tasks)
        {
            //First we will get the lookup table entries from all the concern lookup tables 
            List<LookupEntry> lkpEntries = getAllLookupEntries(projContext, lkpTableNames);

            // Get the Project Context            
            PublishedProject project = projContext.Projects.GetByGuid(ProjectGuid);
            if (project == null)
            {
                Console.WriteLine("No Project Found");
            }

            DraftProject tempDraft = project.Draft;
            JobState state = projContext.WaitForQueue(tempDraft.CheckIn(true), 20);
            DraftProject draft = project.CheckOut();

            foreach (TaskDetail AssignmentDetail in tasks)
            {
                // Retrieve project tasks 
                //DraftTask task = draft.Tasks.GetByGuid(AssignmentDetail.TaskID);
                DraftTask task = draft.Tasks.GetById("13");
                // EnterpriseResource self = EnterpriseResource.GetSelf(projContext);
                //projContext.Load(task,t=> t.Assignments.IncludeWithDefaultProperties(assignment => assignment.PercentWorkComplete, assignment => assignment.ActualStart, assignment => assignment.ActualFinish));
                projContext.Load(task, t => t.Name,t => t.Id);
                //DraftAssignment assignment = draft.Assignments.GetByGuid(AssignmentDetail.AssignmentID);
                //projContext.Load(assignment,a => a.Task);
                //projContext.Load(self);
                projContext.ExecuteQuery();

                // Check if task in present
                if (task == null)
                {
                    Console.WriteLine("No Task present");
                    continue;
                }

                // Check if task in present
                //if (assignment == null)
                //{
                //    Console.WriteLine("No assignment present");
                //    continue;
                //}

                
                //projContext.Load(self, r => r.Name, r => r.Assignments.IncludeWithDefaultProperties(assignment => assignment.Project).Where(self.Assignments => self.Assignments.GetByGuid((AssignmentDetail.AssignmentID)));
                //projContext.ExecuteQuery();

               // DraftAssignment assignment = draft.Assignments.GetByGuid(AssignmentDetail.AssignmentID);
              

                if (AssignmentDetail.Status == "Complete")
                {
                    //self.Assignments.GetByGuid(AssignmentDetail.AssignmentID).PercentComplete = 100;
                    task.PercentComplete = 100;                
                   //task.Assignments.GetByGuid(AssignmentDetail.AssignmentID).PercentWorkComplete = 100;
                    //task.Assignments.GetByGuid(AssignmentDetail.AssignmentID).PercentWorkComplete = 100;
                }

                // assignment.Task.Start = AssignmentDetail.StartDate;
                // assignment.Task.Finish = AssignmentDetail.EndDate;

               // task.IsManual = true;
                 task.Start = AssignmentDetail.StartDate;
               // task.Duration = ((AssignmentDetail.EndDate - AssignmentDetail.StartDate).Days + "d");
                task.Finish = AssignmentDetail.EndDate;

                var job1 = draft.Update();

                if (WaitForJob(projContext, job1))
                {
                    //task.Finish = AssignmentDetail.EndDate;
                    // task.Assignments.GetByGuid(AssignmentDetail.AssignmentID).up
                    //self.Assignments.Update();
                    //task.Start = AssignmentDetail.StartDate;
                    //task.Finish = AssignmentDetail.EndDate;
                    //  self.Assignments.GetByGuid(AssignmentDetail.AssignmentID).Start = Convert.ToDateTime(AssignmentDetail.StartDate);
                    // self.Assignments.GetByGuid(AssignmentDetail.AssignmentID).Finish = Convert.ToDateTime(AssignmentDetail.EndDate);
                    // draft.Tasks.GetByGuid(AssignmentDetail.TaskID).Start = AssignmentDetail.StartDate;
                    // draft.Tasks.GetByGuid(AssignmentDetail.TaskID).Finish = AssignmentDetail.EndDate;
                    projContext.Load(task.CustomFields);
                    // self.Assignments.Update();
                    //self.Assignments.SubmitAllStatusUpdates("");
                    projContext.ExecuteQuery();

                    if (AssignmentDetail.Function != null)
                    {
                        CustomField taskCF = task.CustomFields.FirstOrDefault(cf => cf.Name == "Function");
                        // Get lookup table entry needs to be updated 
                        LookupEntry taskLookUpEntry = lkpEntries.Find(e => e.FullValue == AssignmentDetail.Function);
                        if (taskCF != null && taskLookUpEntry != null)
                        {
                            task[taskCF.InternalName] = new[] { taskLookUpEntry.InternalName };
                        }
                    }
                    if (AssignmentDetail.LaunchPriority != null)
                    {
                        CustomField taskCF = task.CustomFields.FirstOrDefault(cf => cf.Name == "Launch Priority");
                        // Get lookup table entry needs to be updated 
                        LookupEntry taskLookUpEntry = lkpEntries.Find(e => e.FullValue == AssignmentDetail.LaunchPriority);
                        if (taskCF != null && taskLookUpEntry != null)
                        {
                            task[taskCF.InternalName] = new[] { taskLookUpEntry.InternalName };
                        }
                    }
                    if (AssignmentDetail.Organisation != null)
                    {
                        CustomField taskCF = task.CustomFields.FirstOrDefault(cf => cf.Name == "Organisation");
                        // Get lookup table entry needs to be updated 
                        LookupEntry taskLookUpEntry = lkpEntries.Find(e => e.FullValue == AssignmentDetail.Organisation);
                        if (taskCF != null && taskLookUpEntry != null)
                        {
                            task[taskCF.InternalName] = new[] { taskLookUpEntry.InternalName };
                        }
                    }
                    if (AssignmentDetail.Status != null)
                    {
                        CustomField taskCF = task.CustomFields.FirstOrDefault(cf => cf.Name == "Shire Status");
                        // Get lookup table entry needs to be updated 
                        LookupEntry taskLookUpEntry = lkpEntries.Find(e => e.FullValue == AssignmentDetail.Status);
                        if (taskCF != null && taskLookUpEntry != null)
                        {
                            task[taskCF.InternalName] = new[] { taskLookUpEntry.InternalName };
                        }


                    }
                }

            }
            // Update project and check in

            JobState jobState = new JobState();
            var job = draft.Update();
            if(WaitForJob(projContext, job))
            {

                job = draft.Publish(true);
                if (WaitForJob(projContext, job))
                {
                    jobState = JobState.Success;
                }
                //return job;
            }
           // projContext.Load(draft);
            //draft.Publish(true);
            //projContext.ExecuteQuery();
            //projContext.Projects.Update();
           // projContext.ExecuteQuery();
           // JobState jobState = projContext.WaitForQueue(draft.Publish(true), 10);

            //return projContext.WaitForQueue(projContext.Projects.Update(), 10);
            return jobState;
        }


        private static bool WaitForJob(ProjectContext context, QueueJob job)
        {
            bool result = true;

            try
            {
                context.WaitForQueue(job, int.MaxValue);
            }
            catch (ServerException ex)
            {              
                result = false;
            }
            return result;
        }

    }


}


