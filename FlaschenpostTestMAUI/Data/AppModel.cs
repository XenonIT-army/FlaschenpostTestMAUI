using FlaschenpostTestMAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.Data
{
    public class AppModel
    {
        public List<TodoItem> Tasks = [];
        public List<Project> Projects = [];
        public List<Category> Categories = [];
    }
}
