using FlaschenpostTestMAUI.Interfaces;
using FlaschenpostTestMAUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.ServiceManagers
{
    public abstract class BaseServiceManager<T> : IServiceManager<T>, IDisposable where T : class
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        protected static string _baseUrl = string.Empty;
        public BaseServiceManager()
        {
            SetServiceUrl();
            var handler = new HttpClientHandler();

            #if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            #endif

            _client = new HttpClient(handler);
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        public async Task<T> AddAsync(T item)
        {
            System.Uri uri = new System.Uri(string.Format(_baseUrl, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<T>(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await _client.PostAsync(uri, content);
              
                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tTodoItem successfully saved.");

                // Deserialize the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                var deserializedItem = JsonSerializer.Deserialize<T>(responseContent, _serializerOptions);
                if (deserializedItem != null)
                {
                    // Update the ID of the item
                    var idProperty = typeof(T).GetProperty("ID");
                    if (idProperty != null)
                    {
                        idProperty.SetValue(item, idProperty.GetValue(deserializedItem));
                    }
                    return deserializedItem;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return null;
        }

        public async Task<bool> Delete(T item)
        {
            Uri uri = new Uri(string.Format(_baseUrl));

            try
            {
                string json = JsonSerializer.Serialize<T>(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = uri,
                    Content = content
                };
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(@"\tTodoItem successfully deleted.");

                    string responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedItem = JsonSerializer.Deserialize<bool>(responseContent, _serializerOptions);
                    return deserializedItem;
                }
                   
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return false;
        }
      
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            Uri uri = new Uri(string.Format(_baseUrl, string.Empty));
            try
            {

                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var items = JsonSerializer.Deserialize<List<T>>(content, _serializerOptions);
                    return items;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return null;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            Uri uri = new Uri(string.Format($"{_baseUrl}{id}", string.Empty));

            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(@"\tTodoItem successfully deleted.");
                    // Deserialize the response content
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedItem = JsonSerializer.Deserialize<T>(responseContent, _serializerOptions);
                    return deserializedItem;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return null;
        }
        public async Task<bool> Update(T item)
        {
            Uri uri = new Uri(string.Format(_baseUrl, string.Empty));
            HttpResponseMessage response = null;
            try
            {
                string json = JsonSerializer.Serialize<T>(item, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _client.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedItem = JsonSerializer.Deserialize<bool>(responseContent, _serializerOptions);
                    return deserializedItem;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return false;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        protected abstract void SetServiceUrl();
    }
}
