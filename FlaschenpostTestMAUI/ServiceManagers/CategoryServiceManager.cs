using FlaschenpostTestMAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.ServiceManagers
{
    internal class CategoryServiceManager : BaseServiceManager<Category>
    {
        public CategoryServiceManager()
        {
        }
        protected override void SetServiceUrl()
        {
            _baseUrl = Constants.RestUrlCategory;
        }

      
    }
}
