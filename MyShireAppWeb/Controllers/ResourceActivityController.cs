using MyShireAppWeb.Models;
using MyShireAppWeb.Utilities;
//using MyTaskList.Utilities;
using Microsoft.ProjectServer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web;

namespace MyShireAppWeb.Controllers
{
    public class ResourceActivityController : ApiController
    {

        [HttpGet]
        [Route("api/Projects/ResourceAssignmentList")]
        [ActionName("ResourceAssignment")]
        public HttpResponseMessage GetResourcesAssignment()
        {
            // var ProjContext = ContextUtility.GetProjectContext();
            var context = new HttpContextWrapper(HttpContext.Current);
            HttpRequestBase myrequest = context.Request;
            var ProjContext = ContextHelper.GetClientContext(myrequest.Url);

            //ProjContext.Load(ProjContext.Web, web => web.Title);
            //ProjContext.ExecuteQuery();
            //ViewBag.Message = ProjContext.Web.Title;

            PWAUtility pwaUtil = new PWAUtility();

            List<PWAAssignment> PWA_ResourceAssignment = pwaUtil.ResourceAssignments(ProjContext);
            return Request.CreateResponse(HttpStatusCode.OK, PWA_ResourceAssignment);
        }



        [HttpPost]
        [Route("api/Task/UpdateTaskCustomField")]
        [ActionName("UpdateTaskCustomField")]
        public HttpResponseMessage UpdateTaskCustomField(AssignmentDataObject AssignmentUpdatedData)
        {
            var ProjContext = ContextUtility.GetProjectContext();

            PWAUtility PWAUtil = new PWAUtility();


            List<TaskDetail> Tasks = new List<TaskDetail>();
            Guid ProjectUID;
            for (int i = 0; i < AssignmentUpdatedData.Resource_Assignment.Count; i++)
            {
                ProjectUID = AssignmentUpdatedData.Resource_Assignment[i].ProjectID;
                TaskDetail AssignmentDetail = new TaskDetail();
                AssignmentDetail.taskName = AssignmentUpdatedData.Resource_Assignment[i].taskName;
                AssignmentDetail.TaskID = AssignmentUpdatedData.Resource_Assignment[i].TaskID;
                AssignmentDetail.ProjectUID = AssignmentUpdatedData.Resource_Assignment[i].ProjectID;
                AssignmentDetail.Status = AssignmentUpdatedData.Resource_Assignment[i].Status;
                AssignmentDetail.StartDate = AssignmentUpdatedData.Resource_Assignment[i].StartDate;
                AssignmentDetail.EndDate = AssignmentUpdatedData.Resource_Assignment[i].EndDate;
                AssignmentDetail.LaunchPriority = AssignmentUpdatedData.Resource_Assignment[i].LaunchPriority;
                AssignmentDetail.Organisation = AssignmentUpdatedData.Resource_Assignment[i].Organisation;
                AssignmentDetail.Function = AssignmentUpdatedData.Resource_Assignment[i].Function;
                AssignmentDetail.AssignmentID = AssignmentUpdatedData.Resource_Assignment[i].AssignmentID;
                Tasks.Add(AssignmentDetail);
                try
                {
                    if (PWAUtil.UpdateTasks(ProjContext, ProjectUID, Tasks) == JobState.Success)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Value Updated successfully");
                    }

                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable To Update the TaskField");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

                }


            }

            //bool ProjectCreated = PWAUtil.CreatePWAProject(ProjContext, PWAResourceAssignment);

            //if (  PWAUtil.UpdateCustomFieldValues(ProjContext,ProjectUID,taskName, Status) == JobState.Success)
            //    {
            //        return Request.CreateResponse(HttpStatusCode.OK, "Value Updated successfully");
            //    }
            //  else
            //    {
            //        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable To Update the TaskField");
            //    }


            return Request.CreateResponse(HttpStatusCode.OK, "Value Updated successfully");
        }


        [HttpGet]
        [Route("api/Resources/CurrentResourceProjects/{UserDisplayUID}")]
        [ActionName("CurrentResourceProjects")]
        public HttpResponseMessage CurrentResourceProjects(Guid UserDisplayUID)
        {
            var ProjContext = ContextUtility.GetProjectContext();
            PWAUtility pwaUtil = new PWAUtility();

            List<ProjectsInfo> projects = pwaUtil.GetCurrentResourceProjects(ProjContext, UserDisplayUID);


            return Request.CreateResponse(HttpStatusCode.OK, projects);
        }



    }
}
