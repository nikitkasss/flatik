﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Flatik.Data;
using Flatik.Data.Entities;
using Flatik.Data.Repositories;
using Flatik.Monitoring.Deserializers;
using Flatik.Monitoring.Extensions;
using Flatik.Monitoring.Models;
using Flatik.Monitoring.Settings;
using Mapster;

namespace Flatik.Monitoring.Monitor
{
    public class Monitor
    {
        private readonly MonitoringSettings _settings;
        private readonly IFlatRepository _flatRepository;

        public Monitor(MonitoringSettings settings, IFlatRepository flatRepository)
        {
            _settings = settings;
            _flatRepository = flatRepository;
        }

        public event Action<List<Flat>> NewFlats;

        public void Run()
        {
            while (true)
            {
                var flats = _settings.Sites.SelectMany(SendRequest);
                var newFlats = GetNewFlats(flats);

                if (newFlats.Count > 0)
                {
                    NewFlats?.Invoke(newFlats);
                }

                Thread.Sleep(_settings.IntervalInSeconds * 1000);
            }
        }

        private List<Flat> GetNewFlats(IEnumerable<Flat> flats)
        {
            var newFlats = flats.Where(x => _flatRepository.IsExists(x.Id, x.Site)).ToList();
            
            _flatRepository.AddRange(newFlats.Adapt<IEnumerable<FlatEntity>>());

            return newFlats;
        }

        private IEnumerable<Flat> SendRequest(SiteSettings site)
        {
            var baseUrl = site.Url;
            var queryString = site.Parameters.ToQueryString();

            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(queryString).Result;
            if (!response.IsSuccessStatusCode)
            {
                // TODO: Throw an exception.
                return null;
            }

            var result = response.Content.ReadAsStringAsync().Result;
            var type = Type.GetType(site.DeserializerType);

            // TODO: Create deserializer creator.

            if (!(Activator.CreateInstance(type, args: site.Name) is IDeserializer helper))
            {
                return new List<Flat>();
            }
            
            var deserializedObject = helper.Deserialize(result);
            return deserializedObject;
        }
    }
}