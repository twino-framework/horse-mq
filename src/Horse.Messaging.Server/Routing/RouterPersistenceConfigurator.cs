﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Horse.Messaging.Server.Routing;

public class RouterPersistenceConfigurator : IPersistenceConfigurator<RouterConfiguration>
{
    private readonly string _path;
    private readonly string _filename;

    private List<RouterConfiguration> _configurations = new List<RouterConfiguration>();

    public RouterPersistenceConfigurator(string path, string filename)
    {
        if (!path.EndsWith('\\') && !path.EndsWith('/'))
            path += "/";

        _path = path;
        _filename = filename;
    }

    public RouterConfiguration[] Load()
    {
        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        string fullname = $"{_path}{_filename}";

        if (!File.Exists(fullname))
        {
            File.WriteAllText(fullname, "[]");

            lock (_configurations)
            {
                _configurations = new List<RouterConfiguration>();
                return _configurations.ToArray();
            }
        }

        string json = File.ReadAllText(fullname);

        lock (_configurations)
        {
            _configurations = System.Text.Json.JsonSerializer.Deserialize<List<RouterConfiguration>>(json);
            return _configurations.ToArray();
        }
    }

    public void Save()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(_configurations);
        File.WriteAllText($"{_path}{_filename}", json);
    }

    public void Add(RouterConfiguration item)
    {
        lock (_configurations)
            _configurations.Add(item);
    }

    public RouterConfiguration Find(Func<RouterConfiguration, bool> predicate)
    {
        lock (_configurations)
        {
            RouterConfiguration configuration = _configurations.FirstOrDefault(predicate);
            return configuration;
        }
    }

    public void Remove(Func<RouterConfiguration, bool> predicate)
    {
        lock (_configurations)
        {
            RouterConfiguration configuration = _configurations.FirstOrDefault(predicate);
            if (configuration != null)
                _configurations.Remove(configuration);
        }
    }

    public void Remove(RouterConfiguration item)
    {
        lock (_configurations)
            _configurations.Remove(item);
    }

    public void Clear()
    {
        lock (_configurations)
            _configurations.Clear();
    }
}