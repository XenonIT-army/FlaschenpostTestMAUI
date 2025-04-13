using static System.Net.WebRequestMethods;

namespace FlaschenpostTestMAUI.Data
{
    public static class Constants
    {
        public static string BaseAddress =
        DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
       
        public static string RestUrlTodoItem = $"{BaseAddress}/api/todoitem/";
        public static string RestUrlCategory = $"{BaseAddress}/api/category/";
        public static string RestUrlProject = $"{BaseAddress}/api/project/";
    }
}