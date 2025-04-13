using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.ServiceManagers
{
    public class ProjectServiceManager : BaseServiceManager<Project>
    {
        public ProjectServiceManager()
        {
        }
        protected override void SetServiceUrl()
        {
            _baseUrl = Constants.RestUrlProject;
        }

    }
  
}
